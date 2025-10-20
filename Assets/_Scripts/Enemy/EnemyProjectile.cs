using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) return; // Ignoruj in�ch enemy

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position);
            }
        }

        if (!other.isTrigger) // zni� sa na akejko�vek kol�zii
            Destroy(gameObject);
    }
}
