using UnityEngine;
using System.Collections;

public class HitWeaponObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D hitCollider;
    [SerializeField] Animator animator; // 휘두르는 애니메이션 재생용
    [SerializeField] float activeDuration = 0.2f; // 히트박스 활성 시간

    float damage;

    public void Setup(Sprite sprite, float dmg, Vector2 direction, Transform player, float offsetDistance)
    {
        damage = dmg;

        transform.position = (Vector2)player.position + direction * offsetDistance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 좌우 반전만 별도 판단 (스프라이트 자체를 위아래로 뒤집어서 좌우 대칭 효과)
        spriteRenderer.flipY = Mathf.Abs(angle) > 90f;

        if (animator != null)
            animator.Play("Swing", -1, 0f);

        StartCoroutine(ActiveRoutine());
    }

    private IEnumerator ActiveRoutine()
    {
        hitCollider.enabled = true;
        yield return new WaitForSeconds(activeDuration);
        hitCollider.enabled = false;
        Destroy(gameObject, 0.1f); // 애니메이션 끝날 시간 살짝 더 줌
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        Debug.Log($"[Hit] 충돌: {other.gameObject.name} | 데미지: {damage}");
    }
}