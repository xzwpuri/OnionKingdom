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
    bool isGrounded = true;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossSize = spriteRenderer != null ? spriteRenderer.bounds.size : Vector2.one;
        originalGravityScale = rb.gravityScale;
    }

    protected override void Update()
    {
        if (isDead) return;
        if (isUsingPattern) return;

        patternTimer += Time.deltaTime;

        if (isGrounded)
        {
            if (patternTimer >= patternInterval)
            {
                patternTimer = 0f;
                StartCoroutine(UseRandomPatternWrapper());
                return;
            }

            StartCoroutine(MoveJump());
        }
    }

    private IEnumerator UseRandomPatternWrapper()
    {
        isUsingPattern = true;
        yield return StartCoroutine(UseRandomPattern());
        isUsingPattern = false;
    }

    private IEnumerator MoveJump()
    {
        isGrounded = false;
        SetBool(AnimParam.IsMoving, true);

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
        SetBool(AnimParam.IsMoving, false);
    }

    protected override IEnumerator UseRandomPattern()
    {
        int pattern = Random.Range(0, 2);
        if (pattern == 0)
            yield return StartCoroutine(HighJumpPattern());
        else
            yield return StartCoroutine(DigPattern());
    }

    private IEnumerator HighJumpPattern()
    {
        Debug.Log("[당근] 높은 점프 시작");
        PlayTrigger(AnimParam.CarrotHighJump);

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

        DangerZoneIndicator zone = SpawnDangerZone(dangerZonePrefab, new Vector2(startPos.x, startPos.y - bossSize.y / 2f));
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

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        transform.position = startPos;

        RemoveDangerZone(zone);

        Debug.Log("[당근] 높은 점프 착지");

        yield return new WaitForSeconds(highJumpPostDelay);

        rb.gravityScale = originalGravityScale;
    }

    private IEnumerator DigPattern()
    {
        Debug.Log("[당근] 땅 파기 시작");
        PlayTrigger(AnimParam.CarrotDig);

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

        Vector2 zoneCenter = new Vector2(targetPos.x, startPos.y);
        DangerZoneIndicator zone = SpawnDangerZone(dangerZonePrefab, zoneCenter);
        zone.Setup(bossSize);

        yield return new WaitForSeconds(digWaitDuration);

        RemoveDangerZone(zone);

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

    protected override void HandleDeath()
    {
        base.HandleDeath();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
    }
}