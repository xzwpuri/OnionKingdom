using UnityEngine;

public class BossBase : MonoBehaviour
{
    [SerializeField] protected Health health;
    [SerializeField] protected int contactDamage = 1;
    [SerializeField] protected Collider2D bossCollider;
    [SerializeField] protected float contactCooldown = 0.5f;

    protected Transform player;
    float lastContactTime = -999f;

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (health != null)
            health.OnDeath += HandleDeath;
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastContactTime < contactCooldown) return;

        lastContactTime = Time.time;

        Health playerHealth = other.GetComponent<Health>();
        if (playerHealth != null)
        {
            Vector2 hitDir = (other.transform.position - transform.position).normalized;
            playerHealth.TakeDamage(contactDamage, hitDir);
        }
    }

    protected virtual void HandleDeath()
    {
        Debug.Log($"{gameObject.name} 처치됨");
    }
}