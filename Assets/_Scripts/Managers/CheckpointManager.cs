using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Header("Refs (rovnakÈ, ako pouûÌva hr·Ë)")]
    public PlayerController player;
    public InventoryData inventory;
    public PlayerAbilitiesData abilities;

    Vector3 lastCheckpointPos;
    bool hasCheckpoint;

    InventorySnap checkpointInv;
    AbilitiesSnap checkpointAbil;

    bool pendingRespawn;
    Vector3 defaultSpawnPos;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[CheckpointManager] Duplicate detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromPrefs();
        CaptureDefaultSpawnIfNeeded();
        Debug.Log("[CheckpointManager] Alive id=" + GetInstanceID());
    }

    public void SaveCheckpoint(Checkpoint cp)
    {
        if (!player) player = FindObjectOfType<PlayerController>();
        lastCheckpointPos = cp.SpawnPos;
        hasCheckpoint = true;

        checkpointInv = InventorySnap.From(inventory);
        checkpointAbil = AbilitiesSnap.From(abilities);

        SaveToPrefs();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!pendingRespawn) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        StartCoroutine(RespawnAfterLoad());
    }

    IEnumerator RespawnAfterLoad()
    {
        yield return null;

        // n·jdi novÈho hr·Ëa po reloade
        player = FindObjectOfType<PlayerController>();

        if (player)
        {
            player.transform.position = lastCheckpointPos;

            var rb = player.GetComponent<Rigidbody2D>();
#if UNITY_6000_0_OR_NEWER
            if (rb) rb.linearVelocity = Vector2.zero;
#else
        if (rb) rb.velocity = Vector2.zero;
#endif
        }

        // obnov invent·r/ability zo snapshotu
        checkpointInv.ApplyTo(inventory);
        checkpointAbil.ApplyTo(abilities);

        // doplÚ HP
        var ph = player ? player.GetComponent<PlayerHealth>() : null;
        if (ph) ph.health = ph.maxHealth;

        pendingRespawn = false;

        AudioManager.Instance?.PlaySFX("checkpoint");
        Toast.Show("Respawn");
    }

    // ----------------- persist do PlayerPrefs -----------------
    const string K_HAS = "CP_Has";
    const string K_X = "CP_X", K_Y = "CP_Y", K_Z = "CP_Z";
    const string K_HEAL = "CP_Heal", K_REVEAL = "CP_Reveal", K_COINS = "CP_Coins", K_KEYS = "CP_Keys";
    const string K_DJ = "CP_DJ", K_DASH = "CP_Dash", K_WJ = "CP_WJ", K_WS = "CP_WS";

    void SaveToPrefs()
    {
        PlayerPrefs.SetInt(K_HAS, hasCheckpoint ? 1 : 0);
        PlayerPrefs.SetFloat(K_X, lastCheckpointPos.x);
        PlayerPrefs.SetFloat(K_Y, lastCheckpointPos.y);
        PlayerPrefs.SetFloat(K_Z, lastCheckpointPos.z);

        PlayerPrefs.SetInt(K_HEAL, checkpointInv.heal);
        PlayerPrefs.SetInt(K_REVEAL, checkpointInv.reveal);
        PlayerPrefs.SetInt(K_COINS, checkpointInv.coins);
        PlayerPrefs.SetInt(K_KEYS, checkpointInv.keys);

        PlayerPrefs.SetInt(K_DJ, checkpointAbil.dj ? 1 : 0);
        PlayerPrefs.SetInt(K_DASH, checkpointAbil.dash ? 1 : 0);
        PlayerPrefs.SetInt(K_WJ, checkpointAbil.wj ? 1 : 0);
        PlayerPrefs.SetInt(K_WS, checkpointAbil.ws ? 1 : 0);

        PlayerPrefs.Save();
    }

    void LoadFromPrefs()
    {
        hasCheckpoint = PlayerPrefs.GetInt(K_HAS, 0) == 1;
        if (!hasCheckpoint) return;

        lastCheckpointPos = new Vector3(
            PlayerPrefs.GetFloat(K_X, 0),
            PlayerPrefs.GetFloat(K_Y, 0),
            PlayerPrefs.GetFloat(K_Z, 0)
        );

        checkpointInv = new InventorySnap
        {
            heal = PlayerPrefs.GetInt(K_HEAL, 0),
            reveal = PlayerPrefs.GetInt(K_REVEAL, 0),
            coins = PlayerPrefs.GetInt(K_COINS, 0),
            keys = PlayerPrefs.GetInt(K_KEYS, 0),
        };

        checkpointAbil = new AbilitiesSnap
        {
            dj = PlayerPrefs.GetInt(K_DJ, 0) == 1,
            dash = PlayerPrefs.GetInt(K_DASH, 0) == 1,
            wj = PlayerPrefs.GetInt(K_WJ, 0) == 1,
            ws = PlayerPrefs.GetInt(K_WS, 0) == 1,
        };
    }

    void CaptureDefaultSpawnIfNeeded()
    {
        if (!player) player = FindObjectOfType<PlayerController>();
        if (player && defaultSpawnPos == Vector3.zero)
            defaultSpawnPos = player.transform.position;
    }

    // ------- Snapshots --------
    [System.Serializable]
    struct InventorySnap
    {
        public int heal, reveal, coins, keys;
        public static InventorySnap From(InventoryData d)
        {
            return new InventorySnap
            {
                heal = d ? d.healPotions : 0,
                reveal = d ? d.revealPotions : 0,
                coins = d ? d.coins : 0,
                keys = d ? d.keys : 0
            };
        }
        public void ApplyTo(InventoryData d)
        {
            if (!d) return;
            d.healPotions = heal;
            d.revealPotions = reveal;
            d.coins = coins;
            d.keys = keys;
            d.RaiseChanged(); // refresh UI
        }
    }

    [System.Serializable]
    struct AbilitiesSnap
    {
        public bool dj, dash, wj, ws;
        public static AbilitiesSnap From(PlayerAbilitiesData a)
        {
            return new AbilitiesSnap
            {
                dj = a && a.canDoubleJump,
                dash = a && a.canDash,
                wj = a && a.canWallJump,
                ws = a && a.canWallSlide
            };
        }
        public void ApplyTo(PlayerAbilitiesData a)
        {
            if (!a) return;
            a.canDoubleJump = dj;
            a.canDash = dash;
            a.canWallJump = wj;
            a.canWallSlide = ws;
        }
    }

    public static void ClearSavedCheckpoint()
    {
        // tie istÈ kæ˙Ëe, ktorÈ uû pouûÌvaö
        PlayerPrefs.DeleteKey("CP_Has");
        PlayerPrefs.DeleteKey("CP_X");
        PlayerPrefs.DeleteKey("CP_Y");
        PlayerPrefs.DeleteKey("CP_Z");
        PlayerPrefs.DeleteKey("CP_Heal");
        PlayerPrefs.DeleteKey("CP_Reveal");
        PlayerPrefs.DeleteKey("CP_Coins");
        PlayerPrefs.DeleteKey("CP_Keys");
        PlayerPrefs.DeleteKey("CP_DJ");
        PlayerPrefs.DeleteKey("CP_Dash");
        PlayerPrefs.DeleteKey("CP_WJ");
        PlayerPrefs.DeleteKey("CP_WS");
        PlayerPrefs.Save();

        // vymaû aj runtime stav
        if (Instance != null)
        {
            Instance.hasCheckpoint = false;
            Instance.lastCheckpointPos = Vector3.zero;
        }
    }

    public void RespawnAtCheckpoint()
    {
        CaptureDefaultSpawnIfNeeded();

        if (!player) { Debug.LogWarning("Respawn: player not found"); return; }

        // vyber cieæ ñ checkpoint, alebo poËiatoËn˝ spawn
        Vector3 targetPos = hasCheckpoint ? lastCheckpointPos : defaultSpawnPos;

        player.transform.position = targetPos;

        var rb = player.GetComponent<Rigidbody2D>();
#if UNITY_6000_0_OR_NEWER
        if (rb) rb.linearVelocity = Vector2.zero;
#else
    if (rb) rb.velocity = Vector2.zero;
#endif

        // d·ta: ak m·me checkpoint, obnov snapshoty; inak nemeÚ niË
        if (hasCheckpoint)
        {
            checkpointInv.ApplyTo(inventory);
            checkpointAbil.ApplyTo(abilities);
        }

        // HP doplniù vûdy
        var ph = player.GetComponent<PlayerHealth>();
        if (ph) ph.health = ph.maxHealth;

        AudioManager.Instance?.PlaySFX("checkpoint");
        Toast.Show(hasCheckpoint ? "Respawned" : "You  died");
    }

    public void RestartLevel(bool fullReset = true)
    {
        if (fullReset)
        {
            ClearSavedCheckpoint();
            ResetDataToDefaults();
        }

        var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }

    void ResetDataToDefaults()
    {
        // Invent·r
        if (inventory) inventory.ResetInventory();
        // Schopnosti
        if (abilities)
        {
            abilities.canDoubleJump = false;
            abilities.canDash = false;
            abilities.canWallJump = false;
            abilities.canWallSlide = false;
        }
    }
}
