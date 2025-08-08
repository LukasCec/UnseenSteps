using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DisappearingPlatform : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Ko�ko sek�nd po dopade hr��a platforma zmizne.")]
    public float collapseDelay = 0.35f;

    [Tooltip("Za ko�ko sek�nd sa platforma objav� sp�.")]
    public float respawnTime = 4f;

    [Header("FX")]
    [Tooltip("Prefab particle syst�mu pre rozbitie (spawn pri zmiznut�).")]
    public GameObject breakVfxPrefab;

    [Header("Setup")]
    [Tooltip("Child objekt so SpriteRendererom (zap�name/vyp�name). Ak nech� pr�zdne, n�jde sa automaticky prv� SpriteRenderer v de�och.")]
    public GameObject spriteChild;

    [Tooltip("Tag objektu hr��a.")]
    public string playerTag = "Player";

    private BoxCollider2D col;
    private bool isCycling;      
    private bool armed;         

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        if (spriteChild == null)
        {
           
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) spriteChild = sr.gameObject;
        }
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(playerTag)) return;
        if (isCycling || armed) return;

       
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f) 
            {
                StartCoroutine(ArmAndCollapse());
                break;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        
        if (!collision.collider.CompareTag(playerTag)) return;
        if (isCycling || armed) return;

        
        if (collision.transform.position.y > transform.position.y + 0.05f)
        {
            StartCoroutine(ArmAndCollapse());
        }
    }

    private IEnumerator ArmAndCollapse()
    {
        armed = true;
        yield return new WaitForSeconds(collapseDelay);

       
        Collapse();

       
        yield return new WaitForSeconds(respawnTime);
        Respawn();

        armed = false;
    }

    private void Collapse()
    {
        if (isCycling) return;
        isCycling = true;

        
        if (breakVfxPrefab != null)
            Instantiate(breakVfxPrefab, transform.position, Quaternion.identity);

        
        col.enabled = false;
        if (spriteChild != null) spriteChild.SetActive(false);
    }

    private void Respawn()
    {
        
        col.enabled = true;
        if (spriteChild != null) spriteChild.SetActive(true);

        isCycling = false;
    }

    
    public void ForceCollapseNow()
    {
        StopAllCoroutines();
        armed = true;
        Collapse();
        StartCoroutine(RespawnLater());
    }

    private IEnumerator RespawnLater()
    {
        yield return new WaitForSeconds(respawnTime);
        Respawn();
        armed = false;
    }
}
