using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;       // Hr��
    public float smoothSpeed = 5f; // Vyhladenie pohybu
    public Vector3 offset;         // Posun kamery vo�i hr��ovi

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10f; // Udr�a� kameru v spr�vnej h�bke

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
