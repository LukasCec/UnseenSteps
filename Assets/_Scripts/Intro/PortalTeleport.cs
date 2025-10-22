using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PortalTeleport : MonoBehaviour
{
    [Header("Target Scene")]
    [Tooltip("Presné meno scény pod¾a Build Settings.")]
    [SerializeField] private string targetSceneName = "Level_02";

    [Header("Trigger Options")]
    [Tooltip("Ak true, hráè musí stlaèi klávesu; inak sa teleportuje hneï po vstupe.")]
    [SerializeField] private bool requireKeyToEnter = false;
    [SerializeField] private KeyCode enterKey = KeyCode.F;
    [Tooltip("Volite¾nı delay pred naèítaním scény.")]
    [SerializeField] private float loadDelaySeconds = 0f;

    [Header("UI (optional)")]
    [Tooltip("Nápoveda (napr. 'Press F to Enter'), zapína sa len keï je hráè v dosahu a requireKeyToEnter je zapnuté.")]
    [SerializeField] private GameObject visualCueRoot;

    private bool playerInRange = false;
    private bool isLoading = false;

    private void Reset()
    {
        // Uistime sa, e collider je Trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (requireKeyToEnter)
        {
            if (visualCueRoot) visualCueRoot.SetActive(true);
        }
        else
        {
            TryTeleport();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (visualCueRoot) visualCueRoot.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || isLoading) return;

        if (requireKeyToEnter && Input.GetKeyDown(enterKey))
        {
            TryTeleport();
        }
    }

    private void TryTeleport()
    {
        if (isLoading) return;
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning("[PortalTeleport] targetSceneName nie je nastavené.");
            return;
        }
        isLoading = true;
        if (visualCueRoot) visualCueRoot.SetActive(false);
        StartCoroutine(LoadSceneAfterDelay());
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay()
    {
        if (loadDelaySeconds > 0f)
            yield return new WaitForSeconds(loadDelaySeconds);

        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }
}
