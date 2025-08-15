using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private SpriteRenderer sr;
    private Coroutine blinkCo;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int dmg)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - dmg);

        if (blinkCo != null) StopCoroutine(blinkCo);
        blinkCo = StartCoroutine(BlinkEffect());

        if (currentHealth == 0)
        {
            // prehráme náhodný death sfx 1 alebo 2
            if (AudioManager.Instance != null)
            {
                int n = Random.Range(1, 3); // 1 alebo 2
                AudioManager.Instance.PlaySFX($"enemyDeath{n}");
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator BlinkEffect()
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