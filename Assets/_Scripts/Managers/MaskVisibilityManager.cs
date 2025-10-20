using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class MaskVisibilityManager : MonoBehaviour
{
    [Tooltip("Ak je zapnut�, objekty bud� vidite�n� vo vn�tri masky, inak bud� ignorova� masku.")]
    public bool Show = true;

    [Tooltip("Rodi�ovsk� objekty (alebo samostatn�) ktor�ch renderery sa maj� prepn��.")]
    public List<GameObject> targetObjects = new List<GameObject>();

    // na detekciu zmeny v Editore (aby sme nemuseli uklada� sc�nu)
    private bool _lastShow;

    void OnEnable()
    {
        _lastShow = Show;
        UpdateVisibility(true);
    }

    void OnValidate()
    {
        // OnValidate sa vol� pri zmene v Inspectore � zareaguj hne�
        UpdateVisibility(true);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Ke� sa prepne checkbox Show v Inspectore, zachyt�me zmenu a refreshneme
            if (_lastShow != Show)
            {
                _lastShow = Show;
                UpdateVisibility(true);
            }
            return;
        }
#endif
        // Runtime: je to lacn�, m��eme refreshova� priebe�ne
        UpdateVisibility();
    }

    public void UpdateVisibility(bool forceEditorRefresh = false)
    {
        var mode = Show ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.None;

        foreach (var root in targetObjects)
        {
            if (!root) continue;

            // v�etky SpriteRenderery v de�och (aj neakt�vnych)
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

            // v�etky TilemapRenderery v de�och (aj neakt�vnych)
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
            // nech sa hne� prekresl� SceneView/GameView
            SceneView.RepaintAll();
            // volite�n�: ozna� sc�nu ako �dirty�, aby Unity spo�ahlivo serialize-ol zmeny bez Ctrl+S
            // EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }

    // prav� klik na komponent -> Refresh Visibility Now
    [ContextMenu("Refresh Visibility Now")]
    public void RefreshNow()
    {
        _lastShow = Show;
        UpdateVisibility(true);
    }
}
