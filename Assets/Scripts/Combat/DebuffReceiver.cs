using UnityEngine;
using System.Collections;

public class DebuffReceiver : MonoBehaviour
{
    Health health;
    Coroutine burnCoroutine;
    GameObject burnParticleInstance;

    void Awake() => health = GetComponent<Health>();

    public void ApplyBurn(float damagePerTick, float duration, GameObject particlePrefab)
    {
        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);
        if (burnParticleInstance != null)
        {
            Destroy(burnParticleInstance);
            burnParticleInstance = null;
        }
        burnCoroutine = StartCoroutine(BurnRoutine(damagePerTick, duration, particlePrefab));
    }

    IEnumerator BurnRoutine(float damagePerTick, float duration, GameObject particlePrefab)
    {
        if (particlePrefab != null)
        {
            burnParticleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity, transform);
            burnParticleInstance.transform.localPosition = Vector3.zero;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Vector2 worldSize = sr.bounds.size;
                burnParticleInstance.transform.localScale = new Vector3(worldSize.x, worldSize.y, 1f);
            }
        }

        float tickInterval = 0.5f;
        int totalTicks = Mathf.FloorToInt(duration / tickInterval);

        for (int i = 0; i < totalTicks; i++)
        {
            yield return new WaitForSeconds(tickInterval);
            if (health != null && health.CurrentHP > 0)
                health.TakeDamage(Mathf.RoundToInt(damagePerTick));
        }

        if (burnParticleInstance != null)
        {
            Destroy(burnParticleInstance);
            burnParticleInstance = null;
        }
        burnCoroutine = null;
    }
}
