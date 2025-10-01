using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private float lifeTime = 0.3f;
    private float elapsed;

    private void Update()
    {
        elapsed += Time.deltaTime;

        // büyüt
        transform.localScale += Vector3.one * Time.deltaTime * 2f;

        // saydamlaþtýr
        var renderer = GetComponent<Renderer>();
        var color = renderer.material.color;
        color.a = Mathf.Lerp(1f, 0f, elapsed / lifeTime);
        renderer.material.color = color;

        if (elapsed >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
