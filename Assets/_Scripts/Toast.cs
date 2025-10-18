using UnityEngine;
using TMPro;

public class Toast : MonoBehaviour
{
    public static Toast Instance;
    public TextMeshProUGUI toastText;
    public CanvasGroup group;
    public float showTime = 1.5f, fadeTime = 0.25f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        if (!group) group = toastText.GetComponent<CanvasGroup>();
        group.alpha = 0f;
    }

    public static void Show(string msg) { Instance?.StartCoroutine(Instance.ShowCo(msg)); }

    System.Collections.IEnumerator ShowCo(string msg)
    {
        toastText.text = msg;
        // fade in
        float t = 0; while (t < fadeTime) { t += Time.deltaTime; group.alpha = Mathf.Lerp(0, 1, t / fadeTime); yield return null; }
        yield return new WaitForSeconds(showTime);
        // fade out
        t = 0; while (t < fadeTime) { t += Time.deltaTime; group.alpha = Mathf.Lerp(1, 0, t / fadeTime); yield return null; }
        group.alpha = 0f;
    }
}
