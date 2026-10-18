using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        LabelAttribute labelAttribute =
            (LabelAttribute)attribute;

        EditorGUI.PropertyField(
            position,
            property,
            new GUIContent(labelAttribute.Name),
            true
        );
    }

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(
            property,
            label,
            true
        );
    }
}