using UnityEngine;
using System.Collections;

public static class WeaponLibrary
{
    public static void Hit(MonoBehaviour caster, Transform player, WeaponData weapon, Vector2 targetPos, HitWeaponObject hitPrefab)
    {
        Vector2 direction = (targetPos - (Vector2)player.position).normalized;
        float damage = 10f;
        Sprite sprite = null;

        if (weapon.nounEntries.Count > 0)
        {
            NounEntry entry = weapon.nounEntries[0];
            damage += entry.noun.damageModifier;
            sprite = entry.noun.worldSprite;

            foreach (var adj in entry.appliedAdjectives)
                damage += adj.adjectiveDamageBonus;
        }

        float offsetDistance = 1.2f;
        HitWeaponObject hitObj = Object.Instantiate(hitPrefab, player.position, Quaternion.identity);
        hitObj.Setup(sprite, damage, direction, player, offsetDistance);
    }

    public static void Throw(MonoBehaviour caster, Transform player, WeaponData weapon, Vector2 targetPos)
    {
        caster.StartCoroutine(ThrowRoutine(player, weapon, targetPos));
    }

    private static IEnumerator ThrowRoutine(Transform player, WeaponData weapon, Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)player.position).normalized;

        foreach (var entry in weapon.nounEntries)
        {
            float damage = 10f + entry.noun.damageModifier;
            foreach (var adj in entry.appliedAdjectives)
                damage += adj.adjectiveDamageBonus;

            Debug.Log($"[Throw] {entry.noun.word} 투척 | 방향: {direction} | 데미지: {damage}");

            // TODO: 실제 투사체 오브젝트 생성 (포물선 이동)

            yield return new WaitForSeconds(0.3f); // 연발 간격
        }
    }
}