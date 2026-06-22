using UnityEngine;

public class FallingLeaf : MonoBehaviour
{
    [SerializeField] float fallSpeed = 1.5f;
    [SerializeField] float rotationSpeed = 90f;
    [SerializeField] float driftSpeed = 0.3f; // 독자적인 좌우 이동 속도
    [SerializeField] int damage = 1;
    [SerializeField] LayerMask groundLayer;

    Vector2 windForce = Vector2.zero;
    float driftDirection = 1f;
    float rotationDirection = 1f;

    private void Start()
    {
        // 랜덤하게 좌/우 회전 및 이동 방향 결정
        bool goRight = Random.value > 0.5f;
        driftDirection = goRight ? 1f : -1f;
        rotationDirection = goRight ? 1f : -1f;
    }

    private void Update()
    {
        Vector2 driftVelocity = Vector2.right * driftDirection * driftSpeed;
        Vector2 velocity = Vector2.down * fallSpeed + driftVelocity + windForce;

        transform.position += (Vector3)(velocity * Time.deltaTime);
        transform.Rotate(0f, 0f, rotationSpeed * rotationDirection * Time.deltaTime);
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
                if (health.IsInvincible) return;

                Vector2 dir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
                health.TakeDamage(damage, dir);
                Destroy(gameObject);
            }
            return;
        }

        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}