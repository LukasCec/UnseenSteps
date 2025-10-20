using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class SkillGem : MonoBehaviour
{
    [Header("Data")]
    public PlayerAbilitiesData abilitiesData; 
                                             
    [Header("»o tento gem odomkne")]
    public bool unlockDash;
    public bool unlockDoubleJump;
    public bool unlockWallSlide;
    public bool unlockWallJump;

    [Header("Overlay UI")]
    public string overlayTitle = "Skill Unlocked!";
    public Sprite overlayIcon;

    [Header("Perzistencia (voliteænÈ)")]
    public bool rememberPickup = true;
    public string gemId;

    Collider2D col;

    void Reset()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Awake()
    {
        if (!col) col = GetComponent<Collider2D>();

        if (string.IsNullOrEmpty(gemId))
        {
            var p = transform.position;
            gemId = $"{gameObject.scene.name}_{name}_{Mathf.RoundToInt(p.x)}_{Mathf.RoundToInt(p.y)}";
        }

        if (rememberPickup && PlayerPrefs.GetInt(GetPrefKey(), 0) == 1)
        {
            // uû vyzdvihnutÈ ñ neukazuj znova
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Detekcia hr·Ëa ñ buÔ tag "Player" alebo prÌtomnosù PlayerController
        if (!other.CompareTag("Player") && other.GetComponentInParent<PlayerController>() == null)
            return;

        Collect();
    }

    void Collect()
    {
        // 1) Odomkni schopnosti (PlayerAbilitiesData pouûÌva PlayerController na gateovanie akciÌ)
        if (abilitiesData)
        {
            if (unlockDash) abilitiesData.canDash = true;
            if (unlockDoubleJump) abilitiesData.canDoubleJump = true;
            if (unlockWallSlide) abilitiesData.canWallSlide = true;
            if (unlockWallJump) abilitiesData.canWallJump = true;
        }

        // 2) Overlay
        SkillUnlockOverlay.Instance?.Show(overlayTitle, overlayIcon, 0f);

        // 3) SFX
        AudioManager.Instance?.PlaySFX("item"); // alebo si pridaj vlastn˝ "gemPickup"

        // 4) Zapam‰taj si zber (ak chceö)
        if (rememberPickup)
        {
            PlayerPrefs.SetInt(GetPrefKey(), 1);
            PlayerPrefs.Save();
        }

        // 5) Prehraj zvuk
        if (AudioManager.Instance != null && !string.IsNullOrEmpty("item"))
            AudioManager.Instance.PlaySFX("item");

        // 6) ZniË objekt
        Destroy(gameObject);
    }

    string GetPrefKey() => $"gem_collected_{gemId}";
}