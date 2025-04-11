using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CursorRevealCircle : MonoBehaviour
{
    public float radius = 3f;
    public int segments = 100;
    public float followSpeed = 10f;
    public float defaultSensitivity = 1f; // Default mouse sensitivity

    private LineRenderer line;
    private Vector3 smoothPosition;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.loop = true;
        smoothPosition = transform.position;
    }

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        float sensitivity = defaultSensitivity;
        
        
            
        

        smoothPosition = Vector3.Lerp(smoothPosition, mouseWorld, Time.deltaTime * followSpeed * sensitivity);

        transform.position = smoothPosition;

        DrawCircle(smoothPosition, radius);
    }

    void DrawCircle(Vector3 center, float radius)
    {
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            line.SetPosition(i, center + point);
        }
    }
}
