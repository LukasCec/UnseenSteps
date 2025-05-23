using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyWalk))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Shooter Settings")]
    public float detectionRange = 6f;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float attackCooldown = 1.5f;

    private bool canAttack = true;

    // Komponenty
    private EnemyHealth enemyHealth;  // star� sa o HP
    private EnemyWalk enemyWalk;      // star� sa o pohyb + flip okraj/stena
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    [Header("Flip Settings")]
    public float flipCooldown = 0.5f;
    private float lastFlipTime = 0f;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyWalk = GetComponent<EnemyWalk>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            enemyWalk.enabled = true;
            return;
        }

        // ak je hrac v dosahu + viditelny
        if (PlayerInRange() && HasLineOfSight())
        {
            // zastav a strielaj
            enemyWalk.enabled = false;
            rb.linearVelocity = Vector2.zero;

            // Uistime sa, ze animacia nebude "walking"
            animator.SetBool(EnemyShooterAnimationStrings.IsMoving, false);

            LookAtPlayer();
            StopAndShoot();
        }
        else
        {
            // hrac mimo dosahu -> patrola
            enemyWalk.enabled = true;
            // nastavit isMoving = true, nech hr� "walk" anim�ciu
            animator.SetBool(EnemyShooterAnimationStrings.IsMoving, true);
        }
    }

    /// <summary> Skontroluje, �i je hr�� v detectionRange </summary>
    bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    /// <summary> Skontroluje, �i nie je medzi mnou a hr��om stena </summary>
    bool HasLineOfSight()
    {
        Vector2 origin = firePoint.position;
        Vector2 direction = (player.position - firePoint.position).normalized;
        float distance = Vector2.Distance(origin, player.position);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, enemyWalk.groundLayer);
        // Ak nie�o zablokuje cestu -> hr�� je za stenou
        return hit.collider == null;
    }

    /// <summary> Oto��me sa smerom k hr��ovi, ale len ak uplynul flipCooldown </summary>
    void LookAtPlayer()
    {
        float xDiff = player.position.x - transform.position.x;
        if (Mathf.Abs(xDiff) < 0.5f) return;

        if (Time.time - lastFlipTime < flipCooldown) return;

        bool playerOnRight = (xDiff > 0);
        bool isFacingRight = enemyWalk.isFacingRight;  // pre��tame si aktu�lny stav z EnemyWalk

        // Ak je hr�� v opa�nom smere, flipni
        if (playerOnRight && !isFacingRight || !playerOnRight && isFacingRight)
        {
            enemyWalk.Flip();
            lastFlipTime = Time.time;
        }
    }

    /// <summary>Zastav�me a za�neme strie�a�, pokia� m��eme</summary>
    void StopAndShoot()
    {
        if (canAttack)
            StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        canAttack = false;

        // Spusti anim�ciu
        animator.SetTrigger(EnemyShooterAnimationStrings.IsAttacking);

      
       
        ShootProjectile();

        // cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    /// <summary> Vytvor� strelu smerom k hr��ovi </summary>
    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        Vector2 dir = (player.position - firePoint.position).normalized;
        rbBullet.linearVelocity = dir * 10f; // r�chlos� strely
    }

    // --- Pomocn� debug vizu�ly ---
    void OnDrawGizmosSelected()
    {
        // �erven� guli�ka okolo nepriate�a nazna�uj�ca detectionRange
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Ke� je vybrat� firePoint, nakresl�me �iaru smerom k hr��ovi
        if (firePoint != null && player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(firePoint.position, player.position);
        }
    }
}
