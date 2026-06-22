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
            Vector2 diff = (Vector2)other.transform.position - (Vector2)transform.position;
            if (Mathf.Abs(diff.x) < 0.1f)
                diff.x = Random.value > 0.5f ? 1f : -1f;
            Vector2 hitDir = diff.normalized;

            playerHealth.TakeDamage(contactDamage, hitDir, ignoreInvincible: true);
        }
    }

    protected virtual void HandleDeath()
    {
        Debug.Log($"{gameObject.name} 처치됨");
    }
}