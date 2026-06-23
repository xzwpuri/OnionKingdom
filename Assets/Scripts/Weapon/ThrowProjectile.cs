using UnityEngine;
using System.Collections.Generic;

public class ThrowProjectile : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D projectileCollider;

    float damage;
    Vector2 velocity;
    float gravity = 9.8f;
    float lifeTime = 3f;
    List<WordData> adjectives;

    public void Setup(Sprite sprite, float dmg, Vector2 direction, float speed, Vector2 sizeMultiplier, List<WordData> adjectives = null)
    {
        if (sprite != null)
            spriteRenderer.sprite = sprite;

        damage = dmg;
        velocity = direction * speed;
        this.adjectives = adjectives;
        transform.localScale = new Vector3(sizeMultiplier.x, sizeMultiplier.y, 1f);

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

        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            Vector2 dir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            targetHealth.TakeDamage(Mathf.RoundToInt(damage), dir);
            AdjEffectApplier.Apply(adjectives, targetHealth, transform.position);
        }

        Destroy(gameObject);
    }
}