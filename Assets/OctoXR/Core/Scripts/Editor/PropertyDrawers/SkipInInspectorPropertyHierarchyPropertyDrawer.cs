using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor
{
    [CustomPropertyDrawer(typeof(SkipInInspectorPropertyHierarchyAttribute), true)]
    public class SkipInInspectorPropertyHierarchyPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0f;

            foreach (var childProperty in CustomEditorUtility.GetVisibleChildProperties(property))
            {
                height += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            height -= EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var y = position.y;

            foreach (var childProperty in CustomEditorUtility.GetVisibleChildProperties(property))
            {
                var childHeight = EditorGUI.GetPropertyHeight(childProperty);

                position.Set(position.x, y, position.width, childHeight);

                EditorGUI.PropertyField(position, childProperty, true);

                y += childHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}
