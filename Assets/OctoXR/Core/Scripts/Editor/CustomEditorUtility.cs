using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor
{
    public static class CustomEditorUtility
    {
        public const string InspectorScriptPropertyPath = "m_Script";

        public static readonly FieldInfo PropertyDrawerAttributeField =
            typeof(PropertyDrawer).GetField("m_Attribute", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly FieldInfo PropertyDrawerFieldInfoField =
            typeof(PropertyDrawer).GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);

        public static IEnumerable<SerializedProperty> GetVisibleDeepChildProperties(SerializedProperty rootProperty)
        {
            var childProperties = new List<SerializedProperty>();
            var property = rootProperty.Copy();
            var endProperty = property.GetEndProperty(false);

            while (true)
            {
                if (property.NextVisible(true) && !SerializedProperty.EqualContents(property, endProperty))
                {
                    childProperties.Add(property.Copy());

                    continue;
                }

                break;
            }

            return childProperties;
        }

        public static IEnumerable<SerializedProperty> GetVisibleChildProperties(SerializedProperty rootProperty)
        {
            var childProperties = new List<SerializedProperty>();
            var property = rootProperty.Copy();
            var endProperty = property.GetEndProperty(false);

            if (property.NextVisible(true) && !SerializedProperty.EqualContents(property, endProperty))
            {
                childProperties.Add(property.Copy());
            }
            else
            {
                return childProperties;
            }

            while (true)
            {
                if (property.NextVisible(false) && !SerializedProperty.EqualContents(property, endProperty))
                {
                    childProperties.Add(property.Copy());

                    continue;
                }

                break;
            }

            return childProperties;
        }

        public static string GetHandBoneLabelText(HandBoneId boneId) => StringUtility.GetSpaceSeparatedString(boneId.ToString(), true);

        public static void DrawInspectorScriptProperty(SerializedObject serializedObject)
        {
            var scriptProperty = serializedObject.FindProperty(InspectorScriptPropertyPath);

            if (scriptProperty != null)
            {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.PropertyField(scriptProperty);

                EditorGUI.EndDisabledGroup();
            }
        }

        public static T CreatePropertyDrawer<T>(PropertyAttribute attribute, FieldInfo fieldInfo) where T : PropertyDrawer, new()
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var propertyDrawer = new T();

            PropertyDrawerAttributeField.SetValue(propertyDrawer, attribute);
            PropertyDrawerFieldInfoField.SetValue(propertyDrawer, fieldInfo);

            return propertyDrawer;
        }
    }
}
