using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class MaskVisibilityManager : MonoBehaviour
{
    [Tooltip("Ak je zapnuté, objekty budú vidite¾né vo vnútri masky, inak budú ignorova masku.")]
    public bool Show = true;

    [Tooltip("Rodièovské objekty (alebo samostatné) ktorıch renderery sa majú prepnú.")]
    public List<GameObject> targetObjects = new List<GameObject>();

    // na detekciu zmeny v Editore (aby sme nemuseli uklada scénu)
    private bool _lastShow;

    void OnEnable()
    {
        _lastShow = Show;
        UpdateVisibility(true);
    }

    void OnValidate()
    {
        // OnValidate sa volá pri zmene v Inspectore – zareaguj hneï
        UpdateVisibility(true);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Keï sa prepne checkbox Show v Inspectore, zachytíme zmenu a refreshneme
            if (_lastShow != Show)
            {
                _lastShow = Show;
                UpdateVisibility(true);
            }
            return;
        }
#endif
        // Runtime: je to lacné, môeme refreshova priebene
        UpdateVisibility();
    }

    public void UpdateVisibility(bool forceEditorRefresh = false)
    {
        var mode = Show ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None;

        foreach (var root in targetObjects)
        {
            if (!root) continue;

            // všetky SpriteRenderery v deoch (aj neaktívnych)
            var spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                var r = spriteRenderers[i];
                if (r == null) continue;
                r.maskInteraction = mode;

#if UNITY_EDITOR
                if (forceEditorRefresh)
                    EditorUtility.SetDirty(r);
#endif
            }

            // všetky TilemapRenderery v deoch (aj neaktívnych)
            var tilemapRenderers = root.GetComponentsInChildren<TilemapRenderer>(true);
            for (int i = 0; i < tilemapRenderers.Length; i++)
            {
                var r = tilemapRenderers[i];
                if (r == null) continue;
                r.maskInteraction = mode;

#if UNITY_EDITOR
                if (forceEditorRefresh)
                    EditorUtility.SetDirty(r);
#endif
            }
        }

#if UNITY_EDITOR
        if (forceEditorRefresh)
        {
            // nech sa hneï prekreslí SceneView/GameView
            SceneView.RepaintAll();
            // volite¾né: oznaè scénu ako „dirty“, aby Unity spo¾ahlivo serialize-ol zmeny bez Ctrl+S
            // EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }

    // pravı klik na komponent -> Refresh Visibility Now
    [ContextMenu("Refresh Visibility Now")]
    public void RefreshNow()
    {
        _lastShow = Show;
        UpdateVisibility(true);
    }
}
