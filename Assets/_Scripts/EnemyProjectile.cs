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
        if (other.CompareTag("Enemy")) return; // Ignoruj iných enemy

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position);
            }
        }

        if (!other.isTrigger) // zniè sa na akejko¾vek kolízii
            Destroy(gameObject);
    }
}
