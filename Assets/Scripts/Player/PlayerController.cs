using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [Header("Jump")]
    [SerializeField] float jumpForce = 15f;
    [SerializeField] int maxJumpCount = 2;
    [Header("Gravity")]
    [SerializeField] float fallGravityScale = 5f;
    [SerializeField] float riseGravityScale = 5f;
    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundLayer;
    [Header("Animation")]
    [SerializeField] Animator animator;

    Rigidbody2D rb;
    int jumpCount;
    bool isGrounded;
    bool isKnockedBack;
    bool isDead;
    Vector2 externalForce;

    float lastHurtAnimTime = -999f;
    const float hurtAnimCooldown = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDamageTaken += (_, __) => OnHurt();
            health.OnDeath += OnDeath;
        }
    }

    void Update()
    {
        if (isDead) return;
        CheckGround();
        if (!isKnockedBack) Move();
        Jump();
        UpdateGravity();
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
        SetBool(AnimParam.IsGrounded, isGrounded);
        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;
            if (isKnockedBack) isKnockedBack = false;
        }
    }

    void Move()
    {
        float input = Input.GetAxisRaw("Horizontal");
        float finalX = input * moveSpeed + externalForce.x;
        rb.linearVelocity = new Vector2(finalX, rb.linearVelocity.y);

        // transform.localScale로 좌우 반전 (자식 오브젝트 포함 일괄 반전)
        if (input > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (input < 0) transform.localScale = new Vector3(-1, 1, 1);

        SetBool(AnimParam.IsMoving, input != 0f);
    }

    void Jump()
    {
        if (isKnockedBack) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpCount = 1;
            }
            else if (jumpCount < maxJumpCount)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpCount++;
            }
        }
    }

    void UpdateGravity()
    {
        rb.gravityScale = rb.linearVelocity.y < 0 ? fallGravityScale : riseGravityScale;
    }

    void OnHurt()
    {
        if (isDead) return;
        if (Time.time - lastHurtAnimTime < hurtAnimCooldown) return;
        lastHurtAnimTime = Time.time;
        PlayTrigger(AnimParam.Hurt);
    }

    void OnDeath()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        PlayTrigger(AnimParam.Death);
        // TODO: 게임오버 처리 / 리스폰 로직
    }

    public void TriggerAttack()
    {
        if (isDead) return;
        PlayTrigger(AnimParam.Attack);
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void ApplyExternalForce(Vector2 force) => externalForce = force;
    public void ClearExternalForce() => externalForce = Vector2.zero;

    void SetBool(string param, bool value)
    {
        if (animator != null) animator.SetBool(param, value);
    }

    void PlayTrigger(string param)
    {
        if (animator != null) animator.SetTrigger(param);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
