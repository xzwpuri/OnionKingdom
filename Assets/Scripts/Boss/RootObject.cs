using UnityEngine;

public class RootObject : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float growDuration = 0.2f;

    Vector3 targetScale;
    Vector3 basePosition;
    float t = 0f;
    bool growing = true;

    public void SetTargetHeight(float height)
    {
        targetScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
    }

    private void Start()
    {
        basePosition = transform.position;

        if (targetScale == Vector3.zero)
            targetScale = transform.localScale;

        transform.localScale = new Vector3(targetScale.x, 0f, targetScale.z);
    }

    private void Update()
    {
        if (!growing) return;

        t += Time.deltaTime;
        float ratio = Mathf.Clamp01(t / growDuration);

        transform.localScale = new Vector3(targetScale.x, targetScale.y * ratio, targetScale.z);
        transform.position = basePosition + Vector3.up * (targetScale.y * ratio / 2f);

        if (ratio >= 1f)
            growing = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            Vector2 diff = (Vector2)other.transform.position - (Vector2)transform.position;

            if (Mathf.Abs(diff.x) < 0.1f)
                diff.x = Random.value > 0.5f ? 1f : -1f;

            Vector2 dir = diff.normalized;
            health.TakeDamage(damage, dir);
        }
    }
}