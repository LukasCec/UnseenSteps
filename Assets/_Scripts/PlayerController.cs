using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    bool wasGroundedLastFrame;

    [Header("Movement")]
    public float speed = 8f;
    public float jumpForce = 15f;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    bool isDashing;
    [Header("Dash Cooldown")]
    public float dashCooldown = 4f;
    private float lastDashTime = -Mathf.Infinity;

    [Header("Wall Slide & Jump")]
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

    [Header("Movement Block")]
    public float movementCheckDistance = 0.6f;
    bool isGrounded;
    bool isTouchingWall;
    bool isWallSliding;
    bool canWallStick = true;
    float wallStickCounter;
    float horizontal;


    [Header("Dragging")]
    [Tooltip("Layers containing dragable objects")]
    public LayerMask dragableLayer;
    public float dragRange = 1.5f;

    private Dragable currentDrag;
    private bool isDragging;

    [Header("Attack Settings")]
    public GameObject attackHitbox;
    private bool isAttacking;

    [Header("VFX")]
    public GameObject dashEffectPrefab;
    public GameObject jumpEffectPrefab;
    public bool isStunned = false;
    public int damage = 1;

    [Header("Ground + Draggable")]
    [SerializeField] private LayerMask groundAndDragLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        wallStickCounter = stickTime;
        groundAndDragLayer = groundLayer | dragableLayer;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (isDragging)
        {
            // namiesto klasickÈho Flipovania podæa vstupu
            FaceDragable();

            // ukonËenie ùahania
            if (Input.GetMouseButtonUp(1))
                EndDrag();

            // naozaj sa pohni (bez flipu podæa inputu)
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
            return;
        }

        if (isStunned) return;
        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }

        if (horizontal > 0 && !isFacingRight()) Flip();
        else if (horizontal < 0 && isFacingRight()) Flip();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
                Jump();
            else if (isWallSliding || isTouchingWall)
                StartCoroutine(WallJump());
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking && isGrounded)
        {
            isAttacking = true;
            animator.SetTrigger(AnimationStrings.Attack);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Time.time - lastDashTime >= dashCooldown)
        {
            StartCoroutine(Dash());
        }
        if (Input.GetMouseButtonDown(1))
            TryStartDrag();

        WallSlideCheck();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }
        if (!isWallSliding)
        {
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
        Vector3 spawnPos = groundCheck.position;
        Instantiate(jumpEffectPrefab, spawnPos, Quaternion.identity);
    }

    IEnumerator WallJump()
    {
        canWallStick = false;
        float jumpDirection = isFacingRight() ? -1 : 1;
        rb.linearVelocity = new Vector2(wallJumpForceX * jumpDirection, wallJumpForceY);
        isWallSliding = false;
        wallStickCounter = stickTime;

        yield return new WaitForSeconds(wallJumpCooldown);
        canWallStick = true;
    }

    IEnumerator Dash()
    {
        
        isDashing = true;
        lastDashTime = Time.time; // tu sa spustÌ cooldown
        Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);

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
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundAndDragLayer);

        bool wasGrounded = isGrounded;
        isGrounded = hit.collider != null;

        //  Detekuj dopad (bol vo vzduchu ->eraz je na zemi)
        if (!wasGrounded && isGrounded)
        {
            SpawnLandingEffect();
        }

        wasGroundedLastFrame = isGrounded;
    }

    void SpawnLandingEffect()
    {
        if (jumpEffectPrefab != null)
        {
            Instantiate(jumpEffectPrefab, groundCheck.position, Quaternion.identity);
        }
    }

    void CheckWall()
    {
        Vector2 direction = isFacingRight() ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, wallLayer);
        isTouchingWall = hit.collider != null;
    }

    bool CanMoveInDirection(float dir)
    {
        Vector2 direction = dir > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, movementCheckDistance, groundAndDragLayer);
        return hit.collider == null;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    void UpdateAnimator()
    {
        animator.SetBool(AnimationStrings.IsMoving, horizontal != 0);
        animator.SetBool(AnimationStrings.IsGrounded, isGrounded);
        animator.SetFloat(AnimationStrings.YVelocity, rb.linearVelocity.y);
    }

    void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector2 direction = isFacingRight() ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheck.position, (Vector2)wallCheck.position + direction * wallCheckDistance);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, (Vector2)groundCheck.position + Vector2.down * groundCheckDistance);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * movementCheckDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.left * movementCheckDistance);
    }

    public void EnableAttackHitbox()
    {
        // resetneme a zapneme hitbox collider
        attackHitbox.SetActive(false);
        attackHitbox.SetActive(true);

        // zÌskame BoxCollider2D a prepoËÌtame pozÌciu do world space
        BoxCollider2D col = attackHitbox.GetComponent<BoxCollider2D>();
        Vector2 worldPos = attackHitbox.transform.TransformPoint(col.offset);
        Vector2 size = col.size;
        float angle = attackHitbox.transform.eulerAngles.z;

        // OBSAHOVO HºAD¡ME VäETKY objekty v hitboxe
        Collider2D[] hits = Physics2D.OverlapBoxAll(worldPos, size, angle);

        foreach (var h in hits)
        {
            // 1) najprv sk˙siù rozbitn˝ objekt
            Breakable br = h.GetComponent<Breakable>();
            if (br != null)
            {
                br.Hit();
                continue; 
            }

            // 2) potom nepriateæa
            if (h.CompareTag("Enemy"))
            {
                EnemyHealth eh = h.GetComponent<EnemyHealth>();
                if (eh != null)
                    eh.TakeDamage(damage);
            }
        }
    }

    public void DisableAttackHitbox()
    {
        attackHitbox.SetActive(false);
        isAttacking = false;
    }

    public IEnumerator Stun(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    private void TryStartDrag()
    {
        // Najprv n·jdi najbliûöÌ collider v okruhu
        Collider2D hit = Physics2D.OverlapCircle(transform.position, dragRange, dragableLayer);
        if (hit == null) return;

        Dragable dr = hit.GetComponent<Dragable>();
        if (dr != null && dr.IsInRange(transform.position))
        {
            // ZistÌme skutoËn˝ bod dotyku na kolÌderi
            Vector2 worldAnchor = hit.ClosestPoint(transform.position);

            // A poöleme ho do StartDrag
            dr.StartDrag(rb, worldAnchor);

            currentDrag = dr;
            isDragging = true;
        }
    }
    private void EndDrag()
    {
        if (currentDrag != null)
        {
            currentDrag.EndDrag();
            currentDrag = null;
        }
        isDragging = false;
    }

    private void HandleMovementInputsOnly()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    private void FaceDragable()
    {
        if (currentDrag == null) return;
        bool objectOnRight = currentDrag.transform.position.x > transform.position.x;
        // ak je objekt napravo a ja sa nepozer·m doprava, otoË ma
        if (objectOnRight && !isFacingRight()) Flip();
        // ak je objekt naæavo a ja sa pozer·m doprava, otoË ma
        else if (!objectOnRight && isFacingRight()) Flip();
    }

}
