using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 1;
    // optional: leave empty to not rely on tags
    public string targetTag = "Player";

    private bool canDealDamage;

    void OnEnable()
    {
        canDealDamage = true; // one hit per activation window
        Debug.Log("Hitbox ENABLED");
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    void OnTriggerStay2D(Collider2D other) => TryHit(other);

    private void TryHit(Collider2D other)
    {
        if (!canDealDamage) return;

        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        // find PlayerHealth anywhere on the contacted object
        var ph = other.GetComponent<PlayerHealth>()
                 ?? other.GetComponentInParent<PlayerHealth>()
                 ?? other.GetComponentInChildren<PlayerHealth>();

        if (ph != null)
        {
            Debug.Log("Hraca trafil");
            ph.TakeDamage(damage, transform.position);
            canDealDamage = false;
        }
    }
}
