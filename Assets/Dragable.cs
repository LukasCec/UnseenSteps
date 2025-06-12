using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Dragable : MonoBehaviour
{
    [Header("Drag Settings")]
    [Tooltip("Maximum distance from player to start dragging")]
    public float dragRange = 1.5f;

    private FixedJoint2D joint;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Pripojí tento objekt k hráèovi cez FixedJoint,
    /// prièom kotvový bod je práve worldAnchor.
    /// </summary>
    public void StartDrag(Rigidbody2D playerRb, Vector2 worldAnchor)
    {
        if (joint != null) return;

        joint = gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = playerRb;
        joint.autoConfigureConnectedAnchor = false;

        // prepoèítame svetový anchor do lokálnych súradníc oboch tiel
        Vector2 localAnchor = transform.InverseTransformPoint(worldAnchor);
        Vector2 connectedLocal = playerRb.transform.InverseTransformPoint(worldAnchor);

        joint.anchor = localAnchor;
        joint.connectedAnchor = connectedLocal;
    }

    public void EndDrag()
    {
        if (joint != null)
            Destroy(joint);
        joint = null;
    }

    public bool IsInRange(Vector2 playerPosition)
    {
        return Vector2.Distance(transform.position, playerPosition) <= dragRange;
    }
}
