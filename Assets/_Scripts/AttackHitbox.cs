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
        // Ak tam ešte nemáme cooldown záznam, inicializuj ho
        if (!lastHitTime.ContainsKey(other.gameObject))
            lastHitTime[other.gameObject] = -damageCooldown;

        // Máme cooldown pre túto kolíziu?
        if (Time.time - lastHitTime[other.gameObject] < damageCooldown)
            return;

        // 1) Najprv skús rozbitný objekt
        Breakable br = other.GetComponent<Breakable>();
        if (br != null)
        {
            br.Hit();
            lastHitTime[other.gameObject] = Time.time;
            return;
        }

        // 2) Potom nepriate¾a
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
