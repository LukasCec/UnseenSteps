using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class MaskVisibilityManager : MonoBehaviour
{
    [Tooltip("Ak je zapnut�, objekty bud� vidite�n� vo vn�tri masky, inak bud� ignorova� masku.")]
    public bool Show = true;

    [Tooltip("Zoznam objektov, ktor� sa maj� prep�na�.")]
    public List<GameObject> targetObjects = new List<GameObject>();

    private void OnValidate()
    {
        UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        foreach (var obj in targetObjects)
        {
            if (obj == null) continue;

            // Pre SpriteRenderer
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.maskInteraction = Show ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None;
            }

            // Pre TilemapRenderer
            TilemapRenderer tr = obj.GetComponent<TilemapRenderer>();
            if (tr != null)
            {
                tr.maskInteraction = Show ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None;
            }
        }
    }

    // Volanie po�as hry
    private void Update()
    {
#if UNITY_EDITOR
        // Aby sa to aktualizovalo aj ke� men� bool v editore
        if (!Application.isPlaying)
        {
            UpdateVisibility();
            return;
        }
#endif

        // V runtime m��e� prepn�� aj cez skript alebo inspector
        UpdateVisibility();
    }
}
