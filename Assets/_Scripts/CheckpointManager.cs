using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Header("Refs (rovnak�, ako pou��va hr��)")]
    public PlayerController player;
    public InventoryData inventory;
    public PlayerAbilitiesData abilities;

    Vector3 lastCheckpointPos;
    bool hasCheckpoint;

    InventorySnap invSnap;
    AbilitiesSnap abilSnap;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveCheckpoint(Checkpoint cp)
    {
        if (!player) player = FindObjectOfType<PlayerController>();
        lastCheckpointPos = cp.SpawnPos;
        hasCheckpoint = true;

        invSnap = InventorySnap.From(inventory);
        abilSnap = AbilitiesSnap.From(abilities);
    }

    public void RespawnPlayer()
    {
        if (!player) player = FindObjectOfType<PlayerController>();
        if (!player)
            return;

        if (!hasCheckpoint)
        {
            // fallback: ak neexistuje checkpoint, urob to �o doteraz (reload sc�ny)
            var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(s.name);
            return;
        }

        // presun + reset r�chlosti
        player.transform.position = lastCheckpointPos;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        // obnova invent�ra + schopnost�
        invSnap.ApplyTo(inventory);
        abilSnap.ApplyTo(abilities);

        // (odpor��an�) dopl� HP
        var ph = player.GetComponent<PlayerHealth>();
        if (ph) ph.health = ph.maxHealth;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("checkpoint");
        Toast.Show("You got one more chance");
    }

    // ------- Snapshots --------
    [System.Serializable]
    struct InventorySnap
    {
        public int heal, reveal, coins, keys;

        public static InventorySnap From(InventoryData d)
        {
            InventorySnap s = new InventorySnap();
            if (!d) return s;
            s.heal = d.healPotions;
            s.reveal = d.revealPotions;
            s.coins = d.coins;
            s.keys = d.keys;           // ak m� Keys v InventoryData
            return s;
        }

        public void ApplyTo(InventoryData d)
        {
            if (!d) return;
            d.healPotions = heal;
            d.revealPotions = reveal;
            d.coins = coins;
            d.keys = keys;
            d.RaiseChanged(); // vi� mal� �pravu InventoryData ni��ie
        }
    }

    [System.Serializable]
    struct AbilitiesSnap
    {
        public bool dj, dash, wj, ws;

        public static AbilitiesSnap From(PlayerAbilitiesData a)
        {
            AbilitiesSnap s = new AbilitiesSnap();
            if (!a) return s;
            s.dj = a.canDoubleJump;
            s.dash = a.canDash;
            s.wj = a.canWallJump;
            s.ws = a.canWallSlide;
            return s;
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
}
