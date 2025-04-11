using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health = 5;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log("Player hit! HP: " + health);
        if (health <= 0)
        {
            Debug.Log("Player dead!");
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
