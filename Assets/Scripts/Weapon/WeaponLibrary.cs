using UnityEngine;
using System.Collections;

public static class WeaponLibrary
{
    public static void Hit(MonoBehaviour caster, Transform player, WeaponData weapon, Vector2 targetPos, HitWeaponObject hitPrefab)
    {
        Vector2 direction = (targetPos - (Vector2)player.position).normalized;
        float damage = 10f;
        Sprite sprite = null;
        Vector2 sizeMultiplier = Vector2.one;
        if (weapon.nounEntries.Count > 0)
        {
            NounEntry entry = weapon.nounEntries[0];
            damage += entry.noun.damageModifier;
            sprite = entry.noun.worldSprite;
            sizeMultiplier = entry.noun.sizeModifier;
            foreach (var adj in entry.appliedAdjectives)
                damage += adj.adjectiveDamageBonus;
        }
        float offsetDistance = 1.2f;
        HitWeaponObject hitObj = Object.Instantiate(hitPrefab, player.position, Quaternion.identity);
        hitObj.Setup(sprite, damage, direction, player, offsetDistance, sizeMultiplier);
    }

    public static void Throw(MonoBehaviour caster, Transform player, WeaponData weapon, Vector2 targetPos, ThrowProjectile throwPrefab)
    {
        caster.StartCoroutine(ThrowRoutine(player, weapon, targetPos, throwPrefab));
    }

    private static IEnumerator ThrowRoutine(Transform player, WeaponData weapon, Vector2 targetPos, ThrowProjectile throwPrefab)
    {
        Vector2 direction = (targetPos - (Vector2)player.position).normalized;
        float speed = 8f;
        foreach (var entry in weapon.nounEntries)
        {
            float damage = 10f + entry.noun.damageModifier;
            Vector2 sizeMultiplier = entry.noun.sizeModifier;
            foreach (var adj in entry.appliedAdjectives)
                damage += adj.adjectiveDamageBonus;
            ThrowProjectile proj = Object.Instantiate(throwPrefab, player.position, Quaternion.identity);
            proj.Setup(entry.noun.worldSprite, damage, direction, speed, sizeMultiplier);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public static void Shoot(MonoBehaviour caster, Transform player, WeaponData weapon, Vector2 targetPos, ShootProjectile shootPrefab)
    {
        caster.StartCoroutine(ShootRoutine(player, weapon, targetPos, shootPrefab));
    }

    private static IEnumerator ShootRoutine(Transform player, WeaponData weapon, Vector2 targetPos, ShootProjectile shootPrefab)
    {
        Vector2 direction = (targetPos - (Vector2)player.position).normalized;
        float speed = 14f;
        foreach (var entry in weapon.nounEntries)
        {
            float damage = 10f + entry.noun.damageModifier;
            Vector2 sizeMultiplier = entry.noun.sizeModifier;
            foreach (var adj in entry.appliedAdjectives)
                damage += adj.adjectiveDamageBonus;
            ShootProjectile proj = Object.Instantiate(shootPrefab, player.position, Quaternion.identity);
            proj.Setup(entry.noun.worldSprite, damage, direction, speed, sizeMultiplier);
            yield return new WaitForSeconds(0.15f);
        }
    }
}