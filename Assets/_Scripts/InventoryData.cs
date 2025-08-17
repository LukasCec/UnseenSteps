using UnityEngine;
using System;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Game/Inventory Data")]
public class InventoryData : ScriptableObject
{
    [Header("Inventory")]
    public int healPotions;
    public int revealPotions;
    public int coins;
    public int keys;

    public event Action OnInventoryChanged;
    private void RaiseChanged() => OnInventoryChanged?.Invoke();

    public void AddHealPotion(int amount = 1)
    {
        healPotions = Mathf.Max(0, healPotions + amount);
        RaiseChanged();
    }

    public void AddRevealPotion(int amount = 1)
    {
        revealPotions = Mathf.Max(0, revealPotions + amount);
        RaiseChanged();
    }

    public void AddCoins(int amount = 1)
    {
        coins = Mathf.Max(0, coins + amount);
        RaiseChanged();
    }

    public void AddKeys(int amount = 1)
    {
        keys = Mathf.Max(0, keys + amount);
        RaiseChanged();
    }

    public bool UseKey()
    {
        if (keys > 0)
        {
            keys--;
            RaiseChanged();
            return true;
        }
        return false;
    }

    public bool UseHealPotion()
    {
        if (healPotions > 0)
        {
            healPotions--;
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("heal");
            RaiseChanged();
            return true;
        }
        return false;
    }

    public bool UseRevealPotion()
    {
        if (revealPotions > 0)
        {
            revealPotions--;
            RaiseChanged();
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("reveal");
            return true;
        }
        return false;
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            RaiseChanged();
            return true;
        }
        return false;
    }

    public void ResetInventory()
    {
        healPotions = 0;
        revealPotions = 0;
        coins = 0;
        keys = 0;
        RaiseChanged();
    }

#if UNITY_EDITOR
    private void OnValidate() { RaiseChanged(); }
#endif
}
