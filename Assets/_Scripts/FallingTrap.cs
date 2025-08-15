using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    public Rigidbody2D stalactiteRb;
    public float fallForce = 5f;
    private bool hasFallen = false;

    void Start()
    {
        if (stalactiteRb != null)
        {
            stalactiteRb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasFallen) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trap trigger!");
            Drop();
        }
        else
        {
            Debug.Log("Something else entered: " + other.name);
        }
    }

    void Drop()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("spike");
        hasFallen = true;
        stalactiteRb.bodyType = RigidbodyType2D.Dynamic;
        stalactiteRb.gravityScale = 3f;
        stalactiteRb.AddForce(Vector2.down * fallForce, ForceMode2D.Impulse);
    }
}
