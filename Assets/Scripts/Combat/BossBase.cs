using UnityEngine;

public class BossBase : MonoBehaviour
{
    [SerializeField] protected Health health;
    [SerializeField] protected int contactDamage = 1;
    [SerializeField] protected Collider2D bossCollider;
    [SerializeField] protected float contactCooldown = 0.5f; // 연속 충돌 데미지 방지

    protected Transform player;
    float lastContactTime = -999f;

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (Time.time - lastContactTime < contactCooldown) return;

        lastContactTime = Time.time;

        Health playerHealth = collision.gameObject.GetComponent<Health>();
        if (playerHealth != null)
        {
            Vector2 hitDir = (collision.transform.position - transform.position).normalized;
            playerHealth.TakeDamage(contactDamage, hitDir);
        }
    }

    protected virtual void HandleDeath()
    {
        Debug.Log($"{gameObject.name} 처치됨");
    }
}