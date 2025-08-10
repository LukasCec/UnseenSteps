using UnityEngine;

public class SpikesTrap : MonoBehaviour
{
    public GameObject hitboxObject; // Objekt s trigger colliderom
    public int damage = 1;

    void Start()
    {
        hitboxObject.SetActive(false); // ZabezpeËiù, ûe zaËÌna vypnutÈ
    }

    public void EnableHitbox()
    {
        if (IsVisibleToCamera(Camera.main))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("spike");
        }

        hitboxObject.SetActive(true);

        // Manu·lne udeliù damage hr·Ëovi, ak uû v tom momente stojÌ v hitboxe
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
    private bool IsVisibleToCamera(Camera cam)
    {
        if (cam == null) return false;
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        return viewportPos.z > 0 &&
               viewportPos.x > 0 && viewportPos.x < 1 &&
               viewportPos.y > 0 && viewportPos.y < 1;
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
