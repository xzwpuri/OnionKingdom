using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHP = 10;
    int currentHP;
    bool isInvincible = false;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    public Action<int, int> OnHPChanged;
    public Action OnDeath;
    public Action<int, Vector2> OnDamageTaken;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount, Vector2 hitDirection = default)
    {
        if (currentHP <= 0) return;
        if (isInvincible) return;

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        OnHPChanged?.Invoke(currentHP, maxHP);
        OnDamageTaken?.Invoke(amount, hitDirection);
        Debug.Log($"{gameObject.name} 데미지 {amount} | HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망");
        OnDeath?.Invoke();
    }
}