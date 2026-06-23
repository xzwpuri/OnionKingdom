using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossBase : MonoBehaviour
{
    public event System.Action OnDefeated;

    [SerializeField] protected Health health;
    [SerializeField] protected int contactDamage = 1;
    [SerializeField] protected Collider2D bossCollider;
    [SerializeField] protected float contactCooldown = 0.5f;
    [SerializeField] protected float patternInterval = 4f;
    [SerializeField] protected Animator animator;

    protected Transform player;
    protected bool isDead = false;
    protected bool isActivated = false;
    protected bool isUsingPattern = false;
    protected float patternTimer = 0f;

    public void Activate() => isActivated = true;

    float lastContactTime = -999f;
    float lastHurtAnimTime = -999f;
    const float hurtAnimCooldown = 0.5f;
    protected List<DangerZoneIndicator> activeDangerZones = new List<DangerZoneIndicator>();

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (health != null)
        {
            health.OnDeath += HandleDeath;
            health.OnDamageTaken += (_, __) => OnHurt();
        }
    }

    protected virtual void Update()
    {
        if (!isActivated) return;
        if (isDead) return;
        if (isUsingPattern) return;

        UpdatePatternTimer();
    }

    // 자식 클래스가 패턴 사용 조건(예: 땅에 있을 때만)을 다르게 하고 싶으면 오버라이드
    protected virtual void UpdatePatternTimer()
    {
        patternTimer += Time.deltaTime;
        if (patternTimer >= patternInterval)
        {
            patternTimer = 0f;
            StartCoroutine(UseRandomPatternWrapper());
        }
    }

    private IEnumerator UseRandomPatternWrapper()
    {
        isUsingPattern = true;
        yield return StartCoroutine(UseRandomPattern());
        isUsingPattern = false;
    }

    // 자식 클래스에서 반드시 구현
    protected virtual IEnumerator UseRandomPattern()
    {
        yield break;
    }

    protected DangerZoneIndicator SpawnDangerZone(DangerZoneIndicator prefab, Vector2 position)
    {
        DangerZoneIndicator zone = Instantiate(prefab, position, Quaternion.identity);
        activeDangerZones.Add(zone);
        return zone;
    }

    protected void RemoveDangerZone(DangerZoneIndicator zone)
    {
        activeDangerZones.Remove(zone);
        zone.Remove();
    }

    private void ClearAllDangerZones()
    {
        foreach (var zone in activeDangerZones)
        {
            if (zone != null)
                Destroy(zone.gameObject);
        }
        activeDangerZones.Clear();
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastContactTime < contactCooldown) return;

        lastContactTime = Time.time;

        Health playerHealth = other.GetComponent<Health>();
        if (playerHealth != null)
        {
            Vector2 diff = (Vector2)other.transform.position - (Vector2)transform.position;
            if (Mathf.Abs(diff.x) < 0.1f)
                diff.x = Random.value > 0.5f ? 1f : -1f;
            Vector2 hitDir = diff.normalized;

            playerHealth.TakeDamage(contactDamage, hitDir, ignoreInvincible: true);
        }
    }

    void OnHurt()
    {
        if (isDead) return;
        if (Time.time - lastHurtAnimTime < hurtAnimCooldown) return;
        lastHurtAnimTime = Time.time;
        PlayTrigger(AnimParam.Hurt);
    }

    protected virtual void HandleDeath()
    {
        Debug.Log($"{gameObject.name} 처치됨");
        isDead = true;
        StopAllCoroutines();
        ClearAllDangerZones();
        PlayTrigger(AnimParam.Death);

        if (bossCollider != null)
            bossCollider.enabled = false;

        OnDefeated?.Invoke();
    }

    protected void SetBool(string param, bool value)
    {
        if (animator != null) animator.SetBool(param, value);
    }

    protected void PlayTrigger(string param)
    {
        if (animator != null) animator.SetTrigger(param);
    }
}