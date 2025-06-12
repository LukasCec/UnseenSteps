// EnemyWalk.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyWalk : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;

    [Tooltip("Či sa po štarte pozerá doprava")]
    public bool isFacingRight = true;

    private float lastFlipTime;
    public float flipCooldown = 0.2f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckGround();
        CheckWall();
        Patrol();
    }

    private void Patrol()
    {
        // ← use rb.velocity, not linearVelocity
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
    }

    private void CheckGround()
    {
        if (Time.time - lastFlipTime < flipCooldown) return;

        bool isGrounded = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            0.3f,
            groundLayer
        );

        if (!isGrounded)
        {
            Flip();
            lastFlipTime = Time.time;
        }
    }

    private void CheckWall()
    {
        if (wallCheck == null) return;

        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(
            wallCheck.position,
            dir,
            0.5f,
            groundLayer
        );
        if (hit.collider != null)
            Flip();
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.3f);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 dir = isFacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * 0.5f);
        }
    }
}
