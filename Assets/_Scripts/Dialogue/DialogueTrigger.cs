using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    public enum TriggerMode { AutoOnEnter, PressKeyInRange }

    [Header("Trigger Mode")]
    public TriggerMode mode = TriggerMode.PressKeyInRange;
    public KeyCode interactionKey = KeyCode.F;
    public bool oneShot = false;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Visual Cue (only for PressKeyInRange)")]
    [SerializeField] private GameObject visualCueRoot;
    [SerializeField] private TMP_Text visualCueText;
    [SerializeField] private Image visualCueImage;

    [Header("Speaker (per trigger)")]
    [SerializeField] private Sprite speakerPortrait;
    [SerializeField] private string speakerName;

    [Header("SFX (optional)")]
    [SerializeField] private string sfxOnStart = "merchant";

    [Header("Optional Start Delay")]
    public bool useStartDelay = false;
    [Min(0f)] public float startDelaySeconds = 1f;

    private bool playerInRange = false;
    private bool alreadyPlayed = false;
    private Coroutine pendingStartRoutine;

    private void Awake()
    {
        if (visualCueRoot) visualCueRoot.SetActive(false);
    }

    // ───────────────────── NEW: zisti, či hráč stojí v triggeri už pri starte
    private void Start()
    {
        CheckInitialOverlap();
        // ak je AutoOnEnter a hráč je už dnu, spusti (alebo rozbehni odpočet)
        if (mode == TriggerMode.AutoOnEnter && playerInRange && (!alreadyPlayed || !oneShot))
            QueueStartDialogue();
    }

    private void CheckInitialOverlap()
    {
        var col = GetComponent<Collider2D>();
        if (!col) return;

        // nájdeme kolidéry, ktoré sa s týmto triggeom prekrývajú
        var filter = new ContactFilter2D();
        filter.NoFilter();
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[8];
        int count = col.Overlap(filter, results);
        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i].CompareTag("Player"))
            {
                playerInRange = true;
                break;
            }
        }
    }
    // ─────────────────────────────────────────────────────────────────────────────

    private void Update()
    {
        if (!playerInRange) return;

        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (visualCueRoot) visualCueRoot.SetActive(false);
            return;
        }

        if (mode == TriggerMode.AutoOnEnter)
        {
            if ((!alreadyPlayed || !oneShot) && pendingStartRoutine == null)
                QueueStartDialogue();
        }
        else // PressKeyInRange
        {
            if (visualCueRoot) visualCueRoot.SetActive(true);
            if (Input.GetKeyDown(interactionKey))
                QueueStartDialogue();
        }
    }

    private void QueueStartDialogue()
    {
        if (inkJSON == null) return;

        
        if (pendingStartRoutine != null) return;

        if (useStartDelay && startDelaySeconds > 0f)
            pendingStartRoutine = StartCoroutine(StartDialogueDelayed());
        else
            StartDialogue();
    }

    private System.Collections.IEnumerator StartDialogueDelayed()
    {
        float t = 0f;
        while (t < startDelaySeconds)
        {
            if (!playerInRange) { pendingStartRoutine = null; yield break; }
            if (oneShot && alreadyPlayed) { pendingStartRoutine = null; yield break; }
            if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying)
            { pendingStartRoutine = null; yield break; }

            t += Time.deltaTime;
            yield return null;
        }

        StartDialogue();
        pendingStartRoutine = null;
    }

    private void StartDialogue()
    {
        if (inkJSON == null) return;

        if (AudioManager.Instance != null && !string.IsNullOrEmpty(sfxOnStart))
            AudioManager.Instance.PlaySFX(sfxOnStart);

        DialogueManager.GetInstance()
            .EnterDialogueMode(inkJSON, speakerPortrait, speakerName);

        if (visualCueRoot) visualCueRoot.SetActive(false);
        alreadyPlayed = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInRange = true;

        if (mode == TriggerMode.AutoOnEnter && (!alreadyPlayed || !oneShot))
            QueueStartDialogue();
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInRange = false;

        if (pendingStartRoutine != null)
        {
            StopCoroutine(pendingStartRoutine);
            pendingStartRoutine = null;
        }

        if (visualCueRoot) visualCueRoot.SetActive(false);
    }
}
