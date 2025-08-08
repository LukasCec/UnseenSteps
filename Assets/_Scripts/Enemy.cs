// Enemy.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyWalk))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Attack Settings")]
    public float detectionRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Hitbox")]
    public BoxCollider2D hitboxCollider;

    private bool isAttacking = false;
    private bool canAttack = true;

    private Animator animator;
    private Rigidbody2D rb;
    private EnemyWalk enemyWalk;
    private Transform player;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyWalk = GetComponent<EnemyWalk>();
        player = GameObject.FindWithTag("Player")?.transform;

        
        hitboxCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            enemyWalk.enabled = false;
        }
        else
        {
           
            enemyWalk.enabled = true;
            bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
            animator.SetBool("isMoving", moving);
        }

        TryDetectAndAttack();
    }

    private void TryDetectAndAttack()
    {
        if (!canAttack || player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canAttack = false;
        enemyWalk.enabled = false;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        canAttack = true;
    }


    public void EnableHitbox()
    {
        if (hitboxCollider != null)
        {
            
            var dealer = hitboxCollider.GetComponent<EnemyAttackHitbox>();
            if (dealer != null) dealer.BeginWindow();

            hitboxCollider.enabled = true;
            Debug.Log("Hitbox ENABLED");
        }
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            
            var dealer = hitboxCollider.GetComponent<EnemyAttackHitbox>();
            if (dealer != null) dealer.EndWindow();

            hitboxCollider.enabled = false;
            Debug.Log("Hitbox DISABLED");
        }
    }
}
