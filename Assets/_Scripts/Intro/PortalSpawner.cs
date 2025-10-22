using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Options")]
    [SerializeField] private bool spawnOnlyOnce = true;

    private GameObject spawned;

    public void Spawn()
    {
        if (!portalPrefab) { Debug.LogWarning("PortalSpawner: Missing portalPrefab."); return; }
        if (spawnOnlyOnce && spawned) return;

        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;
        spawned = Instantiate(portalPrefab, pos, rot);
    }
}
