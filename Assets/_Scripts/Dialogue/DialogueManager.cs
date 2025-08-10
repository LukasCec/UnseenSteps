using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    
    private static DialogueManager instance;
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;

    [Header("Typing Effect")]
    [SerializeField] private float textSpeed = 0.03f;

    private TextMeshProUGUI[] choicesText;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More Than One Dialogue Manager !!!");
        }

        instance = this; 
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        choicesText = new TextMeshProUGUI[choices.Length];

        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        // || Input.GetMouseButtonDown(0)
        if (currentStory.currentChoices.Count == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            ContinueStory();
        }


    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();

    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            string dialogueToDisplay = currentStory.Continue();

            
            HandleTags(currentStory.currentTags);

            StopAllCoroutines();
            StartCoroutine(TypeDialogue(dialogueToDisplay));
            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void HandleTags(System.Collections.Generic.List<string> tags)
    {
        if (tags == null) return;

        foreach (var tag in tags)
        {
            
            if (tag.StartsWith("OPEN_SHOP"))
            {
                
                string category = "DEFAULT";
                var parts = tag.Split(':');
                if (parts.Length > 1) category = parts[1];

                if (ShopManager.Instance != null)
                    ShopManager.Instance.Open(category);
            }
           
        }
    }


    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length) 
        {
            Debug.LogError("More choices were given than the UI can support.");
        }

        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }


    private IEnumerator SelectFirstChoice()
    {
        // EventSystem.current.SetSelectedGameObject(null);
        // yield return new WaitForEndOfFrame();
        // EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
        yield break;
    }

    private IEnumerator TypeDialogue(string dialogueSentence)
    {
        dialogueText.text = "";
        foreach (char letter in dialogueSentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }


    public void MakeChoice(int ChoiceIndex)
    {
        currentStory.ChooseChoiceIndex(ChoiceIndex);
        ContinueStory();
    }

}
