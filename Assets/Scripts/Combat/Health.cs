using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHP = 10;
    int currentHP;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    public Action<int, int> OnHPChanged; // (currentHP, maxHP)
    public Action OnDeath;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (currentHP <= 0) return;

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        OnHPChanged?.Invoke(currentHP, maxHP);
        Debug.Log($"{gameObject.name} µ¥¹ÌÁö {amount} | HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} »ç¸Á");
        OnDeath?.Invoke();
    }
}