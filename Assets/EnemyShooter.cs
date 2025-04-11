using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public float moveSpeed = 2f;
    public float detectionRange = 6f;
    public LayerMask groundLayer;

    [Header("Checkers")]
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    private bool canAttack = true;

    [Header("Flip Delay")]
    public float flipCooldown = 0.5f;
    private float lastFlipTime;

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

        if (PlayerInRange())
        {
            LookAtPlayer();
            StopAndShoot();
        }
        else
        {
            Patrol();
        }
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        float xDiff = player.position.x - transform.position.x;
        if (Mathf.Abs(xDiff) < 0.5f) return;
        if (Time.time - lastFlipTime < flipCooldown) return;

        bool playerOnRight = xDiff > 0;

        if (playerOnRight && !isFacingRight || !playerOnRight && isFacingRight)
        {
            Flip();
            lastFlipTime = Time.time;
        }
    }

    void Patrol()
    {
        CheckWall(); // flipuj len poËas pohybu

        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isMoving", true);
    }

    void StopAndShoot()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);

        if (canAttack)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        canAttack = false;
        animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(0.3f); // Ëakaj na moment v˝strelu (sync s anim·ciou)

        ShootProjectile();

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void ShootProjectile()
    {
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        Vector2 dir = (player.position - firePoint.position).normalized;
        rbBullet.linearVelocity = dir * 10f;
    }

    void CheckGround()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, groundLayer);

        // ak enemy pad· a nie je hr·Ë v dosahu, mÙûe flipn˙ù (napr. na hrane)
        if (!isGrounded && !PlayerInRange())
        {
            Flip();
        }
    }

    void CheckWall()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, direction, 0.5f, groundLayer);
        if (hit.collider != null)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        StartCoroutine(BlinkEffect());

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator BlinkEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int blinkCount = 3;
        float blinkDuration = 0.1f;

        for (int i = 0; i < blinkCount; i++)
        {
            sr.color = new Color(1, 1, 1, 0.2f);
            yield return new WaitForSeconds(blinkDuration);
            sr.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

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

        if (firePoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(firePoint.position, firePoint.position + Vector3.right * (isFacingRight ? 0.5f : -0.5f));
        }
    }
}
