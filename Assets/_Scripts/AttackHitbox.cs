using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 0.5f;

    private Dictionary<GameObject, float> lastHitTime = new Dictionary<GameObject, float>();

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    void TryDamage(Collider2D other)
    {
        // Initialize cooldown entry if not present
        if (!lastHitTime.ContainsKey(other.gameObject))
            lastHitTime[other.gameObject] = -damageCooldown;

        // Respect cooldown for this collider
        if (Time.time - lastHitTime[other.gameObject] < damageCooldown)
            return;

        // Damage any object implementing IDamageable
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            lastHitTime[other.gameObject] = Time.time;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (lastHitTime.ContainsKey(other.gameObject))
            lastHitTime.Remove(other.gameObject);
    }

    void OnDisable()
    {
        lastHitTime.Clear();
    }
}
