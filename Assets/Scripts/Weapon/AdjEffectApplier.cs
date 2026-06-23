using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class AdjEffectApplier
{
    public static void Apply(List<WordData> adjectives, Health target, Vector2 hitPos)
    {
        if (adjectives == null || adjectives.Count == 0) return;

        WordData burnData = adjectives.FirstOrDefault(a => a.debuffType == DebuffType.Burn);
        WordData explosiveData = adjectives.FirstOrDefault(a => a.debuffType == DebuffType.Explosive);

        if (burnData != null)
            ApplyBurnTo(target, burnData);

        if (explosiveData != null)
            Explode(hitPos, explosiveData, burnData);
    }

    static void ApplyBurnTo(Health target, WordData burnData)
    {
        DebuffReceiver receiver = target.GetComponent<DebuffReceiver>();
        if (receiver != null)
            receiver.ApplyBurn(burnData.debuffValue, burnData.debuffDuration, burnData.particleEffectPrefab);
    }

    static void Explode(Vector2 pos, WordData explosiveData, WordData burnData)
    {
        float radius = explosiveData.debuffDuration;
        float damage = explosiveData.debuffValue;

        if (explosiveData.particleEffectPrefab != null)
        {
            GameObject fx = Object.Instantiate(explosiveData.particleEffectPrefab, pos, Quaternion.identity);
            fx.transform.localScale = Vector3.one * radius * 2f;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius);
        foreach (var col in hits)
        {
            if (col.CompareTag("Player")) continue;
            Health h = col.GetComponent<Health>();
            if (h == null) continue;

            Vector2 dir = (Vector2)col.transform.position - pos;
            if (dir == Vector2.zero) dir = Vector2.up;
            h.TakeDamage(Mathf.RoundToInt(damage), dir.normalized);

            if (burnData != null)
                ApplyBurnTo(h, burnData);
        }
    }
}
