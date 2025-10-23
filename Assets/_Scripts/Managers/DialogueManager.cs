using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices; // tlačidlá/objekty s TMP textom vo vnútri

    [Header("Typing Effect")]
    [SerializeField] private float textSpeed = 0.03f;

    [Header("Speaker UI (optional)")]
    [SerializeField] private Image characterImage;        // použij, ak máš UI Image
    [SerializeField] private RawImage characterRawImage;  // použij, ak máš RawImage
    [SerializeField] private TextMeshProUGUI nameText;    // nepovinné meno postavy

    private TextMeshProUGUI[] choicesText;
    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }

    // ─────────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("More Than One Dialogue Manager !!!");
        }
        instance = this;
    }

    public static DialogueManager GetInstance() => instance;

    private void Start()
    {
        dialogueIsPlaying = false;
        if (dialoguePanel) dialoguePanel.SetActive(false);

        // cache TMP texty z choices
        choicesText = new TextMeshProUGUI[choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i] != null)
            {
                choicesText[i] = choices[i].GetComponentInChildren<TextMeshProUGUI>(true);
                choices[i].SetActive(false);
            }
        }

        // default – skryť speaker prvky
        SetSpeakerSprite(null);
        SetSpeakerTexture(null);
        SetSpeakerName(null);
    }

    private void Update()
    {
        if (!dialogueIsPlaying) return;

        // pokračuj medzerou, len ak nie sú k dispozícii choices
        if (currentStory.currentChoices.Count == 0 && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape)))
        {
            ContinueStory();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // API: vstup do dialógu (bez speaker info)
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        EnterDialogueMode(inkJSON, (Sprite)null, null);
    }

    // API: vstup do dialógu (so Sprite portrétom + menom)
    public void EnterDialogueMode(TextAsset inkJSON, Sprite speakerPortrait, string speakerName = null)
    {
        if (inkJSON == null) { Debug.LogWarning("Ink JSON is null"); return; }

        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        if (dialoguePanel) dialoguePanel.SetActive(true);

        // nastav speaker (Image/RawImage fallback)
        if (characterImage != null)
        {
            SetSpeakerSprite(speakerPortrait);
        }
        if (characterRawImage != null)
        {
            // ak máme sprite, premietneme texture + uvRect (aby zobrazil presný slice)
            SetSpeakerSpriteToRawImage(speakerPortrait);
        }
        SetSpeakerName(speakerName);

        ContinueStory();
    }

    // API: vstup do dialógu (s Texture2D + menom) – ak používaš RawImage s custom textúrou
    public void EnterDialogueMode(TextAsset inkJSON, Texture2D speakerTexture, string speakerName = null)
    {
        if (inkJSON == null) { Debug.LogWarning("Ink JSON is null"); return; }

        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        if (dialoguePanel) dialoguePanel.SetActive(true);

        if (characterRawImage != null) SetSpeakerTexture(speakerTexture);
        if (characterImage != null) SetSpeakerSprite(null);

        SetSpeakerName(speakerName);
        ContinueStory();
    }

    // ─────────────────────────────────────────────────────────────────────────────

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (dialogueText) dialogueText.text = "";

        // skryť choices
        for (int i = 0; i < choices.Length; i++)
            if (choices[i]) choices[i].SetActive(false);

        // skryť speaker prvky
        SetSpeakerSprite(null);
        SetSpeakerTexture(null);
        SetSpeakerName(null);
    }

    private void ContinueStory()
    {
        if (currentStory == null) return;

        if (currentStory.canContinue)
        {
            string line = currentStory.Continue();

            // Handle any tags first (they may be paired with blank lines)
            HandleTags(currentStory.currentTags);

            // NEW: skip whitespace-only lines
            if (string.IsNullOrWhiteSpace(line))
            {
                // Try to continue again (until we hit a non-empty line or end)
                ContinueStory();
                return;
            }

            // (Optional) trim trailing newlines so TMP doesn’t add extra spacing
            line = line.TrimEnd('\r', '\n');

            StopAllCoroutines();
            StartCoroutine(TypeDialogue(line));
            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void HandleTags(List<string> tags)
    {
        if (tags == null) return;

        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag)) continue;

            // príklad tagu: OPEN_SHOP alebo OPEN_SHOP:ALCHEMY
            if (tag.StartsWith("OPEN_SHOP"))
            {
                string category = "DEFAULT";
                var parts = tag.Split(':');
                if (parts.Length > 1) category = parts[1];

                if (ShopManager.Instance != null)
                    ShopManager.Instance.Open(category);
            }
            else if (tag.Trim() == "SPAWN_PORTAL")
            {
                var spawner = GameObject.FindObjectOfType<PortalSpawner>();
                if (spawner != null) spawner.Spawn();
            }
            else if (tag.StartsWith("LOAD_SCENE:"))
            {
                var parts = tag.Split(':');
                if (parts.Length > 1)
                {
                    string sceneName = parts[1].Trim();
                    if (!string.IsNullOrEmpty(sceneName))
                        SceneManager.LoadScene(sceneName);
                }
            }
        }
    }

    private void DisplayChoices()
    {
        if (choices == null || choices.Length == 0) return;

        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support.");
        }

        int i = 0;
        for (; i < currentChoices.Count && i < choices.Length; i++)
        {
            if (!choices[i]) continue;
            choices[i].SetActive(true);
            if (choicesText[i]) choicesText[i].text = currentChoices[i].text;
        }

        // vypni zvyšné
        for (; i < choices.Length; i++)
        {
            if (choices[i]) choices[i].SetActive(false);
        }
    }

    private IEnumerator TypeDialogue(string sentence)
    {
        if (!dialogueText)
            yield break;

        dialogueText.text = "";
        if (string.IsNullOrEmpty(sentence))
            yield break;

        foreach (char c in sentence.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        if (currentStory == null) return;
        if (choiceIndex < 0 || choiceIndex >= currentStory.currentChoices.Count) return;

        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Helpery na speaker UI

    private void SetSpeakerName(string s)
    {
        if (!nameText) return;

        bool show = !string.IsNullOrEmpty(s);
        nameText.gameObject.SetActive(show);
        if (show) nameText.text = s;
    }

    private void SetSpeakerSprite(Sprite sp)
    {
        if (!characterImage) return;

        characterImage.enabled = (sp != null);
        characterImage.sprite = sp;
    }

    private void SetSpeakerTexture(Texture2D tex)
    {
        if (!characterRawImage) return;

        characterRawImage.enabled = (tex != null);
        characterRawImage.texture = tex;
        characterRawImage.uvRect = new Rect(0, 0, 1, 1); // celá textúra
    }

    // Ak chceš použiť Sprite (slice zo sprite sheetu) do RawImage:
    private void SetSpeakerSpriteToRawImage(Sprite sp)
    {
        if (!characterRawImage) return;

        if (sp == null)
        {
            characterRawImage.enabled = false;
            characterRawImage.texture = null;
            return;
        }

        var tex = sp.texture;
        var r = sp.textureRect; // pixelové súradnice výrezu v rámci texture

        characterRawImage.enabled = true;
        characterRawImage.texture = tex;
        characterRawImage.uvRect = new Rect(
            r.x / tex.width,
            r.y / tex.height,
            r.width / tex.width,
            r.height / tex.height
        );
    }
}
