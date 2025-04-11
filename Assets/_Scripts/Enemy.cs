using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public float moveSpeed = 2f;
    public float detectionRange = 1.5f;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    [Header("Checkers")]
    public Transform groundCheck;
    public Transform wallCheck;
    public GameObject attackHitbox;

    [Header("Attack")]
    public float attackCooldown = 1f;
    private bool isAttacking;
    private bool canAttack = true;

    private bool isGrounded;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        attackHitbox.SetActive(false); // ensure it's off at start
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        CheckGround();
        CheckWall();
        DetectPlayer();

        if (!isAttacking)
            Patrol();
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isMoving", true);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, groundLayer);
        if (!isGrounded)
            Flip();
    }

    void CheckWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, 0.5f, groundLayer);
        if (hit.collider != null)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        canAttack = true;
    }

    //  Tieto metódy spúšajú/zastavujú hitbox - pripojené na animáciu
    public void EnableAttackHitbox()
    {
        // Reset aktivácie
        attackHitbox.SetActive(false);
        attackHitbox.SetActive(true);

        // ROVNO aplikuj damage na všetkých hráèov vo vnútri hitboxu
        BoxCollider2D col = attackHitbox.GetComponent<BoxCollider2D>();
        Vector2 hitboxPos = (Vector2)attackHitbox.transform.position + col.offset;
        Vector2 hitboxSize = col.size;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
    hitboxPos,
    hitboxSize,
    0f
);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(1, transform.position);
                }
            }
        }
    }


    public void DisableAttackHitbox()
    {
        attackHitbox.SetActive(false);
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        StartCoroutine(BlinkEffect()); // zavolá blikanie

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }


    IEnumerator BlinkEffect()
    {
        int blinkCount = 3;
        float blinkDuration = 0.1f;

        for (int i = 0; i < blinkCount; i++)
        {
            sr.color = new Color(1, 1, 1, 0.2f); // prieh¾adný
            yield return new WaitForSeconds(blinkDuration);
            sr.color = Color.white; // spä na normál
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    // Editor Debug Gizmos
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.2f);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 dir = isFacingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * 0.1f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (attackHitbox != null)
        {
            Gizmos.color = Color.magenta;
            BoxCollider2D col = attackHitbox.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                Gizmos.matrix = attackHitbox.transform.localToWorldMatrix;
                Gizmos.DrawWireCube(col.offset, col.size);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}
