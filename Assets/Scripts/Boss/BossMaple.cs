using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossMaple : BossBase
{
    [Header("Leaf Pattern")]
    [SerializeField] FallingLeaf leafPrefab;
    [SerializeField] Vector2 leafSpawnXRangeRelative = new Vector2(-8f, 8f);
    [SerializeField] float leafSpawnYRelative = 8f;
    [SerializeField] float leafInterval = 1f;
    [SerializeField] float leafIntervalPhase2 = 0.5f;

    [Header("Root Growth Pattern")]
    [SerializeField] Vector2[] rootPositions;
    [SerializeField] int rootDamage = 1;
    [SerializeField] float rootPreDelay = 0.5f;
    [SerializeField] float rootPostDelay = 0.5f;
    [SerializeField] float rootHeight = 2f;
    [SerializeField] float rootWidth = 1.5f;
    [SerializeField] GameObject rootPrefab;
    [SerializeField] float rootLifeTime = 0.5f;

    [Header("Autumn Wind Pattern")]
    [SerializeField] float windForce = 3f;
    [SerializeField] float windDuration = 3f;

    [Header("Root Surge Pattern (땅파기 유사)")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int surgeDamage = 2;
    [SerializeField] float surgeTrackDuration = 0.5f;
    [SerializeField] float surgeWaitDuration = 0.8f;
    [SerializeField] float surgePostDelay = 0.5f;
    [SerializeField] float surgeHeightAbovePlayer = 4f;
    [SerializeField] GameObject surgeRootPrefab;
    [SerializeField] float surgeLifeTime = 0.3f;

    [Header("Pattern Timing")]
    [SerializeField] float patternInterval = 3f;

    [Header("Danger Zone")]
    [SerializeField] DangerZoneIndicator dangerZonePrefab;

    [Header("Death")]
    [SerializeField] Sprite witheredSprite;

    SpriteRenderer spriteRenderer;
    float leafTimer = 0f;
    float patternTimer = 0f;
    bool isUsingPattern = false;
    bool isWindActive = false;
    Vector2 currentWindForce = Vector2.zero;
    bool isDead = false;

    List<FallingLeaf> activeLeaves = new List<FallingLeaf>();

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isDead) return;

        leafTimer += Time.deltaTime;
        float currentLeafInterval = (health.CurrentHP <= health.MaxHP / 2) ? leafIntervalPhase2 : leafInterval;
        if (leafTimer >= currentLeafInterval)
        {
            leafTimer = 0f;
            SpawnLeaf();
        }

        if (isWindActive && player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
                pc.ApplyExternalForce(currentWindForce);
        }

        if (isUsingPattern) return;

        patternTimer += Time.deltaTime;
        if (patternTimer >= patternInterval)
        {
            patternTimer = 0f;
            StartCoroutine(UseRandomPattern());
        }
    }

    private void SpawnLeaf()
    {
        float x = transform.position.x + Random.Range(leafSpawnXRangeRelative.x, leafSpawnXRangeRelative.y);
        float y = transform.position.y + leafSpawnYRelative;
        Vector2 spawnPos = new Vector2(x, y);
        FallingLeaf leaf = Instantiate(leafPrefab, spawnPos, Quaternion.identity);

        if (isWindActive)
            leaf.ApplyWind(currentWindForce * 0.5f);

        activeLeaves.Add(leaf);
    }

    private IEnumerator UseRandomPattern()
    {
        isUsingPattern = true;

        int pattern = Random.Range(0, 3);
        if (pattern == 0)
            yield return StartCoroutine(RootGrowthPattern());
        else if (pattern == 1)
            yield return StartCoroutine(AutumnWindPattern());
        else
            yield return StartCoroutine(RootSurgePattern());

        isUsingPattern = false;
    }

    private IEnumerator RootGrowthPattern()
    {
        Debug.Log("[단풍나무] 뿌리 과성장 시작");

        List<DangerZoneIndicator> zones = new List<DangerZoneIndicator>();
        List<Vector2> groundPositions = new List<Vector2>();

        foreach (var relativePos in rootPositions)
        {
            float worldX = transform.position.x + relativePos.x;

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(worldX, transform.position.y + 10f), Vector2.down, 30f, groundLayer);
            float groundY = hit.collider != null ? hit.point.y : transform.position.y;

            Vector2 groundPos = new Vector2(worldX, groundY);
            groundPositions.Add(groundPos);

            Vector2 zoneCenter = groundPos + Vector2.up * (rootHeight / 2f);
            DangerZoneIndicator zone = Instantiate(dangerZonePrefab, zoneCenter, Quaternion.identity);
            zone.Setup(new Vector2(rootWidth, rootHeight));
            zone.SetDamage(rootDamage);
            zones.Add(zone);
        }

        yield return new WaitForSeconds(rootPreDelay);

        foreach (var groundPos in groundPositions)
        {
            if (rootPrefab != null)
            {
                GameObject root = Instantiate(rootPrefab, groundPos, Quaternion.identity);
                RootObject rootObj = root.GetComponent<RootObject>();
                if (rootObj != null)
                    rootObj.SetTargetHeight(rootHeight);
                Destroy(root, rootLifeTime);
            }
        }

        yield return new WaitForSeconds(0.2f);

        foreach (var zone in zones)
            zone.Remove();

        Debug.Log("[단풍나무] 뿌리 과성장 솟아오름");

        yield return new WaitForSeconds(rootPostDelay);
    }

    private IEnumerator AutumnWindPattern()
    {
        Debug.Log("[단풍나무] 가을 바람 시작");

        if (player == null) yield break;

        Vector2 windDir = ((Vector2)transform.position - (Vector2)player.position).normalized;
        currentWindForce = windDir * windForce;
        isWindActive = true;

        foreach (var leaf in activeLeaves)
        {
            if (leaf != null)
                leaf.ApplyWind(currentWindForce * 0.5f);
        }

        yield return new WaitForSeconds(windDuration);

        isWindActive = false;
        player.GetComponent<PlayerController>()?.ClearExternalForce();

        foreach (var leaf in activeLeaves)
        {
            if (leaf != null)
                leaf.ClearWind();
        }

        Debug.Log("[단풍나무] 가을 바람 종료");
    }

    private IEnumerator RootSurgePattern()
    {
        Debug.Log("[단풍나무] 뿌리 급성장 시작");

        if (player == null) yield break;

        float t = 0f;
        Vector2 targetPos = player.position;

        while (t < surgeTrackDuration)
        {
            t += Time.deltaTime;
            targetPos = player.position;
            yield return null;
        }

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(targetPos.x, targetPos.y + 5f), Vector2.down, 20f, groundLayer);
        float groundY = hit.collider != null ? hit.point.y : targetPos.y - 5f;

        float targetHeight = targetPos.y + surgeHeightAbovePlayer;
        float rootLength = targetHeight - groundY;

        Vector2 zoneCenter = new Vector2(targetPos.x, groundY + rootLength / 2f);
        DangerZoneIndicator zone = Instantiate(dangerZonePrefab, zoneCenter, Quaternion.identity);
        zone.Setup(new Vector2(1.5f, rootLength));
        zone.SetDamage(surgeDamage);

        yield return new WaitForSeconds(surgeWaitDuration);

        zone.Remove();

        if (surgeRootPrefab != null)
        {
            GameObject root = Instantiate(surgeRootPrefab, new Vector2(targetPos.x, groundY), Quaternion.identity);
            RootObject rootObj = root.GetComponent<RootObject>();
            if (rootObj != null)
                rootObj.SetTargetHeight(rootLength);
            Destroy(root, surgeLifeTime);
        }

        Debug.Log("[단풍나무] 뿌리 급성장 솟아오름");

        yield return new WaitForSeconds(surgePostDelay);
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        isDead = true;
        StopAllCoroutines();

        if (bossCollider != null)
            bossCollider.enabled = false;

        if (spriteRenderer != null && witheredSprite != null)
            spriteRenderer.sprite = witheredSprite;
    }
}