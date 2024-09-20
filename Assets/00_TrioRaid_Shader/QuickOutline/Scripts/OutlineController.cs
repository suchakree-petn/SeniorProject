using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using static Outline;

public class OutlineController : MonoBehaviour
{
    [SerializeField]
    private Mode outlineMode;

    [SerializeField]
    private Color outlineColor = Color.white;

    [SerializeField, Range(0f, 20f)]
    private float outlineWidth = 2f;


    [FoldoutGroup("Reference")]
    [InlineEditor]
    [SerializeField] private Outline[] outlines;

    private bool isShowing;

    private void Awake()
    {
        outlines = transform.GetComponentsInChildren<Outline>();
        if (outlines.Length == 0)
        {
            enabled = false;
        }
        isShowing = outlines[0].enabled;
    }

    private void OnValidate()
    {

        if (!EditorApplication.isPlaying)
        {
            isShowing = outlines[0].enabled;

            outlines = transform.GetComponentsInChildren<Outline>();
            foreach (Outline outline in outlines)
            {
                outline.OutlineMode = outlineMode;
                outline.OutlineColor = outlineColor;
                outline.OutlineWidth = outlineWidth;
                outline.UpdateOutlineInEditor();
            }
            return;
        }

#if UNITY_EDITOR
        foreach (Outline outline in outlines)
        {
            outline.OutlineMode = outlineMode;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineWidth;
            outline.UpdateOutlineInEditor();
        }
#endif
    }


    [HideIf("isShowing")]
    [Button, GUIColor(0, 1, 0)]
    public void ShowOutline()
    {
        Debug.Log("show");

        isShowing = true;
        foreach (var outline in outlines)
        {
            outline.CreateOutline();
            outline.enabled = true;
        }
    }

    [ShowIf("isShowing")]
    [Button, GUIColor(1, 0, 0)]
    public void HideOutline()
    {
        Debug.Log("hide");
        isShowing = false;

        foreach (var outline in outlines)
        {
            outline.RemoveOutline();
            outline.enabled = false;
        }
    }
}
