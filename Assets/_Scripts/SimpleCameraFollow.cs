using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;       // Hr·Ë
    public float smoothSpeed = 5f; // Vyhladenie pohybu
    public Vector3 offset;         // Posun kamery voËi hr·Ëovi

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10f; // Udrûaù kameru v spr·vnej hÂbke

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
