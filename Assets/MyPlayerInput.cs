using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayerInput : MonoBehaviour
{
    public bool IsPressLeft => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
    public bool IsPressRight => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
    public bool IsPressUp => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);

    [Header("UI Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button rotateButton;

    private bool uiPressLeft, uiPressRight, uiPressRotate;

    private void Start()
    {
        if (leftButton != null) leftButton.onClick.AddListener(() => uiPressLeft = true);
        if (rightButton != null) rightButton.onClick.AddListener(() => uiPressRight = true);
        if (rotateButton != null) rotateButton.onClick.AddListener(() => uiPressRotate = true);
    }

    private void Update()
    {
        if (IsPressLeft || uiPressLeft)
        {
            var isMovable = GameManager.Instance.IsInside(GetPreviewHorizontalPosition(-1));
            if (isMovable) MoveHorizontal(-1);
            uiPressLeft = false;
        }
        else if (IsPressRight || uiPressRight)
        {
            var isMovable = GameManager.Instance.IsInside(GetPreviewHorizontalPosition(1));
            if (isMovable) MoveHorizontal(1);
            uiPressRight = false;
        }
        else if (IsPressUp || uiPressRotate)
        {
            var isRotatable = GameManager.Instance.IsInside(GetPreviewPosition());
            if (isRotatable) Rotate();
            uiPressRotate = false;
        }
    }

    private List<Vector2> GetPreviewPosition()
    {
        var result = new List<Vector2>();
        var listPiece = GameManager.Instance.Current.ListPiece;
        var pivot = GameManager.Instance.Current.transform.position;
        foreach (var piece in listPiece)
        {
            var position = piece.position;

            position -= pivot;
            position = new Vector3(position.y, -position.x, 0);
            position += pivot;

            result.Add(position);
        }
        return result;
    }

    private List<Vector2> GetPreviewHorizontalPosition(int value)
    {
        var result = new List<Vector2>();
        var listPiece = GameManager.Instance.Current.ListPiece;
        foreach (var piece in listPiece)
        {
            var position = piece.position;
            position.x += value;
            result.Add(position);
        }
        return result;
    }

    private void MoveHorizontal(int value)
    {
        var current = GameManager.Instance.Current.transform;
        var position = current.position;
        position.x += value;
        current.position = position;
    }

    private void Rotate()
    {
        var current = GameManager.Instance.Current.transform;
        var angles = current.eulerAngles;
        angles.z += -90;
        current.eulerAngles = angles;
    }
}
