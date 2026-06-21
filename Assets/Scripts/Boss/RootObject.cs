using UnityEngine;

public class RootObject : MonoBehaviour
{
    [SerializeField] int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            Vector2 dir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            health.TakeDamage(damage, dir);
        }
    }
}