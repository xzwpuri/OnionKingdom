using UnityEngine;
using System.Collections.Generic;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D projectileCollider;

    float damage;
    Vector2 velocity;
    float lifeTime = 5f;
    List<WordData> adjectives;

    public void Setup(Sprite sprite, float dmg, Vector2 direction, float speed, Vector2 sizeMultiplier, List<WordData> adjectives = null)
    {
        if (sprite != null)
            spriteRenderer.sprite = sprite;

        damage = dmg;
        velocity = direction * speed;
        this.adjectives = adjectives;
        transform.localScale = new Vector3(sizeMultiplier.x, sizeMultiplier.y, 1f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
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
            Vector2 dir = velocity.normalized;
            targetHealth.TakeDamage(Mathf.RoundToInt(damage), dir);
            AdjEffectApplier.Apply(adjectives, targetHealth, transform.position);
        }

        Destroy(gameObject);
    }
}
