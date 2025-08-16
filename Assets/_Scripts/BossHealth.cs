using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 50;
    public int currentHealth;
    public bool IsDead { get; private set; }

    private Animator animator;
    private Collider2D[] colliders;

    void Awake()
    {
        animator = GetComponent<Animator>();
        colliders = GetComponentsInChildren<Collider2D>(true);
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        if (IsDead) return;
        currentHealth -= Mathf.Max(1, dmg);
        animator.SetTrigger("Hurt");
        AudioManager.Instance?.PlaySFX("bossHurt");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        animator.SetTrigger("Death");
        AudioManager.Instance?.PlaySFX("bossDeath");

        // vypni kolÌzie a pohyb
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        foreach (var c in colliders) c.enabled = false;

        // nechaj dohraù anim·ciu a zmaû
        Destroy(gameObject, 3f);
    }
}
