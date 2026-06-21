using UnityEngine;
using System.Collections;

public class HitWeaponObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer effectSpriteRenderer;
    [SerializeField] Transform nounSpriteTransform; // 회전시킬 대상이라 Transform으로
    [SerializeField] SpriteRenderer nounSpriteRenderer;
    [SerializeField] Collider2D hitCollider;
    [SerializeField] Animator animator;
    [SerializeField] float activeDuration = 0.2f;

    float damage;

    public void Setup(Sprite nounSprite, float dmg, Vector2 direction, Transform player, float offsetDistance, Vector2 sizeMultiplier)
    {
        damage = dmg;

        transform.position = (Vector2)player.position + direction * offsetDistance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        bool flip = Mathf.Abs(angle) > 90f;
        effectSpriteRenderer.flipY = flip;

        if (nounSpriteRenderer != null)
        {
            if (nounSprite != null)
            {
                nounSpriteRenderer.sprite = nounSprite;
                nounSpriteRenderer.flipY = flip;
                nounSpriteRenderer.gameObject.SetActive(true);

                Vector3 baseScale = nounSpriteTransform.localScale; // 기본 비율 (X:3, Y:0.5)
                nounSpriteTransform.localScale = new Vector3(
                    baseScale.x * sizeMultiplier.x,
                    baseScale.y * sizeMultiplier.y,
                    1f
                );

                StartCoroutine(SwingNounSprite(flip));
            }
            else
            {
                nounSpriteRenderer.gameObject.SetActive(false);
            }
        }

        if (animator != null)
            animator.Play("Swing", -1, 0f);

        StartCoroutine(ActiveRoutine());
    }

    private IEnumerator SwingNounSprite(bool flip)
    {
        float t = 0f;
        float startAngle = flip ? -30f : 30f;
        float endAngle = flip ? 30f : -30f;

        while (t < activeDuration)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(startAngle, endAngle, t / activeDuration);
            nounSpriteTransform.localRotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }

    private IEnumerator ActiveRoutine()
    {
        hitCollider.enabled = true;
        yield return new WaitForSeconds(activeDuration);
        hitCollider.enabled = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
            targetHealth.TakeDamage(Mathf.RoundToInt(damage));
    }
}