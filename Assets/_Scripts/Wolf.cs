using UnityEngine;

[RequireComponent(typeof(EnemyWalk))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public class Wolf : MonoBehaviour
{
    [Header("Attack Hitbox (child with BoxCollider2D)")]
    [Tooltip("Sem ho� odkaz na collider, ktor� m� by� zap�nan� po�as �toku.")]
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

        // istota: hitbox je defaultne vypnut�
        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    void FixedUpdate()
    {
        // Po�as �toku zastav�me horizont�lny pohyb a vypneme ch�dzu
        if (isAttacking)
        {
            if (enemyWalk.enabled) enemyWalk.enabled = false;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("isMoving", false);
            return;
        }

        // mimo �toku nech sa spr�va norm�lne (patroluje cez EnemyWalk)
        if (!enemyWalk.enabled) enemyWalk.enabled = true;

        // jednoduch� prep�na� idle/run pod�a r�chlosti
        bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        animator.SetBool("isMoving", moving);
    }

    /// <summary>
    /// Zavolaj z Animation Event v �to�nej anim�cii na frame, ke� m� hitbox za�a� bra�.
    /// </summary>
    public void EnableHitbox()
    {
        isAttacking = true;

        if (hitboxCollider != null)
        {
            // ak m� na sebe EnemyAttackHitbox, otvor damage okno
            var dealer = hitboxCollider.GetComponent<EnemyAttackHitbox>();
            if (dealer != null) dealer.BeginWindow();

            hitboxCollider.enabled = true;
            //Debug.Log("[Wolf] Hitbox ENABLED");
        }
    }

    /// <summary>
    /// Zavolaj z Animation Event v �to�nej anim�cii, ke� m� hitbox presta� bra�.
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
