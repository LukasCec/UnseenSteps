using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(SpriteRenderer))]
public class FullscreenSpriteFitCamera : MonoBehaviour
{
    public Camera cam; public float pixelsPerUnit = 100f;
    void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        var sr = GetComponent<SpriteRenderer>(); if (!sr || !sr.sprite || !cam) return;
        float h = cam.orthographicSize * 2f, w = h * cam.aspect;
        Vector2 sp = sr.sprite.rect.size / pixelsPerUnit;
        transform.localScale = new Vector3(w / sp.x, h / sp.y, 1f);
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + 1f);
    }
}
