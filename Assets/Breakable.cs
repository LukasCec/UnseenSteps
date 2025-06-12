using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Breakable : MonoBehaviour
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
    /// Volajte, keÔ objekt zasiahne hr·Ë.
    /// </summary>
    public void Hit()
    {
        currentStrength--;

        // 1) Blink efekt
        StartCoroutine(BlinkEffect());

        // 2) Shake efekt ñ zaznamen· si teraz aktu·lnu pozÌciu
        StartCoroutine(ShakeEffect());

        // 3) Ak ûivotnosù vyËerpan· -> rozbiù
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
        // zaznamen·me si s˙Ëasn˙ svetov˙ pozÌciu
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

        // vr·time presne na t˙ ist˙ pozÌciu
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

        // ZniËi samotn˝ GameObject
        Destroy(gameObject);
    }
}
