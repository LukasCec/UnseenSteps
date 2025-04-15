using UnityEngine;

public class ParallaxLoop : MonoBehaviour
{
    public float parallaxMultiplier = 0.5f;
    private Transform cam;
    private float lengthX;
    private float startX;

    void Start()
    {
        cam = Camera.main.transform;
        startX = transform.position.x;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        float temp = cam.position.x * (1 - parallaxMultiplier);
        float dist = cam.position.x * parallaxMultiplier;

        // Posúvame objekt pod¾a multiplikátora
        transform.position = new Vector3(startX + dist, transform.position.y, transform.position.z);

        // Ak je kamera ïalej než segment, resetuj pozíciu
        if (temp > startX + lengthX)
        {
            startX += lengthX;
        }
        else if (temp < startX - lengthX)
        {
            startX -= lengthX;
        }
    }
}
