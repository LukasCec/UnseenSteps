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
        if (other.CompareTag("Enemy"))
        {
            if (!lastHitTime.ContainsKey(other.gameObject))
                lastHitTime[other.gameObject] = -damageCooldown;

            if (Time.time - lastHitTime[other.gameObject] >= damageCooldown)
            {
                // Namiesto 'Enemy enemy = ...' použijeme 'EnemyHealth eh = ...'
                EnemyHealth eh = other.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    eh.TakeDamage(damage);
                    lastHitTime[other.gameObject] = Time.time;
                }
            }
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
