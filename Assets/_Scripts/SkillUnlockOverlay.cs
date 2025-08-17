using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillUnlockOverlay : MonoBehaviour
{
    public static SkillUnlockOverlay Instance { get; private set; }

    [Header("UI Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float holdDuration = 2.5f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        // Necháme objekt stále aktívny, iba ho sprieh¾adníme
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    /// <summary>
    /// Zobrazí overlay s textom a ikonou
    /// </summary>
    public void Show(string skillName, Sprite skillIcon, float delay = 0f)
    {
        // ak by niekto vypol objekt v Hierarchy, zapneme ho
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(ShowRoutine(skillName, skillIcon, delay));
    }

    private IEnumerator ShowRoutine(string skillName, Sprite skillIcon, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        if (title) title.text = skillName;
        if (icon)
        {
            icon.sprite = skillIcon;
            icon.enabled = (skillIcon != null);
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        // Fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // držanie na obrazovke
        yield return new WaitForSecondsRealtime(holdDuration);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        HideInstant();
    }

    /// <summary>
    /// Okamžite skryje overlay (ale ponechá objekt aktívny)
    /// </summary>
    public void HideInstant()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}
