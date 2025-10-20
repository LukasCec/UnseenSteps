using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyWalk))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBomber : MonoBehaviour
{
    [Header("Detection / Chase")]
    public float detectionRange = 6f;      // ke� hr��a �zac�ti�
    public float explodeRange = 1.25f;     // ke� je takto bl�zko, spust� fuse
    public bool requireLineOfSight = false; // ak chce� ako Shooter (stena blokuje)

    [Header("Movement")]
    public float patrolSpeed = 1.4f;
    public float chaseSpeed = 2.4f;
    public float flipCooldown = 0.35f;

    [Header("Explosion")]
    public int explosionDamage = 2;
    public float explosionRadius = 1.75f;
    public float explosionForce = 12f;
    public float armingDelay = 0.1f;  // mal� poistka po za�iatku fuse
    public float fuseTime = 0.8f;     // ako dlho �sy�� pred v�buchom
    public GameObject explosionVfx;   // volite�n� VFX
    public string explodeSfx = "enemyExplode"; // k��� pre AudioManager

    private EnemyWalk enemyWalk;
    private EnemyHealth enemyHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private float lastFlipTime = 0f;
    private bool isFusing = false;
    private bool isExploding = false;

    void Awake()
    {
        enemyWalk = GetComponent<EnemyWalk>();
        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // �tartujeme na patrol r�chlosti
        enemyWalk.moveSpeed = patrolSpeed;
    }

    void FixedUpdate()
    {
        if (player == null || isExploding) return;

        // po�as fuse u� ni� nerob�me (stoj�me a �ak�me na v�buch)
        if (isFusing)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange && (!requireLineOfSight || HasLineOfSight()))
        {
            // chase m�d: zvy� r�chlos� a pozeraj sa na hr��a
            enemyWalk.enabled = true;
            enemyWalk.moveSpeed = chaseSpeed;
            animator.SetBool(EnemyBomberAnimationStrings.IsMoving, true);

            LookAtPlayer();

            // sme v dosahu na v�buch? spusti fuse
            if (dist <= explodeRange)
            {
                StartFuse();
            }
        }
        else
        {
            // be�n� patrola
            enemyWalk.enabled = true;
            enemyWalk.moveSpeed = patrolSpeed;
            animator.SetBool(EnemyBomberAnimationStrings.IsMoving, true);
        }
    }

    bool HasLineOfSight()
    {
        // rovnak� logika ako u Shootra � reusni layer z EnemyWalk
        // (ray zhruba z poz�cie trupu smerom k hr��ovi)
        Vector2 origin = transform.position;
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(origin, player.position);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, enemyWalk.groundLayer);
        return hit.collider == null;
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        float xDiff = player.position.x - transform.position.x;
        if (Mathf.Abs(xDiff) < 0.15f) return;
        if (Time.time - lastFlipTime < flipCooldown) return;

        bool playerOnRight = (xDiff > 0);
        bool isFacingRight = enemyWalk.isFacingRight;

        if ((playerOnRight && !isFacingRight) || (!playerOnRight && isFacingRight))
        {
            enemyWalk.Flip();
            lastFlipTime = Time.time;
        }
    }

    void StartFuse()
    {
        if (isFusing || isExploding) return;

        isFusing = true;

        // zastav�me pohyb
        enemyWalk.enabled = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool(EnemyBomberAnimationStrings.IsMoving, false);
        animator.SetTrigger(EnemyBomberAnimationStrings.Fuse);

        StartCoroutine(FuseRoutine());
    }

    IEnumerator FuseRoutine()
    {
        // mal� poistka � keby sa spustil fuse v tom istom frame ako kol�zia
        yield return new WaitForSeconds(armingDelay);

        // "z�palnica" �as
        yield return new WaitForSeconds(fuseTime);

        Explode();
    }

    void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        if (AudioManager.Instance != null && !string.IsNullOrEmpty(explodeSfx))
            AudioManager.Instance.PlaySFX(explodeSfx);

        animator.SetTrigger(EnemyBomberAnimationStrings.Explode);

        if (explosionVfx != null)
            Instantiate(explosionVfx, transform.position, Quaternion.identity);

        // Damage v r�diuse
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var col in hits)
        {
            if (col.gameObject == gameObject) continue;

            // DMG cez IDamageable (tvoj PlayerHealth aj EnemyHealth to pravdepodobne implementuj�)
            var dmg = col.GetComponent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(explosionDamage);

            // Knockback ak m� rigidbody
            var body = col.attachedRigidbody;
            if (body != null)
            {
                Vector2 dir = (col.transform.position - transform.position).normalized;
                body.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }
        }

        // zma�eme sa (ak chce� dopozera� expl�ziu anim�cie, m��e� prida� kr�tke oneskorenie)
        Destroy(gameObject, 0.02f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explodeRange);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

public static class EnemyBomberAnimationStrings
{
    public const string IsMoving = "isMoving";
    public const string Fuse = "fuse";
    public const string Explode = "explode";
}
