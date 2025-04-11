using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] points;
    public float speed = 2f;
    private int currentPointIndex = 0;

    void Update()
    {
        if (points.Length == 0) return;

        Transform target = points[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Length;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform); // hráè sa prilepí
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null); // hráè sa odlepí
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        if (points != null && points.Length > 1)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 from = points[i].position;
                Vector3 to = points[(i + 1) % points.Length].position;
                Gizmos.DrawLine(from, to);
            }
        }
    }
}
