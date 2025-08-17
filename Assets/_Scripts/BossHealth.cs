using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 50;
    public int currentHealth;
    public bool IsDead { get; private set; }

    [Header("Unlock UI Settings")]
    [Tooltip("ZobrazÌ sa po tom, Ëo boss zmizne (musÌ sedieù s Destroy delayom).")]
    public float overlayDelayAfterDeath = 3f; // zodpoved· Destroy(..., 3f)
    public string unlockedSkillName = "Dash Unlocked";
    public Sprite unlockedSkillIcon;

    [Header("Optional: Zapnutie schopnosti")]
    public PlayerAbilitiesData abilitiesData;
    public enum AbilityToUnlock { None, Dash, DoubleJump, WallSlide, WallJump }
    public AbilityToUnlock unlockAbility = AbilityToUnlock.Dash;

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

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        foreach (var c in colliders) c.enabled = false;

        // (1) Napl·nuj zobrazenie overlayu po zmiznutÌ bossa
        SkillUnlockOverlay.Instance?.Show(unlockedSkillName, unlockedSkillIcon, overlayDelayAfterDeath);

        // (2) Voliteæne ñ hneÔ odomkni schopnosù hr·Ëovi
        ApplyAbilityUnlock();

        // (3) Zmaû bossa po 3 sekund·ch (musÌ sedieù s overlayDelayAfterDeath)
        Destroy(gameObject, overlayDelayAfterDeath);
    }

    private void ApplyAbilityUnlock()
    {
        if (abilitiesData == null) return;

        switch (unlockAbility)
        {
            case AbilityToUnlock.Dash: abilitiesData.canDash = true; break;
            case AbilityToUnlock.DoubleJump: abilitiesData.canDoubleJump = true; break;
            case AbilityToUnlock.WallSlide: abilitiesData.canWallSlide = true; break;
            case AbilityToUnlock.WallJump: abilitiesData.canWallJump = true; break;
            case AbilityToUnlock.None: default: break;
        }
    }
}
