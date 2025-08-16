using UnityEngine;

public class LighthouseReveal : MonoBehaviour
{
    [Header("Reveal")]
    public LighthouseRevealCircle revealCircle; // priradÌö dieùa RevealCircle
    public float revealRadius = 200f;           // dosah maj·ka (koneËn˝)
    public bool startsActive = false;

    [Header("Activation")]
    public bool activateOnPlayerEnter = true;   // jednoduch· aktiv·cia cez trigger
    public bool oneShot = true;

    bool isActive;

    void Start()
    {
        isActive = startsActive;
        ApplyState();
    }

    public void Activate()
    {
        if (isActive && oneShot) return;
        isActive = true;
        ApplyState();
        // TODO: SFX/VFX/achievement, ak chceö
    }

    public void Deactivate()
    {
        isActive = false;
        ApplyState();
    }

    void ApplyState()
    {
        if (revealCircle != null)
        {
            revealCircle.gameObject.SetActive(isActive);
            revealCircle.SetRadius(revealRadius);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!activateOnPlayerEnter) return;
        if (other.CompareTag("Player")) Activate();
    }
}
