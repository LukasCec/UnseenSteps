using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class ResetOnTrigger : MonoBehaviour
{
    void Awake()
    {
        var bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (CheckpointManager.Instance != null)
                CheckpointManager.Instance.RespawnPlayer();
        }
    }
}
