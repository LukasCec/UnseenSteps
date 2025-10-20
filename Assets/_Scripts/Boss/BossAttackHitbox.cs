using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class BossAttackHitbox : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 2;
    public string targetTag = "Player";

    [Header("Knockback (optional)")]
    public float knockbackForce = 8f;
    public Vector2 knockbackDir = Vector2.right;

    private bool windowOpen = false;
    private readonly HashSet<GameObject> hitThisWindow = new HashSet<GameObject>();
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

   
    public void BeginWindow()
    {
        windowOpen = true;
        hitThisWindow.Clear();
    }

    public void EndWindow()
    {
        windowOpen = false;
        hitThisWindow.Clear();
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    void OnTriggerStay2D(Collider2D other) => TryHit(other);

    private void TryHit(Collider2D other)
    {
        if (!windowOpen) return;
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag)) return;

        var ph = other.GetComponent<PlayerHealth>()
                 ?? other.GetComponentInParent<PlayerHealth>()
                 ?? other.GetComponentInChildren<PlayerHealth>();
        if (ph == null) return;
        if (hitThisWindow.Contains(ph.gameObject)) return;

        ph.TakeDamage(damage, transform.position);
        hitThisWindow.Add(ph.gameObject);

        var rb = ph.GetComponent<Rigidbody2D>();
        if (rb && knockbackForce > 0f)
        {
            float dirSign = Mathf.Sign(transform.lossyScale.x);
            Vector2 dir = new Vector2(knockbackDir.x * dirSign, knockbackDir.y).normalized;
            rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
