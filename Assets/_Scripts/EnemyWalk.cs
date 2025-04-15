using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyWalk : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;

    [Tooltip("Èi sa po štarte pozerá doprava")]
    public bool isFacingRight = true;

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
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;

        bool isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, groundLayer);

        // Ak už nie je zem pod nohami -> flip
        if (!isGrounded)
        {
            Flip();
        }
    }

    private void CheckWall()
    {
        if (wallCheck == null) return;

        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        float checkDistance = 0.5f;

        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, direction, checkDistance, groundLayer);

        // Ak narazil do steny -> flip
        if (hit.collider != null)
        {
            Flip();
        }
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // --- Pomocné debug èiary v Scéne ---
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
