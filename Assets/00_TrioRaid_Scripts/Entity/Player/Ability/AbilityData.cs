using Sirenix.OdinInspector;
using UnityEngine;

[InlineEditor]
public class AbilityData : ScriptableObject
{
    public string Name;
    public string Description;
    public float Cooldown;

    [PreviewField(60)]
    public Sprite Icon;
}
