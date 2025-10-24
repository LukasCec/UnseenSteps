using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitiesPanelUI : MonoBehaviour
{
    [Header("Data")]
    public PlayerAbilitiesData abilities;

    [Header("Icon roots (GameObjects s Raw/Image vo vnútri)")]
    public GameObject dashIconRoot;
    public GameObject doubleJumpIconRoot;
    public GameObject wallIconRoot;

    [Header("Key UI containers (dragni sem celé 'TxtBackground')")]
    public GameObject dashKeyRoot;
    public GameObject doubleJumpKeyRoot;
    public GameObject wallKeyRoot;

    [Header("Voliteľné: ak chceš meniť texty")]
    public TMP_Text dashKeyLabel;
    public TMP_Text doubleJumpKeyLabel;
    public TMP_Text wallKeyLabel;

    [Header("Vizuál")]
    [Range(0f, 1f)] public float lockedAlpha = 0.5f;

    MaskableGraphic dashG, djG, wallG;
    bool lastDash, lastDJ, lastWall;

    void Awake()
    {
        if (dashIconRoot) dashG = dashIconRoot.GetComponentInChildren<MaskableGraphic>(true);
        if (doubleJumpIconRoot) djG = doubleJumpIconRoot.GetComponentInChildren<MaskableGraphic>(true);
        if (wallIconRoot) wallG = wallIconRoot.GetComponentInChildren<MaskableGraphic>(true);
    }

    void Start()
    {
        lastDash = abilities && abilities.canDash;
        lastDJ = abilities && abilities.canDoubleJump;
        lastWall = abilities && (abilities.canWallJump || abilities.canWallSlide);
        RefreshAll();
    }

    void Update()
    {
        if (!abilities) return;
        bool dash = abilities.canDash;
        bool dj = abilities.canDoubleJump;
        bool wall = abilities.canWallJump || abilities.canWallSlide;
        if (dash != lastDash || dj != lastDJ || wall != lastWall)
        {
            lastDash = dash; lastDJ = dj; lastWall = wall;
            RefreshAll();
        }
    }

    void RefreshAll()
    {
        SetIcon(dashG, dashKeyRoot, dashKeyLabel, lastDash);
        SetIcon(djG, doubleJumpKeyRoot, doubleJumpKeyLabel, lastDJ);
        SetIcon(wallG, wallKeyRoot, wallKeyLabel, lastWall);
    }

    void SetIcon(MaskableGraphic g, GameObject keyRoot, TMP_Text key, bool unlocked)
    {
        if (g)
        {
            var c = g.color; c.a = unlocked ? 1f : lockedAlpha; g.color = c;
        }

        // Zobraz/skry celý blok s pozadím + textom
        if (keyRoot) keyRoot.SetActive(unlocked);
        else if (key) // fallback: keď si nevyplnil root, skry aspoň rodiča textu
        {
            var root = key.transform.parent ? key.transform.parent.gameObject : key.gameObject;
            root.SetActive(unlocked);
        }
    }
}
