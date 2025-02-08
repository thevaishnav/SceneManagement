using Omnix.SceneManagement;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneArgs))]
public class ScenePropertiesDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty sceneProp = property.FindPropertyRelative("scene");
        if (sceneProp.enumValueIndex == 2)
            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position.height = EditorGUIUtility.singleLineHeight;

        float padding = 4f;
        float labelWidth = EditorGUIUtility.labelWidth;
        float fieldWidth = (position.width - labelWidth - padding) / 3f;

        Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
        position.x += labelWidth;
        position.width -= labelWidth;

        SerializedProperty sceneProp = property.FindPropertyRelative("scene");
        SerializedProperty modeProp = property.FindPropertyRelative("mode");
        SerializedProperty isAsyncProp = property.FindPropertyRelative("isAsync");

        Rect sceneRect = new Rect(position.x, position.y, fieldWidth, position.height);
        Rect modeRect = new Rect(sceneRect.xMax + padding, position.y, fieldWidth, position.height);
        Rect asyncRect = new Rect(modeRect.xMax + padding, position.y, labelWidth, position.height);

        EditorGUI.LabelField(labelRect, label);
        EditorGUI.PropertyField(sceneRect, sceneProp, GUIContent.none);
        EditorGUI.PropertyField(modeRect, modeProp, GUIContent.none);
        isAsyncProp.boolValue = EditorGUI.ToggleLeft(asyncRect, "Load Async", isAsyncProp.boolValue);
        if (sceneProp.enumValueIndex == 2)
        {
            var errorRect = new Rect(position);
            errorRect.y += errorRect.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.HelpBox(errorRect, "Scene `Unknown` is Invalid", MessageType.Error);
        }
        EditorGUI.EndProperty();
    }
}