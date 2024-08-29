using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

    private void OnValidate()
    {
#if UNITY_EDITOR
        outlines = transform.GetComponentsInChildren<Outline>();

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
    [Button,GUIColor(0,1,0)]
    public void ShowOutline()
    {
        isShowing = true;

        foreach (var outline in outlines)
        {
            outline.CreateOutline();
        }
    }

    [ShowIf("isShowing")]
    [Button,GUIColor(1,0,0)]
    public void HideOutline()
    {
        isShowing = false;

        foreach (var outline in outlines)
        {
            outline.RemoveOutline();
        }
    }
}
