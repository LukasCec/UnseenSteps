using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Chest Settings")]
    public GameObject coinPrefab;           
    public int coinCount = 5;               
    public Transform spawnPoint;           
    public Vector2 dropForceMin = new Vector2(-2f, 5f);
    public Vector2 dropForceMax = new Vector2(2f, 7f);

    [Header("Item Drop (optional)")]
    public GameObject itemPrefab;
    public bool dropItem = false;

    [Header("UI Tooltip")]
    public GameObject tooltipUI; 

    [Header("Animation")]
    public float openDelay = 0.6f; 
    private Animator animator;
    private bool playerInRange = false;
    private bool isOpened = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (tooltipUI != null)
            tooltipUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !isOpened)
        {
            if (tooltipUI != null && !tooltipUI.activeSelf)
                tooltipUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(OpenAndDrop());
            }
        }
        else
        {
            if (tooltipUI != null && tooltipUI.activeSelf)
                tooltipUI.SetActive(false);
        }
    }

    private System.Collections.IEnumerator OpenAndDrop()
    {
        isOpened = true;
        animator.SetTrigger("OpenChest");

        yield return new WaitForSeconds(openDelay); 

        SpawnLoot();

        if (tooltipUI != null)
            tooltipUI.SetActive(false);
    }

    private void SpawnLoot()
    {
        
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                float forceX = Random.Range(dropForceMin.x, dropForceMax.x);
                float forceY = Random.Range(dropForceMin.y, dropForceMax.y);
                rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
            }
        }

        
        if (dropItem && itemPrefab != null)
        {
            Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }
}
