using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 1;
    public string targetTag = "Player";

    private bool canDealDamage;

    public void BeginWindow()
    {
        canDealDamage = true;
        Debug.Log("Hitbox window begin");
    }

    public void EndWindow()
    {
        canDealDamage = false;
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
            canDealDamage = false;
        }
    }
}
