using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BlockController Current { get; set; }

    private const int GridSizeX = 10;
    private const int GridSizeY = 20;

    public bool[,] Grid = new bool[GridSizeX, GridSizeY];

    public float GameSpeed => gameSpeed;
    [SerializeField, Range(.1f, 1f)] private float gameSpeed = 1;

    [SerializeField] private List<BlockController> listPrefabs;

    private List<BlockController> _listHistory = new List<BlockController>();

    [Header("UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private RectTransform nextPreviewPanel; // ✅ UI panel (NEXT altı)

    private int score = 0;
    private BlockController nextBlock;

    // UI için Image prefab (küçük kare göstermek için)
    [SerializeField] private Image blockUIPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        nextBlock = listPrefabs[UnityEngine.Random.Range(0, listPrefabs.Count)];
        Spawn();
        UpdateScoreUI();
    }

    public bool IsInside(List<Vector2> listCoordinate)
    {
        foreach (var coordinate in listCoordinate)
        {
            int x = Mathf.RoundToInt(coordinate.x);
            int y = Mathf.RoundToInt(coordinate.y);

            if (x < 0 || x >= GridSizeX) return false;
            if (y < 0 || y >= GridSizeY) return false;
            if (Grid[x, y]) return false;
        }
        return true;
    }

    public void Spawn()
    {
        // Şu anki Next sahneye gelir
        var newBlock = Instantiate(nextBlock);
        Current = newBlock;
        _listHistory.Add(newBlock);

        // Yeni Next seçilir
        nextBlock = listPrefabs[UnityEngine.Random.Range(0, listPrefabs.Count)];

        // UI temizle
        foreach (Transform child in nextPreviewPanel)
            Destroy(child.gameObject);

        // Yeni Next bloğunu UI içine çizeriz
        DrawNextBlockUI(nextBlock);
    }

    private void DrawNextBlockUI(BlockController blockPrefab)
    {
        if (blockPrefab == null || blockUIPrefab == null) return;

        // Pivot bloğun merkezini almak yerine tüm karelerin pozisyonlarını topla
        List<Vector3> offsets = new List<Vector3>();
        foreach (var piece in blockPrefab.ListPiece)
        {
            offsets.Add(piece.localPosition);
        }

        // Min - Max değerleri bul (blok boyutları)
        float minX = Mathf.Min(offsets.ConvertAll(p => p.x).ToArray());
        float maxX = Mathf.Max(offsets.ConvertAll(p => p.x).ToArray());
        float minY = Mathf.Min(offsets.ConvertAll(p => p.y).ToArray());
        float maxY = Mathf.Max(offsets.ConvertAll(p => p.y).ToArray());

        float width = maxX - minX + 1;
        float height = maxY - minY + 1;

        // Panel boyutunu al
        float panelWidth = nextPreviewPanel.rect.width;
        float panelHeight = nextPreviewPanel.rect.height;

        // Kaç px = 1 blok?
        float scale = Mathf.Min(panelWidth / width, panelHeight / height);

        // Ortalamayı bul (blok merkezi)
        Vector2 blockCenter = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);

        // Kareleri çiz
        foreach (var piece in blockPrefab.ListPiece)
        {
            var uiBlock = Instantiate(blockUIPrefab, nextPreviewPanel);
            uiBlock.color = piece.GetComponent<SpriteRenderer>().color;

            // Offset hesapla
            Vector2 offset = (Vector2)piece.localPosition - blockCenter;

            // UI pozisyonu
            uiBlock.rectTransform.anchoredPosition = offset * scale;
            uiBlock.rectTransform.sizeDelta = Vector2.one * scale;
        }
    }


    private bool IsFullRow(int index)
    {
        for (int i = 0; i < GridSizeX; i++)
        {
            if (!Grid[i, index]) return false;
        }
        return true;
    }

    public void UpdateRemoveObjectController()
    {
        for (int i = 0; i < GridSizeY; i++)
        {
            var isFull = IsFullRow(i);
            if (isFull)
            {
                foreach (var myBlock in _listHistory)
                {
                    var willDestroy = new List<Transform>();
                    foreach (var piece in myBlock.ListPiece)
                    {
                        int y = Mathf.RoundToInt(piece.position.y);
                        if (y == i)
                        {
                            willDestroy.Add(piece);
                        }
                        else if (y > i)
                        {
                            var position = piece.position;
                            position.y--;
                            piece.position = position;
                        }
                    }

                    foreach (var item in willDestroy)
                    {
                        myBlock.ListPiece.Remove(item);
                        Destroy(item.gameObject);
                    }
                }

                for (int j = 0; j < GridSizeX; j++)
                    Grid[j, i] = false;

                for (int j = i + 1; j < GridSizeY; j++)
                    for (int k = 0; k < GridSizeX; k++)
                        Grid[k, j - 1] = Grid[k, j];

                AddScore(UnityEngine.Random.Range(50, 150));
                UpdateRemoveObjectController();
                return;
            }
        }
    }

    private void AddScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public void OnGameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0;
    }
}
