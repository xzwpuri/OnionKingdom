using UnityEngine;
using System.Collections;

public class BossCarrot : BossBase
{
    [Header("Movement")]
    [SerializeField] float moveJumpForce = 4f;
    [SerializeField] float moveJumpHorizontalSpeed = 2f;

    [Header("High Jump Pattern")]
    [SerializeField] int highJumpDamage = 1;
    [SerializeField] float highJumpForce = 15f;
    [SerializeField] float highJumpPreSinkDistance = 0.3f;
    [SerializeField] float highJumpPreDelay = 0.5f;
    [SerializeField] float highJumpPostDelay = 1f;
    [SerializeField] float screenWidth = 30f;

    [Header("Dig Pattern")]
    [SerializeField] float digSinkDistance = 5f;
    [SerializeField] float digSinkDuration = 0.4f;
    [SerializeField] float digTrackDuration = 1f;
    [SerializeField] float digWaitDuration = 0.8f;
    [SerializeField] float digRiseDuration = 0.4f;
    [SerializeField] float digPostDelay = 0.5f;

    [Header("Pattern Timing")]
    [SerializeField] float patternInterval = 4f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Danger Zone")]
    [SerializeField] DangerZoneIndicator dangerZonePrefab;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Vector2 bossSize;
    float originalGravityScale;
    float patternTimer = 0f;
    bool isUsingPattern = false;
    bool isGrounded = true;
    bool isDead = false;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossSize = spriteRenderer != null ? spriteRenderer.bounds.size : Vector2.one;
        originalGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (isDead) return;
        if (isUsingPattern) return;

        patternTimer += Time.deltaTime;

        if (isGrounded)
        {
            if (patternTimer >= patternInterval)
            {
                patternTimer = 0f;
                StartCoroutine(UseRandomPattern());
                return;
            }

            StartCoroutine(MoveJump());
        }
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        isDead = true;
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        if (bossCollider != null)
            bossCollider.enabled = false;
    }

    private IEnumerator MoveJump()
    {
        isGrounded = false;

        if (player == null) yield break;
        float dir = player.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * moveJumpHorizontalSpeed, moveJumpForce);

        yield return new WaitForSeconds(0.1f);

        while (!Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer))
        {
            yield return null;
        }

        isGrounded = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        // 착지 데미지는 보스 Trigger Collider의 충돌 판정으로 처리됨 (BossBase)
    }

    private IEnumerator UseRandomPattern()
    {
        isUsingPattern = true;

        int pattern = Random.Range(0, 2);
        if (pattern == 0)
            yield return StartCoroutine(HighJumpPattern());
        else
            yield return StartCoroutine(DigPattern());

        isUsingPattern = false;
    }

    private IEnumerator HighJumpPattern()
    {
        Debug.Log("[당근] 높은 점프 시작");

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        Vector2 startPos = transform.position;
        Vector2 sinkPos = startPos + Vector2.down * highJumpPreSinkDistance;

        float t = 0f;
        while (t < highJumpPreDelay)
        {
            t += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, sinkPos, t / highJumpPreDelay);
            yield return null;
        }

        DangerZoneIndicator zone = Instantiate(dangerZonePrefab, new Vector2(startPos.x, startPos.y - bossSize.y / 2f), Quaternion.identity);
        zone.Setup(new Vector2(screenWidth, 0.3f));
        zone.SetDamage(highJumpDamage);

        transform.position = startPos;
        rb.gravityScale = originalGravityScale;
        rb.linearVelocity = new Vector2(0f, highJumpForce);

        yield return new WaitForSeconds(0.2f);

        while (!Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer))
        {
            yield return null;
        }

        // 착지 즉시 속도/중력 정지하고 위치를 startPos(원래 지면 위치)로 보정
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        transform.position = startPos;

        zone.Remove();

        Debug.Log("[당근] 높은 점프 착지");

        yield return new WaitForSeconds(highJumpPostDelay);

        // 후딜레이 끝나면 중력 복구 (이동 점프 재개 가능하게)
        rb.gravityScale = originalGravityScale;
    }

    private IEnumerator DigPattern()
    {
        Debug.Log("[당근] 땅 파기 시작");

        if (bossCollider != null) bossCollider.enabled = false;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        Vector2 startPos = transform.position;
        Vector2 sinkPos = startPos + Vector2.down * digSinkDistance;

        float t = 0f;
        while (t < digSinkDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, sinkPos, t / digSinkDuration);
            yield return null;
        }

        t = 0f;
        Vector2 targetPos = transform.position;
        while (t < digTrackDuration)
        {
            t += Time.deltaTime;
            if (player != null)
                targetPos = new Vector2(player.position.x, transform.position.y);
            transform.position = targetPos;
            yield return null;
        }

        // 솟아오를 위치(지면 기준)에 보스 크기만큼 범위 표시 (시각적 경고용, 데미지는 충돌로 처리)
        Vector2 zoneCenter = new Vector2(targetPos.x, startPos.y);
        DangerZoneIndicator zone = Instantiate(dangerZonePrefab, zoneCenter, Quaternion.identity);
        zone.Setup(bossSize);
        // SetDamage 호출 안 함 - 데미지는 솟아오른 보스의 충돌 판정으로 처리됨

        yield return new WaitForSeconds(digWaitDuration);

        zone.Remove(); // 데미지 없이 단순 제거

        // 솟아오름
        Vector2 riseStartPos = transform.position;
        Vector2 riseEndPos = zoneCenter;

        t = 0f;
        while (t < digRiseDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector2.Lerp(riseStartPos, riseEndPos, t / digRiseDuration);
            yield return null;
        }

        if (bossCollider != null) bossCollider.enabled = true;
        rb.gravityScale = originalGravityScale;

        Debug.Log("[당근] 땅 파기 솟아오름");

        yield return new WaitForSeconds(digPostDelay);
    }
}