using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene To Load")]
    [Tooltip("Názov alebo index scény, ktorá sa má spusti po Play.")]
    public string playSceneName = "Level1";
    public int playSceneIndex = -1;

    [Header("Optional: Fade/Loading")]
    public Animator fadeAnimator;          
    public float fadeDuration = 0.5f;      

    [Header("Optional: SFX")]
    public AudioSource uiClickSfx;

   
    public void OnPlayPressed()
    {
        PlayClick();
        StartCoroutine(LoadGame());
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
        if (uiClickSfx != null) uiClickSfx.Play();
    }

 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExitPressed();
    }
}
