using UnityEngine;

public class BossHealthbarTrigger : MonoBehaviour
{
    [Header("References")]
    public BossHealthBarUI bossHealthbarUI; 

    private void Start()
    {
        if (bossHealthbarUI != null)
            bossHealthbarUI.HideImmediate(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && bossHealthbarUI != null)
            bossHealthbarUI.Show();
    }

    
}
