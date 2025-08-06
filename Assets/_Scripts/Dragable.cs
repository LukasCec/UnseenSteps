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
    /// Pripoj� tento objekt k hr��ovi cez FixedJoint,
    /// pri�om kotvov� bod je pr�ve worldAnchor.
    /// </summary>
    public void StartDrag(Rigidbody2D playerRb, Vector2 worldAnchor)
    {
        if (joint != null) return;

        joint = gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = playerRb;
        joint.autoConfigureConnectedAnchor = false;

        // prepo��tame svetov� anchor do lok�lnych s�radn�c oboch tiel
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
