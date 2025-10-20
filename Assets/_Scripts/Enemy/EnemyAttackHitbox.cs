using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 1;
    public string targetTag = "Player";

    private bool canDealDamage;
    private bool hasHit;

    // Eventy pre Enemy
    public event Action OnSuccessfulHit;
    public event Action OnMiss;

    public void BeginWindow()
    {
        canDealDamage = true;
        hasHit = false; // reset pred každým oknom útoku
        Debug.Log("Hitbox window begin");
    }

    public void EndWindow()
    {
        canDealDamage = false;

        // ak poèas okna neprebehol zásah -> MISS
        if (!hasHit)
        {
            OnMiss?.Invoke();
            Debug.Log("Attack MISS");
        }

        Debug.Log("Hitbox window end");
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    void OnTriggerStay2D(Collider2D other) => TryHit(other);

    private void TryHit(Collider2D other)
    {
        if (!canDealDamage) return;
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag)) return;

        var ph = other.GetComponent<PlayerHealth>()
                 ?? other.GetComponentInParent<PlayerHealth>()
                 ?? other.GetComponentInChildren<PlayerHealth>();

        if (ph != null)
        {
            ph.TakeDamage(damage, transform.position);
            hasHit = true;          // oznaèíme, že útok trafil
            canDealDamage = false;  // len jeden zásah v rámci okna
            OnSuccessfulHit?.Invoke();
            Debug.Log("Attack HIT");
        }
    }
}
