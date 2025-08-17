using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class DoorHandle : MonoBehaviour
{
    [Header("Setup")]
    public InventoryData inventoryData;
    public KeyCode interactKey = KeyCode.U; // nastavíš aj v Inspectore

    [Tooltip("Na odomknutie je potrebný kľúč?")]
    public bool requiresKey = true;

    [Tooltip("Spotrebovať kľúč pri odomknutí?")]
    public bool consumeKey = true;

    [Tooltip("Dvere začínajú už odomknuté (napr. pre test)")]
    public bool startUnlocked = false;

    [Header("Animator")]
    [Tooltip("Názov boolu v Animatori, ktorý otvára dvere.")]
    public string animatorBoolName = "isOpen";   // <-- tvoje meno parametra

    [Header("Persistence (voliteľné)")]
    public bool persistAcrossSessions = false;
    public string doorId;

    private Animator anim;
    private Collider2D solidCollider;    // isTrigger = false (blokuje)
    private Collider2D triggerCollider;  // isTrigger = true  (interakcia)
    private bool inRange;
    private bool isUnlocked;
    private int isOpenHash;

    void Awake()
    {
        anim = GetComponent<Animator>();
        isOpenHash = Animator.StringToHash(animatorBoolName);

        // Nájdeme existujúce collidery na TOM ISTOM GameObjecte
        var all = GetComponents<Collider2D>();
        foreach (var c in all)
        {
            if (c.isTrigger) triggerCollider = c;
            else solidCollider = c;
        }

        // Ak NEMÁŠ trigger → automaticky ho pridáme
        if (triggerCollider == null)
        {
            var bc = gameObject.AddComponent<BoxCollider2D>();
            bc.isTrigger = true;
            // zmysluplná veľkosť podľa pevného collidera, ak existuje
            if (solidCollider is BoxCollider2D s)
            {
                bc.size = s.size * 1.2f;
                bc.offset = s.offset;
            }
            triggerCollider = bc;
            Debug.Log($"[DoorHandle] '{name}': chýbal TRIGGER collider, pridaný automaticky.");
        }

        if (solidCollider == null)
        {
            Debug.LogWarning($"[DoorHandle] '{name}': nenašiel som pevný Collider2D (isTrigger=false). Dvere nebudú fyzicky blokovať prechod.");
        }

        // id pre perzistenciu
        if (string.IsNullOrEmpty(doorId))
        {
            var p = transform.position;
            doorId = $"{gameObject.scene.name}_{name}_{Mathf.RoundToInt(p.x)}_{Mathf.RoundToInt(p.y)}";
        }

        // (voliteľné) varovanie, ak Animator nemá daný bool
        bool found = false;
        foreach (var p2 in anim.parameters)
            if (p2.type == AnimatorControllerParameterType.Bool && p2.name == animatorBoolName) { found = true; break; }
        if (!found) Debug.LogWarning($"[DoorHandle] Animator '{name}' nemá bool parameter '{animatorBoolName}'. Skontroluj názov/prechody.");
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
    }

    void Update()
    {
        if (!inRange) return;
        if (Input.GetKeyDown(interactKey))
            TryUnlock();
    }

    void TryUnlock()
    {
        if (isUnlocked) return;

        if (requiresKey)
        {
            if (inventoryData != null && inventoryData.keys > 0)
            {
                if (consumeKey) inventoryData.UseKey(); // odpočíta + RaiseChanged + SFX (ak máš)
                SetUnlocked(true);
                PlaySfx("doorUnlock");
            }
            else
            {
                PlaySfx("doorLocked");
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
    }

    void ApplyState(bool initial)
    {
        anim.SetBool(isOpenHash, isUnlocked);              // prepne animator bool 'isOpen'
        if (solidCollider != null) solidCollider.isTrigger = isUnlocked; // otvorené = priechodné
    }

    string GetPrefKey() => $"door_unlocked_{doorId}";

    void PlaySfx(string key)
    {
        if (!string.IsNullOrEmpty(key) && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(key);
    }

    // — TRIGGER DETEKCIA HRÁČA —
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayerCollider(other)) return;
        inRange = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayerCollider(other)) return;
        inRange = false;
    }

    bool IsPlayerCollider(Collider2D col)
    {
        // robustné: hľadá PlayerController v rodičoch alebo tag "Player"
        return col.GetComponentInParent<PlayerController>() != null
               || col.CompareTag("Player")
               || (col.transform.root != null && col.transform.root.CompareTag("Player"));
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // vizualizácia triggera pre debug
        var triggers = GetComponents<Collider2D>();
        foreach (var c in triggers)
        {
            if (!c.isTrigger) continue;
            Gizmos.color = Color.cyan;
            var b = c.bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
#endif
}
