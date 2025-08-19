using UnityEngine;

[RequireComponent(typeof(EnemyWalk))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public class Wolf : MonoBehaviour
{
    [Header("Attack Hitbox (child with BoxCollider2D)")]
    [Tooltip("Sem hoï odkaz na collider, ktorı má by zapínanı poèas útoku.")]
    public BoxCollider2D hitboxCollider;

    private Animator animator;
    private Rigidbody2D rb;
    private EnemyWalk enemyWalk;

    private bool isAttacking = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyWalk = GetComponent<EnemyWalk>();

        // istota: hitbox je defaultne vypnutı
        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    void FixedUpdate()
    {
        // Poèas útoku zastavíme horizontálny pohyb a vypneme chôdzu
        if (isAttacking)
        {
            if (enemyWalk.enabled) enemyWalk.enabled = false;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("isMoving", false);
            return;
        }

        // mimo útoku nech sa správa normálne (patroluje cez EnemyWalk)
        if (!enemyWalk.enabled) enemyWalk.enabled = true;

        // jednoduchı prepínaè idle/run pod¾a rıchlosti
        bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        animator.SetBool("isMoving", moving);
    }

    /// <summary>
    /// Zavolaj z Animation Event v útoènej animácii na frame, keï má hitbox zaèa bra.
    /// </summary>
    public void EnableHitbox()
    {
        isAttacking = true;

        if (hitboxCollider != null)
        {
            // ak má na sebe EnemyAttackHitbox, otvor damage okno
            var dealer = hitboxCollider.GetComponent<EnemyAttackHitbox>();
            if (dealer != null) dealer.BeginWindow();

            hitboxCollider.enabled = true;
            //Debug.Log("[Wolf] Hitbox ENABLED");
        }
    }

    /// <summary>
    /// Zavolaj z Animation Event v útoènej animácii, keï má hitbox presta bra.
    /// </summary>
    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            var dealer = hitboxCollider.GetComponent<EnemyAttackHitbox>();
            if (dealer != null) dealer.EndWindow();

            hitboxCollider.enabled = false;
            //Debug.Log("[Wolf] Hitbox DISABLED");
        }

        isAttacking = false;
    }
}
