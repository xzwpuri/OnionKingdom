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

    Rigidbody2D rb;
    int jumpCount;
    bool isGrounded;
    bool isKnockedBack = false;
    Vector2 externalForce = Vector2.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        CheckGround();

        if (!isKnockedBack)
            Move();

        Jump();
        UpdateGravity();
    }
    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = hit != null;
        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;
            if (isKnockedBack)
                isKnockedBack = false;
        }
    }
    void Move()
    {
        float input = Input.GetAxisRaw("Horizontal");
        float finalX = input * moveSpeed + externalForce.x;
        rb.linearVelocity = new Vector2(finalX, rb.linearVelocity.y);
        if (input > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (input < 0) transform.localScale = new Vector3(-1, 1, 1);
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
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = fallGravityScale;
        else
            rb.gravityScale = riseGravityScale;
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void ApplyPush(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void ApplyExternalForce(Vector2 force)
    {
        externalForce = force;
    }

    public void ClearExternalForce()
    {
        externalForce = Vector2.zero;
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