using UnityEngine;

public class FallingLeaf : MonoBehaviour
{
    [SerializeField] float fallSpeed = 1.5f;
    [SerializeField] float rotationSpeed = 90f;
    [SerializeField] int damage = 1;
    [SerializeField] LayerMask groundLayer;

    Vector2 windForce = Vector2.zero;

    private void Update()
    {
        Vector2 velocity = Vector2.down * fallSpeed + windForce;
        transform.position += (Vector3)(velocity * Time.deltaTime);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public void ApplyWind(Vector2 force)
    {
        windForce = force;
    }

    public void ClearWind()
    {
        windForce = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                Vector2 dir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
                health.TakeDamage(damage, dir);
            }
            Destroy(gameObject);
            return;
        }

        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}