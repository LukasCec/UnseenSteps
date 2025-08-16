using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LighthouseRevealCircle : MonoBehaviour
{
    [Header("Circle Settings")]
    public int segments = 100;

    [Header("Reveal Mask Settings")]
    public SpriteMask spriteMask;
    public float revealRadius = 200f; // rovnaká metrika ako pri kurzore
    public float edgeScale = 1.57f;   // rovnaký multiplikátor ako používaš pri kurze

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (line != null) { line.positionCount = segments + 1; line.loop = true; }
        ApplyRadius();
    }

    public void SetRadius(float r)
    {
        revealRadius = r;
        ApplyRadius();
    }

    void ApplyRadius()
    {
        float radius = revealRadius / 100f;

        // nakresli kruh (rovnaké ako u teba)
        if (line != null)
        {
            float angleStep = 360f / segments;
            Vector3 center = transform.position;
            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.Deg2Rad * angleStep * i;
                Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
                line.SetPosition(i, center + point);
            }
        }

        // škálovanie masky – rovnaké ako u kurzora
        transform.localScale = Vector3.one * (radius * edgeScale);
    }
}