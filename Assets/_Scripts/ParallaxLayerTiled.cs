using UnityEngine;

[DefaultExecutionOrder(1000)]
public class ParallaxLayerTiledSmooth : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;      // ak nech� pr�zdne, vezme MainCamera
    public Transform playerTransform;      // (volite�n�) pre jemn� Y nasledovanie hr��a
    public Transform left, mid, right;     // segmenty: left/mid/right

    [Header("Parallax (strength)")]
    [Range(0f, 2f)] public float xMultiplier = 0.5f;
    [Range(0f, 2f)] public float yMultiplier = 0f;
    public bool usePlayerY = true;         // ak chce�, aby Y bral z hr��a (pri p�de p�sob� lep�ie)
    public bool lockStartY = false;        // ak true, Y sa v�bec nemen�

    [Header("Smoothing (seconds)")]
    [Min(0f)] public float xDamping = 0.10f;  // men�ie = r�chlej�ie
    [Min(0f)] public float yDamping = 0.25f;

    [Header("Pixel Art Helpers")]
    public bool snapToPixelGrid = true;
    public float pixelsPerUnit = 32f;

    [Header("Auto layout")]
    public bool autoArrange = true;        // rozlo�� left/mid/right na -W,0,+W

    // intern�
    float _segmentWidth;
    Vector3 _startCamPos, _startPos;
    float _velX, _velY;                    // SmoothDamp r�chlosti

    void OnEnable()
    {
        if (!cameraTransform)
        {
            var cam = Camera.main;
            if (cam) cameraTransform = cam.transform;
        }

        // ��rka segmentu (z hociktor�ho SpriteRenderer-a)
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

        // --- 1) CIE�OV� poz�cia vrstvy (pod�a rozdielu kamery/hr��a od �tartu) ---
        var refY = (usePlayerY && playerTransform ? playerTransform.position.y
                                                  : cameraTransform.position.y);

        float targetX = _startPos.x + (cameraTransform.position.x - _startCamPos.x) * xMultiplier;
        float targetY = lockStartY
            ? _startPos.y
            : _startPos.y + (refY - _startCamPos.y) * yMultiplier;

        // --- 2) SMOOTH: plynul� dobiehanie na cie� (SmoothDamp) ---
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref _velX, xDamping);
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref _velY, yDamping);

        if (snapToPixelGrid && pixelsPerUnit > 0f)
        {
            newX = Mathf.Round(newX * pixelsPerUnit) / pixelsPerUnit;
            newY = Mathf.Round(newY * pixelsPerUnit) / pixelsPerUnit;
        }
        transform.position = new Vector3(newX, newY, transform.position.z);

        // --- 3) Nekone�n� cyklenie segmentov pod�a SKUTO�NEJ kamery (bez smoothingu) ---
        if (_segmentWidth <= 0f || !left || !mid || !right) return;

        // zisti krajn� segmenty
        Transform a = left, b = mid, c = right;
        Transform leftmost = a, rightmost = a;
        if (b.position.x < leftmost.position.x) leftmost = b;
        if (c.position.x < leftmost.position.x) leftmost = c;
        if (b.position.x > rightmost.position.x) rightmost = b;
        if (c.position.x > rightmost.position.x) rightmost = c;

        float camX = cameraTransform.position.x;

        if (camX > rightmost.position.x)
        {
            // presu� �plne �av� za prav�
            var p = leftmost.position;
            p.x = rightmost.position.x + _segmentWidth;
            if (snapToPixelGrid && pixelsPerUnit > 0f)
                p.x = Mathf.Round(p.x * pixelsPerUnit) / pixelsPerUnit;
            leftmost.position = p;
        }
        else if (camX < leftmost.position.x)
        {
            // presu� �plne prav� pred �av�
            var p = rightmost.position;
            p.x = leftmost.position.x - _segmentWidth;
            if (snapToPixelGrid && pixelsPerUnit > 0f)
                p.x = Mathf.Round(p.x * pixelsPerUnit) / pixelsPerUnit;
            rightmost.position = p;
        }
    }
}
