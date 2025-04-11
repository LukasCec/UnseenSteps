using UnityEngine;

public class SpikesTrap : MonoBehaviour
{
    public GameObject hitboxObject; // Objekt s trigger colliderom
    public int damage = 1;

    void Start()
    {
        hitboxObject.SetActive(false); // Zabezpe�i�, �e za��na vypnut�
    }

    public void EnableHitbox()
    {
        hitboxObject.SetActive(true);

        // Manu�lne udeli� damage hr��ovi, ak u� v tom momente stoj� v hitboxe
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            hitboxObject.transform.position,
            hitboxObject.GetComponent<BoxCollider2D>().size,
            0f
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var health = hit.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage, transform.position);
                }
            }
        }
    }

    public void DisableHitbox()
    {
        hitboxObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (hitboxObject != null)
        {
            Gizmos.color = Color.red;
            BoxCollider2D box = hitboxObject.GetComponent<BoxCollider2D>();
            Gizmos.DrawWireCube(hitboxObject.transform.position + (Vector3)box.offset, box.size);
        }
    }
}
