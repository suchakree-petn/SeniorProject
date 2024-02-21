using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyGUIAttribute))]
public class ReadOnlyGUIDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Disable editing
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true; // Re-enable editing
    }
}