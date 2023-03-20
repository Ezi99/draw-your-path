using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute), true)]
    public class ReadOnlyFieldPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var readOnlyField = (ReadOnlyFieldAttribute)attribute;

            var enabled = GUI.enabled;
            var isReadOnly = readOnlyField.Option == ReadOnlyFieldOption.ReadOnlyAlways ||
                (EditorApplication.isPlaying && (readOnlyField.Option == ReadOnlyFieldOption.ReadOnlyInPlayMode));

            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(isReadOnly);

            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!enabled);
        }
    }
}
