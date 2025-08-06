using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Breakable : MonoBehaviour, IDamageable
{
    [Header("Durability")]
    [SerializeField] private int strength = 3;
    private int currentStrength;

    [Header("Blink Settings")]
    [SerializeField] private int blinkCount = 2;
    [SerializeField] private float blinkDuration = 0.1f;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("Particles")]
    [Tooltip("ParticleSystem prefab for destruction effect")]
    [SerializeField] private ParticleSystem destructionParticles;

    private SpriteRenderer sr;

    void Awake()
    {
        currentStrength = strength;
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called when the object takes damage.
    /// </summary>
    public void TakeDamage(int dmg)
    {
        currentStrength -= dmg;

        // 1) Blink efekt
        StartCoroutine(BlinkEffect());

        // 2) Shake efekt  zaznamen si teraz aktulnu pozciu
        StartCoroutine(ShakeEffect());

        // 3) Ak ivotnos vyerpan -> rozbi
        if (currentStrength <= 0)
            Break();
    }

    private IEnumerator BlinkEffect()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            sr.color = new Color(1f, 1f, 1f, 0.2f);
            yield return new WaitForSeconds(blinkDuration);
            sr.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    private IEnumerator ShakeEffect()
    {
        // zaznamenme si sasn svetov pozciu
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.position = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // vrtime presne na t ist pozciu
        transform.position = originalPos;
    }

    private void Break()
    {
        // Destruction particles
        if (destructionParticles != null)
        {
            var ps = Instantiate(
                destructionParticles,
                transform.position,
                Quaternion.identity
            );
            Destroy(
                ps.gameObject,
                ps.main.duration + ps.main.startLifetime.constantMax
            );
        }

        // Znii samotn GameObject
        Destroy(gameObject);
    }
}
