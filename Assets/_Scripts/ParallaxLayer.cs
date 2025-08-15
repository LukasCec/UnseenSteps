using UnityEngine;

[ExecuteAlways]
public class ParallaxLayer : MonoBehaviour
{
    public Transform cameraTransform;
    [Range(0f, 1f)] public float xMultiplier = 0.5f; 
    [Range(0f, 1f)] public float yMultiplier = 0f;   
    public bool lockStartY = true;                   

    Vector3 _startPos;
    Vector3 _startCamPos;

    void OnEnable()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main ? Camera.main.transform : null;

        _startPos = transform.position;
        _startCamPos = cameraTransform ? cameraTransform.position : Vector3.zero;
    }

    void LateUpdate()
    {
        if (!cameraTransform) return;

        Vector3 camDelta = cameraTransform.position - _startCamPos;

        float x = _startPos.x + camDelta.x * xMultiplier;
        float y = lockStartY ? _startPos.y : _startPos.y + camDelta.y * yMultiplier;

        // bez zmeny Z (zostáva pod¾a hierarchie/sortingu)
        transform.position = new Vector3(x, y, _startPos.z);
    }
}
