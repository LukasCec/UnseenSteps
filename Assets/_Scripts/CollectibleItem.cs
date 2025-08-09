using UnityEngine;

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

    [Header("Pickup Effect")]
    public GameObject pickupEffectPrefab; 

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

        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }

        
        Destroy(gameObject);
    }
}
