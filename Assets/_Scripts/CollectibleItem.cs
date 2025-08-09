using UnityEngine;
using TMPro;

public class CollectibleItem : MonoBehaviour
{
    public enum ItemType
    {
        HealPotion,
        RevealPotion,
        Coin
    }

    [Header("Item Settings")]
    public ItemType itemType;
    public int amount = 1;

    [Header("Inventory Reference")]
    public InventoryData inventoryData;

    [Header("UI References (TMP Texts)")]
    public TMP_Text healText;
    public TMP_Text revealText;
    public TMP_Text coinsText;

    [Header("Pickup Effect")]
    public GameObject pickupEffectPrefab;

    [Header("Floating Animation")]
    public float floatAmplitude = 0.25f; 
    public float floatFrequency = 1f;    
    private Vector3 startPos;
    private float phaseOffset; 

    private void Start()
    {
        startPos = transform.position;
        phaseOffset = Random.Range(0f, Mathf.PI * 2f); 
        UpdateUI();
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin((Time.time * floatFrequency) + phaseOffset) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        switch (itemType)
        {
            case ItemType.HealPotion:
                inventoryData.AddHealPotion(amount);
                break;

            case ItemType.RevealPotion:
                inventoryData.AddRevealPotion(amount);
                break;

            case ItemType.Coin:
                inventoryData.AddCoins(amount);
                break;
        }

        UpdateUI();

        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    void UpdateUI()
    {
        if (healText != null)
            healText.text = $"{inventoryData.healPotions}";

        if (revealText != null)
            revealText.text = $"{inventoryData.revealPotions}";

        if (coinsText != null)
            coinsText.text = inventoryData.coins.ToString();
    }
}
