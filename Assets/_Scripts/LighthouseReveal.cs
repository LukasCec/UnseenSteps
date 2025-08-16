using UnityEngine;

public class LighthouseReveal : MonoBehaviour
{
    [Header("Reveal")]
    public LighthouseRevealCircle revealCircle; // prirad� die�a RevealCircle
    public float revealRadius = 200f;           // dosah maj�ka (kone�n�)
    public bool startsActive = false;

    [Header("Activation")]
    public bool activateOnPlayerEnter = true;   // jednoduch� aktiv�cia cez trigger
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
        // TODO: SFX/VFX/achievement, ak chce�
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
