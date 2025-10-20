using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Setup")]
    public Transform spawnPoint;          
    public GameObject tooltipUI;           
    public KeyCode interactKey = KeyCode.C;

    [Header("VFX/Audio/Anim")]
    public Animator animator;               
    public string activateTrigger = "Activate";

    bool playerInRange;
    bool isActivated;

    void Reset()
    {
        var sp = new GameObject("SpawnPoint");
        sp.transform.SetParent(transform);
        sp.transform.localPosition = Vector3.up * 0.5f;
        spawnPoint = sp.transform;
    }

    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (tooltipUI) tooltipUI.SetActive(false);
    }

    void Update()
    {
        if (isActivated)
        {
            if (tooltipUI && tooltipUI.activeSelf) tooltipUI.SetActive(false);
            return;
        }

        if (playerInRange)
        {
            if (tooltipUI && !tooltipUI.activeSelf) tooltipUI.SetActive(true);
            if (Input.GetKeyDown(interactKey)) Activate();
        }
        else if (tooltipUI && tooltipUI.activeSelf)
            tooltipUI.SetActive(false);
    }

    void Activate()
    {
        isActivated = true;
        if (tooltipUI) tooltipUI.SetActive(false);
        if (animator && !string.IsNullOrEmpty(activateTrigger)) animator.SetTrigger(activateTrigger);
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("checkpoint");

        CheckpointManager.Instance.SaveCheckpoint(this);
        Toast.Show("Saved!");
    }

    void OnTriggerEnter2D(Collider2D c) { if (c.CompareTag("Player")) playerInRange = true; }
    void OnTriggerStay2D(Collider2D c) { if (c.CompareTag("Player")) playerInRange = true; }
    void OnTriggerExit2D(Collider2D c) { if (c.CompareTag("Player")) playerInRange = false; }

    public Vector3 SpawnPos => (spawnPoint ? spawnPoint.position : transform.position);
}
