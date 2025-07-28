using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    private SpriteRenderer sr;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        StartCoroutine(BlinkEffect());

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator BlinkEffect()
    {
        int blinkCount = 3;
        float blinkDuration = 0.1f;

        for (int i = 0; i < blinkCount; i++)
        {
            sr.color = new Color(1, 1, 1, 0.2f);  // troku priehadn
            yield return new WaitForSeconds(blinkDuration);

            sr.color = Color.white;              // vrtime pvodn farbu
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}
