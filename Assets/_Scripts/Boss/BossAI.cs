using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class BossAI : MonoBehaviour
{
    [Header("Movement / Targeting")]
    public float moveSpeed = 2f;
    public float detectionRange = 6f;
    public float attackRange = 1.6f;
    public float attackCooldown = 2f;
    public Transform attackOrigin;

    [Header("Refs")]
    public BossAttackHitbox meleeHitbox;

    [Header("Ground/Wall Check")]
    public LayerMask groundLayer;
    [Tooltip("Ako ïaleko pred seba pozrie na hranu (vodorovne)")]
    public float edgeLookAhead = 0.5f;
    [Tooltip("Ako hlboko dolu h¾adá zem z bodu pred sebou")]
    public float edgeRayDown = 1.0f;
    [Tooltip("Kontrola steny pred sebou")]
    public float wallCheckDistance = 0.2f;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private BossHealth health;
    private BoxCollider2D box;
    private bool canAttack = true;
    private bool facingRight = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<BossHealth>();
        box = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        if (health != null && health.IsDead) { rb.linearVelocity = Vector2.zero; return; }
        if (player == null) { animator.SetBool("IsMoving", false); rb.linearVelocity = Vector2.zero; return; }

        float dist = Vector2.Distance(attackOrigin.position, player.position);

        if (dist <= attackRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("IsMoving", false);
            if (canAttack) StartCoroutine(AttackRoutine());
            return;
        }

        if (dist <= detectionRange)
        {
            float dirX = Mathf.Sign(player.position.x - transform.position.x);

            if (IsWallAhead(dirX) || IsLedgeAhead(dirX) == false)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                animator.SetBool("IsMoving", false);
            }
            else
            {
                rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);
                animator.SetBool("IsMoving", Mathf.Abs(rb.linearVelocity.x) > 0.01f);
            }

            if (dirX > 0 && !facingRight) Flip();
            else if (dirX < 0 && facingRight) Flip();
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("IsMoving", false);
        }
    }

    bool IsWallAhead(float dirX)
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(dirX * (box.bounds.extents.x + 0.02f), 0f);
        RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(dirX, 0f), wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    bool IsLedgeAhead(float dirX)
    {
        Vector2 front = (Vector2)box.bounds.center + new Vector2(dirX * (box.bounds.extents.x + edgeLookAhead), -box.bounds.extents.y * 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(front, Vector2.down, edgeRayDown, groundLayer);
        return hit.collider != null;
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        AudioManager.Instance?.PlaySFX("bossSwing");
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void Flip()
    {
        facingRight = !facingRight;
        var s = transform.localScale; s.x *= -1; transform.localScale = s;
    }

    public void Anim_OpenHitbox() => meleeHitbox?.BeginWindow();
    public void Anim_CloseHitbox() => meleeHitbox?.EndWindow();
}
