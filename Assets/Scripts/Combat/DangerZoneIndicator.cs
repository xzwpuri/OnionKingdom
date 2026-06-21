using UnityEngine;

public class DangerZoneIndicator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color zoneColor = new Color(1f, 0f, 0f, 0.4f);

    Vector2 zoneSize;
    int damage = 0;

    public void Setup(Vector2 size)
    {
        zoneSize = size;
        spriteRenderer.color = zoneColor;
        transform.localScale = new Vector3(size.x, size.y, 1f);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void Remove()
    {
        DealDamageIfOverlapping();
        Destroy(gameObject);
    }

    private void DealDamageIfOverlapping()
    {
        if (damage <= 0) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, zoneSize, 0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null)
                {
                    Vector2 dir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
                    playerHealth.TakeDamage(damage, dir);
                }
            }
        }
    }
}