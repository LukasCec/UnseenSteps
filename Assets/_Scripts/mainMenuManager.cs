using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene To Load")]
    [Tooltip("Názov alebo index scény, ktorá sa má spusti po Play.")]
    public string playSceneName = "GameDaysScene";
    public int playSceneIndex = -1;

    [Header("Optional: Fade/Loading")]
    public Animator fadeAnimator;          
    public float fadeDuration = 0.5f;      

    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic("background");
    }

    public void OnPlayPressed()
    {
        PlayClick();
        StartCoroutine(LoadGame());
    }

    public void OnSettingsPressed()
    {
        PlayClick();
    }

    public void OnExitPressed()
    {
        PlayClick();
      
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadGame()
    {
        
        if (fadeAnimator != null) fadeAnimator.SetTrigger("FadeOut");
        if (fadeDuration > 0f) yield return new WaitForSecondsRealtime(fadeDuration);

        
        AsyncOperation op;
        if (playSceneIndex >= 0)
            op = SceneManager.LoadSceneAsync(playSceneIndex, LoadSceneMode.Single);
        else
            op = SceneManager.LoadSceneAsync(playSceneName, LoadSceneMode.Single);

        
        while (!op.isDone) yield return null;
    }

    private void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("buttonClick");
    }
 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExitPressed();
    }
}
