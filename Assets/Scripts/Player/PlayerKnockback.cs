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

        Vector2 finalDirection = hitDirection.normalized;
        finalDirection.y = Mathf.Max(finalDirection.y, 0f);

        Vector2 finalForce = new Vector2(finalDirection.x * horizontalForce, verticalForce);

        playerController.ApplyKnockback(finalForce, knockbackDuration);

        // 넉백 지속시간 동안 무적
        health.SetInvincible(true);
        StartCoroutine(EndInvincibility(knockbackDuration));
    }

    private System.Collections.IEnumerator EndInvincibility(float duration)
    {
        yield return new WaitForSeconds(duration);
        health.SetInvincible(false);
    }
}