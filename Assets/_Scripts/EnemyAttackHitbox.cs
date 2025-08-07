using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 1;
    [Tooltip("Filtrovanie na tag cie¾a")]
    public string targetTag = "Player";

    // len jeden zásah na jedno zapnutie hitboxu
    private bool canDealDamage;

    void OnEnable()
    {
        canDealDamage = true;
        // Debug:
         Debug.Log($"{name} hitbox ENABLED");
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    void OnTriggerStay2D(Collider2D other) => TryHit(other);

    private void TryHit(Collider2D other)
    {
        if (!canDealDamage) return;
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag)) return;

        // nájdi PlayerHealth kdeko¾vek na hráèovi (self/parent/child)
        var ph = other.GetComponent<PlayerHealth>()
                 ?? other.GetComponentInParent<PlayerHealth>()
                 ?? other.GetComponentInChildren<PlayerHealth>();

        if (ph != null)
        {
            Debug.Log("Hraca trafil");
            ph.TakeDamage(damage, transform.position);
            canDealDamage = false; // hit iba raz poèas aktívneho okna
        }
    }
}
