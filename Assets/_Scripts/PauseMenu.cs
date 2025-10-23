using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public static bool IsPaused { get; private set; }

    [Header("UI")]
    public CanvasGroup pauseCanvas;

    [Header("Overlays to close on ESC")]
    public GameObject dialoguePanel;   
    public GameObject shopPanel;       

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.Escape;
    public string mainMenuSceneName = "MainMenu";

    bool isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        HideInstant();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (CloseOverlays()) return;
            Toggle();
        }
    }

    bool CloseOverlays()
    {
        bool closed = false;

         var dm = DialogueManager.GetInstance();
        if (dm != null && dm.CloseIfOpen()) closed = true;

         if (ShopManager.Instance != null && ShopManager.Instance.CloseIfOpen())
            closed = true;

        return closed;
    }

    public void Toggle() { if (isPaused) Resume(); else Pause(); }

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

        // dôležité: vyèisti výber a osy po UI
        EventSystem.current?.SetSelectedGameObject(null);
        Input.ResetInputAxes();
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
            AudioManager.Instance.PlaySFX("buttonClick");
    }

    // ---- Button hooks ----
    public void OnContinuePressed()
    {
        PlayClick();
        EventSystem.current?.SetSelectedGameObject(null);
        Input.ResetInputAxes();
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
