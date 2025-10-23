using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Refs")]
    public GameObject shopPanel;
    public InventoryData inventory;    
    public TMP_Text coinsText;

    [Header("Prices")]
    public int healPrice = 10;
    public int revealPrice = 15;
    public bool IsOpen => shopPanel != null && shopPanel.activeInHierarchy;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    void OnEnable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged += Refresh;
    }

    void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;
    }

    public bool CloseIfOpen()
    {
        if (IsOpen)
        {
            Close();
            return true;
        }
        return false;
    }

    public void Close()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    public void Open(string category = "ELIXIRS")
    {
        shopPanel.SetActive(true);
        Refresh();
    }

    public void BuyHeal()
    {
        if (inventory.SpendCoins(healPrice))
            inventory.AddHealPotion(1);
        Refresh();
    }

    public void BuyReveal()
    {
        if (inventory.SpendCoins(revealPrice))
            inventory.AddRevealPotion(1);
        Refresh();
    }

    private void Refresh()
    {
        if (coinsText != null && inventory != null)
            coinsText.text = inventory.coins.ToString();
    }
}
