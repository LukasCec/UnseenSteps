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
    [SerializeField] private GameObject visualCueRoot; // kontajner (enable/disable)
    [SerializeField] private TMP_Text visualCueText;    // napr. "Press F"
    [SerializeField] private Image visualCueImage;      // ikona klávesy

    [Header("Speaker (per trigger)")]
    [SerializeField] private Sprite speakerPortrait;
    [SerializeField] private string speakerName;

    [Header("SFX (optional)")]
    [SerializeField] private string sfxOnStart = "merchant";

    private bool playerInRange = false;
    private bool alreadyPlayed = false;

    private void Awake()
    {
        if (visualCueRoot) visualCueRoot.SetActive(false);
    }

    private void Update()
    {
        // ak hráè nie je v dosahu, niè
        if (!playerInRange) return;

        // ak práve prebieha dialóg, nápovedu skry a nerieš input
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (visualCueRoot) visualCueRoot.SetActive(false);
            return;
        }

        if (mode == TriggerMode.AutoOnEnter)
        {
            if (!alreadyPlayed || !oneShot) StartDialogue();
        }
        else // PressKeyInRange
        {
            if (visualCueRoot) visualCueRoot.SetActive(true);

            if (Input.GetKeyDown(interactionKey))
                StartDialogue();
        }
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

        // Auto režim – spusti hneï po vstupe
        if (mode == TriggerMode.AutoOnEnter && (!alreadyPlayed || !oneShot))
            StartDialogue();
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInRange = false;

        if (visualCueRoot) visualCueRoot.SetActive(false);
    }
}
