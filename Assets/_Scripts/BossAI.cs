using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
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

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private BossHealth health;
    private bool canAttack = true;
    private bool facingRight = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<BossHealth>();
    }

    void FixedUpdate()
    {
        if (health != null && health.IsDead) { rb.linearVelocity = Vector2.zero; return; }
        if (player == null) { animator.SetBool("IsMoving", false); rb.linearVelocity = Vector2.zero; return; }

        float dist = Vector2.Distance(attackOrigin.position, player.position);

        if (dist <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);

            if (canAttack) StartCoroutine(AttackRoutine());
        }
        else if (dist <= detectionRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
            animator.SetBool("IsMoving", Mathf.Abs(rb.linearVelocity.x) > 0.01f);

            if (dir.x > 0 && !facingRight) Flip();
            else if (dir.x < 0 && facingRight) Flip();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
        }
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
        Vector3 s = transform.localScale; s.x *= -1; transform.localScale = s;
    }

    // ---- Animation Events volajú tieto bridge metódy na ROOTe ----
    public void Anim_OpenHitbox() => meleeHitbox?.BeginWindow();
    public void Anim_CloseHitbox() => meleeHitbox?.EndWindow();

    // Debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(attackOrigin.position, attackRange);
    }
}
