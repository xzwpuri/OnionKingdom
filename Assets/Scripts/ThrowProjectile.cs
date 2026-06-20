using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D projectileCollider;

    float damage;
    Vector2 velocity;
    float gravity = 9.8f;
    float lifeTime = 3f;

    public void Setup(Sprite sprite, float dmg, Vector2 direction, float speed)
    {
        if (sprite != null)
            spriteRenderer.sprite = sprite;

        damage = dmg;
        velocity = direction * speed;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 포물선 이동
        velocity.y -= gravity * Time.deltaTime;
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // 이동 방향에 맞게 회전 (자연스러운 회전 낙하)
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log($"[Throw] 충돌: {other.gameObject.name} | 데미지: {damage}");
        Destroy(gameObject);
    }
}