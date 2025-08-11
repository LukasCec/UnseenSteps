using UnityEngine;

[DefaultExecutionOrder(1000)]
public class ParallaxLayerTiledSmooth : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;      // ak necháš prázdne, vezme MainCamera
    public Transform playerTransform;      // (volite¾né) pre jemné Y nasledovanie hráèa
    public Transform left, mid, right;     // segmenty: left/mid/right

    [Header("Parallax (strength)")]
    [Range(0f, 2f)] public float xMultiplier = 0.5f;
    [Range(0f, 2f)] public float yMultiplier = 0f;
    public bool usePlayerY = true;         // ak chceš, aby Y bral z hráèa (pri páde pôsobí lepšie)
    public bool lockStartY = false;        // ak true, Y sa vôbec nemení

    [Header("Smoothing (seconds)")]
    [Min(0f)] public float xDamping = 0.10f;  // menšie = rýchlejšie
    [Min(0f)] public float yDamping = 0.25f;

    [Header("Pixel Art Helpers")]
    public bool snapToPixelGrid = true;
    public float pixelsPerUnit = 32f;

    [Header("Auto layout")]
    public bool autoArrange = true;        // rozloží left/mid/right na -W,0,+W

    // interné
    float _segmentWidth;
    Vector3 _startCamPos, _startPos;
    float _velX, _velY;                    // SmoothDamp rýchlosti

    void OnEnable()
    {
        if (!cameraTransform)
        {
            var cam = Camera.main;
            if (cam) cameraTransform = cam.transform;
        }

        // šírka segmentu (z hociktorého SpriteRenderer-a)
        var sr = mid ? mid.GetComponent<SpriteRenderer>() : null;
        if (!sr && left) sr = left.GetComponent<SpriteRenderer>();
        if (!sr && right) sr = right.GetComponent<SpriteRenderer>();
        if (sr) _segmentWidth = sr.bounds.size.x;

        if (autoArrange && _segmentWidth > 0f && left && mid && right)
        {
            left.position = new Vector3(mid.position.x - _segmentWidth, mid.position.y, left.position.z);
            right.position = new Vector3(mid.position.x + _segmentWidth, mid.position.y, right.position.z);
        }

        _startCamPos = cameraTransform ? cameraTransform.position : Vector3.zero;
        _startPos = transform.position;
        _velX = _velY = 0f;
    }

    void LateUpdate()
    {
        if (!cameraTransform) return;

        // --- 1) CIE¼OVÁ pozícia vrstvy (pod¾a rozdielu kamery/hráèa od štartu) ---
        var refY = (usePlayerY && playerTransform ? playerTransform.position.y
                                                  : cameraTransform.position.y);

        float targetX = _startPos.x + (cameraTransform.position.x - _startCamPos.x) * xMultiplier;
        float targetY = lockStartY
            ? _startPos.y
            : _startPos.y + (refY - _startCamPos.y) * yMultiplier;

        // --- 2) SMOOTH: plynulé dobiehanie na cie¾ (SmoothDamp) ---
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref _velX, xDamping);
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref _velY, yDamping);

        if (snapToPixelGrid && pixelsPerUnit > 0f)
        {
            newX = Mathf.Round(newX * pixelsPerUnit) / pixelsPerUnit;
            newY = Mathf.Round(newY * pixelsPerUnit) / pixelsPerUnit;
        }
        transform.position = new Vector3(newX, newY, transform.position.z);

        // --- 3) Nekoneèné cyklenie segmentov pod¾a SKUTOÈNEJ kamery (bez smoothingu) ---
        if (_segmentWidth <= 0f || !left || !mid || !right) return;

        // zisti krajné segmenty
        Transform a = left, b = mid, c = right;
        Transform leftmost = a, rightmost = a;
        if (b.position.x < leftmost.position.x) leftmost = b;
        if (c.position.x < leftmost.position.x) leftmost = c;
        if (b.position.x > rightmost.position.x) rightmost = b;
        if (c.position.x > rightmost.position.x) rightmost = c;

        float camX = cameraTransform.position.x;

        if (camX > rightmost.position.x)
        {
            // presuò úplne ¾avý za pravý
            var p = leftmost.position;
            p.x = rightmost.position.x + _segmentWidth;
            if (snapToPixelGrid && pixelsPerUnit > 0f)
                p.x = Mathf.Round(p.x * pixelsPerUnit) / pixelsPerUnit;
            leftmost.position = p;
        }
        else if (camX < leftmost.position.x)
        {
            // presuò úplne pravý pred ¾avý
            var p = rightmost.position;
            p.x = leftmost.position.x - _segmentWidth;
            if (snapToPixelGrid && pixelsPerUnit > 0f)
                p.x = Mathf.Round(p.x * pixelsPerUnit) / pixelsPerUnit;
            rightmost.position = p;
        }
    }
}
