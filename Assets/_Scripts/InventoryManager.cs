using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Game/Inventory Data")]
public class InventoryData : ScriptableObject
{
    [Header("Inventory")]
    public int healPotions;
    public int revealPotions;
    public int coins;

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

    public void ResetInventory()
    {
        healPotions = 0;
        revealPotions = 0;
        coins = 0;
    }
}
