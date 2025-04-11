using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Movement")]
    public float speed = 8f;
    public float jumpForce = 15f;
    public float movementCheckDistance = 0.6f;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    bool isDashing;

    [Header("WallSlide & WallJump")]
    public Transform wallCheck;
    public Transform groundCheck;
    public float wallCheckDistance = 0.5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public float wallSlideSpeed = 2f;
    public float wallJumpForceX = 12f;
    public float wallJumpForceY = 15f;
    public float stickTime = 3f;
    public float wallJumpCooldown = 0.3f;

    bool isGrounded;
    bool isTouchingWall;
    bool isWallSliding;
    bool canWallStick = true;
    float wallStickCounter;
    float horizontal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallStickCounter = stickTime;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal > 0 && !isFacingRight())
            Flip();
        else if (horizontal < 0 && isFacingRight())
            Flip();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
                Jump();
            else if (isWallSliding || isTouchingWall)
                StartCoroutine(WallJump());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            StartCoroutine(Dash());

        WallSlideCheck();
    }

    void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (!isWallSliding)
        {
            // ak hráè ide doprava (horizontal = 1), skontroluj èi môže
            // ak hráè ide do¾ava (horizontal = -1), skontroluj èi môže
            if ((horizontal > 0 && !CanMoveInDirection(1)) || (horizontal < 0 && !CanMoveInDirection(-1)))
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                Vector2 targetVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.2f);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }

        CheckGround();
        CheckWall();
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    IEnumerator WallJump()
    {
        canWallStick = false; // Doèasne zakáž prilepenie na stenu

        float jumpDirection = isFacingRight() ? -1 : 1;
        rb.linearVelocity = new Vector2(wallJumpForceX * jumpDirection, wallJumpForceY);

        isWallSliding = false;
        wallStickCounter = stickTime;

        yield return new WaitForSeconds(wallJumpCooldown);

        canWallStick = true; // znova umožni prilepenie
    }

    IEnumerator Dash()
    {
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(horizontal * dashSpeed, 0f);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    void WallSlideCheck()
    {
        if (isTouchingWall && !isGrounded && horizontal != 0 && canWallStick)
        {
            if (wallStickCounter > 0)
            {
                wallStickCounter -= Time.deltaTime;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                isWallSliding = false;
            }
            else
            {
                isWallSliding = true;
            }
        }
        else
        {
            isWallSliding = false;
            wallStickCounter = stickTime;
        }
    }

    void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }

    void CheckWall()
    {
        Vector2 direction = isFacingRight() ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, wallLayer);
        isTouchingWall = hit.collider != null;
    }

    bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // WallCheck Raycast
        Vector2 direction = isFacingRight() ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(wallCheck.position, (Vector2)wallCheck.position + direction * wallCheckDistance);

        // GroundCheck Raycast
        Gizmos.DrawLine(groundCheck.position, (Vector2)groundCheck.position + Vector2.down * groundCheckDistance);

        // Movement Check Raycast
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * movementCheckDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.left * movementCheckDistance);
    }

    bool CanMoveInDirection(float dir)
    {
        Vector2 direction = dir > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, movementCheckDistance, groundLayer);
        return hit.collider == null;
    }
}
