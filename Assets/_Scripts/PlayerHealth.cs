using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealth = 5;    
    public int health = 5;        

    private SpriteRenderer sr;

    [Header("Knockback")]
    public float knockbackForce = 10f;
    private PlayerController controller;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();

       
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    
    public void TakeDamage(int dmg) => TakeDamage(dmg, transform.position);

    public void TakeDamage(int dmg, Vector2 attackerPosition)
    {
        if (health <= 0) return;

        health = Mathf.Clamp(health - dmg, 0, maxHealth);
        Debug.Log("Player hit! HP: " + health);

        Vector2 knockDirection = (transform.position - (Vector3)attackerPosition).normalized;
        controller.StartCoroutine(controller.Stun(0.2f));

        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockDirection * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(BlinkEffect());

        if (health <= 0)
        {
            Debug.Log("Player dead!");
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }

    
    public void Heal(int amount)
    {
        if (amount <= 0 || health <= 0) return;
        int old = health;
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        if (health != old)
        {
            
            //Debug.Log($"Healed to {health}/{maxHealth}");
        }
    }

    IEnumerator BlinkEffect()
    {
        int blinkCount = 3;
        float blinkDuration = 0.1f;

        for (int i = 0; i < blinkCount; i++)
        {
            sr.color = new Color(1, 1, 1, 0.2f);
            yield return new WaitForSeconds(blinkDuration);
            sr.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}
