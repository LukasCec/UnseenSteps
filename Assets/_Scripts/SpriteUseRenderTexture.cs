using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteUseRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture; // priraï RT_LowRes
    SpriteRenderer sr;
    MaterialPropertyBlock mpb;
    static readonly int MainTexID = Shader.PropertyToID("_MainTex");
    static readonly int BaseMapID = Shader.PropertyToID("_BaseMap"); // URP 2D používa _BaseMap

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (renderTexture == null) return;
        sr.GetPropertyBlock(mpb);
        // pre istotu nastavíme OBE – pod¾a shaderu sa použije jedna z nich
        mpb.SetTexture(MainTexID, renderTexture);
        mpb.SetTexture(BaseMapID, renderTexture);
        sr.SetPropertyBlock(mpb);
    }
}
