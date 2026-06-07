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
        bool wasGrounded = isGrounded;
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = hit != null;
        if (!wasGrounded && isGrounded)
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
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}