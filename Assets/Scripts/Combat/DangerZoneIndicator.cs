using UnityEngine;

public class DangerZoneIndicator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color zoneColor = new Color(1f, 0f, 0f, 0.4f);

    public void Setup(Vector2 size)
    {
        spriteRenderer.color = zoneColor;
        transform.localScale = new Vector3(size.x, size.y, 1f);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}