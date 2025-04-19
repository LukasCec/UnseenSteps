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
        // Ak tam e�te nem�me cooldown z�znam, inicializuj ho
        if (!lastHitTime.ContainsKey(other.gameObject))
            lastHitTime[other.gameObject] = -damageCooldown;

        // M�me cooldown pre t�to kol�ziu?
        if (Time.time - lastHitTime[other.gameObject] < damageCooldown)
            return;

        // 1) Najprv sk�s rozbitn� objekt
        Breakable br = other.GetComponent<Breakable>();
        if (br != null)
        {
            br.Hit();
            lastHitTime[other.gameObject] = Time.time;
            return;
        }

        // 2) Potom nepriate�a
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth eh = other.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
                lastHitTime[other.gameObject] = Time.time;
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
