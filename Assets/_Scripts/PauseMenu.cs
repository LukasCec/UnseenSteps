using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    public static bool IsPaused { get; private set; }

    [Header("UI")]
    [Tooltip("Root canvas/panel pauzovacieho menu")]
    public CanvasGroup pauseCanvas;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.Escape;
    [Tooltip("N·zov scÈny s hlavn˝m menu")]
    public string mainMenuSceneName = "MainMenu";

    bool isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        HideInstant(); // na ötarte nech nie je vidieù
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ShopManager.Instance?.Close();
            Toggle();
        }
    }

    public void Toggle()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        IsPaused = true;
        Time.timeScale = 0f;
        if (pauseCanvas)
        {
            pauseCanvas.alpha = 1f;
            pauseCanvas.interactable = true;
            pauseCanvas.blocksRaycasts = true;
        }
    }

    public void Resume()
    {
        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;
        HideInstant();
    }

    void HideInstant()
    {
        if (pauseCanvas)
        {
            pauseCanvas.alpha = 0f;
            pauseCanvas.interactable = false;
            pauseCanvas.blocksRaycasts = false;
        }
    }

    void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("buttonClick");  // n·zov musÌ sedieù v AudioManager.sfx
    }

    // ---- Button hooks ----

    public void OnContinuePressed()
    {
        PlayClick();
        Resume();
    }

    public void OnRestartLevelPressed()
    {
        PlayClick();
        Resume();
        CheckpointManager.Instance?.RestartLevel(fullReset: true);
    }

    public void OnMainMenuPressed()
    {
        PlayClick();
        Resume();
        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            SceneManager.LoadScene(0);
    }

    public void OnQuitGamePressed()
    {
        PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
