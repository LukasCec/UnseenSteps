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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
        canAttack = true;
    }

    public void EnableAttackHitbox()
    {
        attackHitbox.SetActive(true);
    }

    public void DisableAttackHitbox()
    {
        attackHitbox.SetActive(false);
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }



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

        // Player detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Optional: attack hitbox outline
        if (attackHitbox != null)
        {
            Gizmos.color = Color.magenta;
            BoxCollider2D col = attackHitbox.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                Vector3 pos = col.transform.position + (Vector3)col.offset;
                Vector3 size = col.size;
                Gizmos.matrix = col.transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero + (Vector3)col.offset, size);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}
