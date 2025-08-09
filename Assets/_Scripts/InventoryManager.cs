using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Game/Inventory Data")]
public class InventoryData : ScriptableObject
{
    [Header("Inventory")]
    public int healPotions;
    public int revealPotions;
    public int coins;

    // --- Add ---
    public void AddHealPotion(int amount = 1)
    {
        healPotions = Mathf.Max(0, healPotions + amount);
    }

    public void AddRevealPotion(int amount = 1)
    {
        revealPotions = Mathf.Max(0, revealPotions + amount);
    }

    public void AddCoins(int amount = 1)
    {
        coins = Mathf.Max(0, coins + amount);
    }

    // --- Use/Spend ---
    public bool UseHealPotion()
    {
        if (healPotions > 0)
        {
            healPotions--;
            return true;
        }
        return false;
    }

    public bool UseRevealPotion()
    {
        if (revealPotions > 0)
        {
            revealPotions--;
            return true;
        }
        return false;
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            return true;
        }
        return false;
    }

    // --- Reset ---
    public void ResetInventory()
    {
        healPotions = 0;
        revealPotions = 0;
        coins = 0;
    }
}
