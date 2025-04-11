using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health = 5;
    private SpriteRenderer sr;
    [Header("Knockback")]
    public float knockbackForce = 10f;
    private PlayerController controller;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); // alebo z child objektu ak sprite nie je priamo na root
        controller = GetComponent<PlayerController>();
    }

    public void TakeDamage(int dmg, Vector2 attackerPosition)
    {
        health -= dmg;
        Debug.Log("Player hit! HP: " + health);
        Vector2 knockDirection = (transform.position - (Vector3)attackerPosition).normalized;
        controller.StartCoroutine(controller.Stun(0.2f));
        

        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // reset r�chlosti
        GetComponent<Rigidbody2D>().AddForce(knockDirection * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(BlinkEffect());
        if (health <= 0)
        {
            Debug.Log("Player dead!");
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

    }

    IEnumerator BlinkEffect()
    {
        int blinkCount = 3;
        float blinkDuration = 0.1f;

        for (int i = 0; i < blinkCount; i++)
        {
            sr.color = new Color(1, 1, 1, 0.2f); // prieh�adn�
            yield return new WaitForSeconds(blinkDuration);
            sr.color = Color.white; // sp� na norm�l
            yield return new WaitForSeconds(blinkDuration);
        }
    }

   

}
