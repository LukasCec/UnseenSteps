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

    private void Awake()
    {
        UpdateUI(); 
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
