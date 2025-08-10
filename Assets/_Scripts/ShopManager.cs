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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            Close();
        }
    }
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        shopPanel.SetActive(false);
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

    public void Open(string category = "ELIXIRS")
    {
        shopPanel.SetActive(true);
        Refresh();
        
        // Time.timeScale = 0f;
    }

    public void Close()
    {
        shopPanel.SetActive(false);
        // Time.timeScale = 1f;
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
