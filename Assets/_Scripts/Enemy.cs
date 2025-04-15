using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyWalk))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Attack Settings")]
    public float detectionRange = 1.5f;
    public float attackCooldown = 1f;
    public GameObject attackHitbox;

    private bool isAttacking;
    private bool canAttack = true;

    // Komponenty
    private EnemyHealth enemyHealth;  // star� sa o HP
    private EnemyWalk enemyWalk;      // star� sa o pohyb + flip okraj/stena
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    void Awake()
    {
        // Na��tame pripojen� komponenty
        enemyHealth = GetComponent<EnemyHealth>();
        enemyWalk = GetComponent<EnemyWalk>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (attackHitbox != null)
            attackHitbox.SetActive(false); // Prevent�vne vypneme
    }

    void FixedUpdate()
    {
        // Ak zrovna �to��me, m��eme zablokova� movement
        // (alebo ponecha�, ak chcete, aby nepriate� mohol chodi� aj po�as anim�cie �toku)
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            enemyWalk.enabled = false;
        }
        else
        {
            enemyWalk.enabled = true;
        }

        DetectPlayer();
    }

    /// <summary> Kontrola, �i je hr�� v dosahu na �tok, a pr�padn� spustenie </summary>
    void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange && canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    /// <summary> Coroutine na spracovanie �toku </summary>
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canAttack = false;

        // Spusti anim�ciu �toku
        if (animator != null)
            animator.SetTrigger("isAttacking");

        // Po�k�me cooldown
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        canAttack = true;
    }

    //  Tieto met�dy sp���aj�/zastavuj� hitbox - pripojen� na anim�ciu (Animation Event)
    public void EnableAttackHitbox()
    {
        if (attackHitbox == null) return;
        attackHitbox.SetActive(false);
        attackHitbox.SetActive(true);

        // M��ete tu priamo rie�i� damage hr��ovi (OverlapBox...), 
        // alebo to necha� na skript v AttackHitbox
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox == null) return;
        attackHitbox.SetActive(false);
    }

    // --- Pomocn� debug vizu�ly ---
    void OnDrawGizmosSelected()
    {
        // �erven� guli�ka okolo nepriate�a nazna�uj�ca detectionRange
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
