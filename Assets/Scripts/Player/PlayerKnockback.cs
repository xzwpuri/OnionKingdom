using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] PlayerController playerController;
    [SerializeField] float knockbackPerDamage = 6f;
    [SerializeField] float upwardBoostPerDamage = 4f;
    [SerializeField] float knockbackDuration = 0.2f;

    private void Awake()
    {
        health.OnDamageTaken += HandleKnockback;
    }

    private void HandleKnockback(int damage, Vector2 hitDirection)
    {
        float horizontalForce = damage * knockbackPerDamage;
        float verticalForce = damage * upwardBoostPerDamage;

        // 방향에서 좌우 신호만 추출 (크기 무시, 부호만 사용)
        float horizontalSign = hitDirection.x >= 0 ? 1f : -1f;

        Vector2 finalForce = new Vector2(horizontalSign * horizontalForce, verticalForce);

        playerController.ApplyKnockback(finalForce, knockbackDuration);

        health.SetInvincible(true);
        StartCoroutine(EndInvincibility(knockbackDuration));
    }

    private System.Collections.IEnumerator EndInvincibility(float duration)
    {
        yield return new WaitForSeconds(duration);
        health.SetInvincible(false);
    }
}