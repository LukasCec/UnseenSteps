using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteUseRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture; // prira� RT_LowRes
    SpriteRenderer sr;
    MaterialPropertyBlock mpb;
    static readonly int MainTexID = Shader.PropertyToID("_MainTex");
    static readonly int BaseMapID = Shader.PropertyToID("_BaseMap"); // URP 2D pou��va _BaseMap

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (renderTexture == null) return;
        sr.GetPropertyBlock(mpb);
        // pre istotu nastav�me OBE � pod�a shaderu sa pou�ije jedna z nich
        mpb.SetTexture(MainTexID, renderTexture);
        mpb.SetTexture(BaseMapID, renderTexture);
        sr.SetPropertyBlock(mpb);
    }
}
