using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class DoorHandle : MonoBehaviour
{
    [Header("Setup")]
    public InventoryData inventoryData;
    public KeyCode interactKey = KeyCode.F;

    [Tooltip("Na odomknutie je potrebný kľúč?")]
    public bool requiresKey = true;

    [Tooltip("Spotrebovať kľúč pri odomknutí?")]
    public bool consumeKey = true;

    [Tooltip("Dvere začínajú už odomknuté (test)")]
    public bool startUnlocked = false;

    [Header("Animator")]
    public string animatorBoolName = "isOpen";

    [Header("UI Tooltip")]
    public GameObject tooltipUI;     // parent GO s textom/ikonou
    public TMP_Text tooltipText;     // voliteľné

    [Header("Interakcia bez triggeru")]
    public float interactRange = 1.6f;     // dosah pre tooltip a F
    public Transform playerOverride;       // voliteľné – ak necháš prázdne, nájde podľa tagu "Player"

    [Header("Persistence (voliteľné)")]
    public bool persistAcrossSessions = false;
    public string doorId;

    private Animator anim;
    private Collider2D solidCollider; // nepovinné, ale ak je prítomný, prepíname isTrigger pri odomknutí
    private bool isUnlocked;
    private int isOpenHash;
    private Transform player;

    void Awake()
    {
        anim = GetComponent<Animator>();
        isOpenHash = Animator.StringToHash(animatorBoolName);

        // ak má parent nejaký pevný collider, zapamätáme si ho
        solidCollider = GetComponent<Collider2D>();
        if (solidCollider != null && solidCollider.isTrigger)
            Debug.LogWarning("[DoorHandle] Na parente máš trigger collider. Nevadí, ale blokovanie rieš dieťaťom (Square).");

        if (string.IsNullOrEmpty(doorId))
        {
            var p = transform.position;
            doorId = $"{gameObject.scene.name}_{name}_{Mathf.RoundToInt(p.x)}_{Mathf.RoundToInt(p.y)}";
        }

        if (tooltipUI) tooltipUI.SetActive(false);

        // sanity check animator bool
        bool found = false;
        foreach (var p2 in anim.parameters)
            if (p2.type == AnimatorControllerParameterType.Bool && p2.name == animatorBoolName) { found = true; break; }
        if (!found) Debug.LogWarning($"[DoorHandle] Animator nemá bool '{animatorBoolName}'.");
    }

    void Start()
    {
        isUnlocked = startUnlocked;
        if (persistAcrossSessions)
        {
            int pref = PlayerPrefs.GetInt(GetPrefKey(), isUnlocked ? 1 : 0);
            isUnlocked = (pref == 1);
        }
        ApplyState(initial: true);

        // nájdi hráča
        player = playerOverride ? playerOverride : FindPlayerTransform();
        if (!player) Debug.LogWarning("[DoorHandle] Nenašiel som hráča (tag 'Player'). Tooltip pôjde OFF.");
    }

    void Update()
    {
        UpdateTooltipAndInteraction();
    }

    void UpdateTooltipAndInteraction()
    {
        if (!player) { if (tooltipUI) tooltipUI.SetActive(false); return; }

        // vzdialenosť
        float dist = Vector2.Distance(player.position, transform.position);
        bool inRange = dist <= interactRange;

        // tooltip ukazujeme iba ak sú dvere ZAMKNUTÉ a hráč je v dosahu
        bool show = inRange && !isUnlocked;
        if (tooltipUI) tooltipUI.SetActive(show);

        // aktualizuj text
        if (show && tooltipText)
        {
            if (!requiresKey)
            {
                tooltipText.text = $"{interactKey} — Open";
            }
            else
            {
                bool hasKey = (inventoryData != null && inventoryData.keys > 0);
                tooltipText.text = hasKey ? $"{interactKey} — Unlock" : $"Need  a  key";
            }
        }

        // interakcia
        if (show && Input.GetKeyDown(interactKey))
            TryUnlock();
    }

    void TryUnlock()
    {
        if (isUnlocked) return;

        if (requiresKey)
        {
            if (inventoryData != null && inventoryData.keys > 0)
            {
                if (consumeKey) inventoryData.UseKey();
                SetUnlocked(true);
                PlaySfx("doorUnlock");
            }
            else
            {
                PlaySfx("doorLocked");
                if (tooltipText) tooltipText.text = $"{interactKey} — Need a key";
            }
        }
        else
        {
            SetUnlocked(true);
            PlaySfx("doorUnlock");
        }
    }

    public void SetUnlocked(bool unlocked)
    {
        if (isUnlocked == unlocked) return;
        isUnlocked = unlocked;

        if (persistAcrossSessions)
        {
            PlayerPrefs.SetInt(GetPrefKey(), isUnlocked ? 1 : 0);
            PlayerPrefs.Save();
        }
        ApplyState(initial: false);

        // po odomknutí schovaj tooltip
        if (tooltipUI) tooltipUI.SetActive(false);
    }

    void ApplyState(bool initial)
    {
        anim.SetBool(isOpenHash, isUnlocked);

        // ak je na parente pevný collider, otvorené = priechodné
        if (solidCollider) solidCollider.isTrigger = isUnlocked;
        // POZOR: ak blokovanie rieši dieťa `Square`, toto sa ho nedotkne – je to OK.
    }

    string GetPrefKey() => $"door_unlocked_{doorId}";

    void PlaySfx(string key)
    {
        if (!string.IsNullOrEmpty(key) && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(key);
    }

    Transform FindPlayerTransform()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        return go ? go.transform : null;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // vizualizácia dosahu
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
#endif
}
