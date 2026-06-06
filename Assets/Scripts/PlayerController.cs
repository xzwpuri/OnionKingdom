using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 14f;
    [SerializeField] int maxJumpCount = 2;

    [Header("Gravity")]
    [SerializeField] float fallGravityScale = 4f;   // 낙하 시 중력
    [SerializeField] float riseGravityScale = 2f;   // 상승 시 중력

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundLayer;

    Rigidbody2D rb;
    int jumpCount;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGround();
        Move();
        Jump();
        UpdateGravity();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
            jumpCount = 0;
    }

    void Move()
    {
        float input = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(input * moveSpeed, rb.linearVelocity.y);

        if (input > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (input < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }
    }

    void UpdateGravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = fallGravityScale;
        else
            rb.gravityScale = riseGravityScale;
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