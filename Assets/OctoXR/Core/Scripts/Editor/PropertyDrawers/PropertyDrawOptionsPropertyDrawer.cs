using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor
{
    [CustomPropertyDrawer(typeof(PropertyDrawOptionsAttribute), true)]
    public class PropertyDrawOptionsPropertyDrawer : PropertyDrawer
    {
        private static readonly MethodInfo propertyHandlerCacheGetter;
        private static readonly MethodInfo getPropertyHandlerFromCacheMethod;
        private static readonly MethodInfo setPropertyHandlerInCacheMethod;
#if UNITY_2021_1_OR_NEWER
        private static readonly FieldInfo propertyHandlerPropertyDrawersField;
#else
        private static readonly FieldInfo propertyHandlerPropertyDrawerField;
#endif

        private static readonly HashSet<object> configuredPropertyHandlers = new HashSet<object>();

        static PropertyDrawOptionsPropertyDrawer()
        {
            try
            {
                var scriptAttributeUtilityType = typeof(EditorGUI).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                var propertyHandlerCacheProperty = scriptAttributeUtilityType.GetProperty(
                    "propertyHandlerCache",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

                propertyHandlerCacheGetter = propertyHandlerCacheProperty.GetMethod;
                getPropertyHandlerFromCacheMethod = propertyHandlerCacheProperty.PropertyType.GetMethod(
                     "GetHandler",
                     BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                setPropertyHandlerInCacheMethod = propertyHandlerCacheProperty.PropertyType.GetMethod(
                     "SetHandler",
                     BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                var propertyHandlerType = typeof(EditorGUI).Assembly.GetType("UnityEditor.PropertyHandler");

#if UNITY_2021_1_OR_NEWER
                propertyHandlerPropertyDrawersField = propertyHandlerType.GetField(
                    "m_PropertyDrawers", BindingFlags.NonPublic | BindingFlags.Instance);
#else
                propertyHandlerPropertyDrawerField = propertyHandlerType.GetField(
                    "m_PropertyDrawer", BindingFlags.NonPublic | BindingFlags.Instance);
#endif
            }
            catch
            {
            }
        }

        private SkipInInspectorPropertyHierarchyPropertyDrawer skipInInspectorPropertyHierarchyPropertyDrawer;
        private SetValueViaPropertyOrMethodPropertyDrawer setValueViaPropertyOrMethodPropertyDrawer;

        private void CreateSkipInspectorPropertyHierarchyPropertyDrawer()
        {
            if (skipInInspectorPropertyHierarchyPropertyDrawer == null)
            {
                var skipInInspectorPropertyHierarchyAttribute = new SkipInInspectorPropertyHierarchyAttribute();

                skipInInspectorPropertyHierarchyPropertyDrawer =
                    CustomEditorUtility.CreatePropertyDrawer<SkipInInspectorPropertyHierarchyPropertyDrawer>(
                        skipInInspectorPropertyHierarchyAttribute, fieldInfo);
            }
        }

        private void CreateSetValueViaPropertyOrMethodPropertyDrawer(PropertyDrawOptionsAttribute propertyDrawOptions)
        {
            if (setValueViaPropertyOrMethodPropertyDrawer == null)
            {
                var setValueViaPropertyOrMethodAttribute = new SetValueViaPropertyOrMethodAttribute
                {
                    PropertyOrMethodName = propertyDrawOptions.SetValueViaPropertyOrMethodName
                };

                setValueViaPropertyOrMethodPropertyDrawer =
                    CustomEditorUtility.CreatePropertyDrawer<SetValueViaPropertyOrMethodPropertyDrawer>(
                        setValueViaPropertyOrMethodAttribute, fieldInfo);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyDrawOptions = (PropertyDrawOptionsAttribute)attribute;

            if (propertyDrawOptions.IsCustomCollection)
            {
                var backingArrayOrListProperty = property.FindPropertyRelative(propertyDrawOptions.CustomCollectionBackingArrayOrListFieldPath);

                SetInternalPropertyHandlerForCustomCollectionOptions(backingArrayOrListProperty, property, propertyDrawOptions);

                return EditorGUI.GetPropertyHeight(backingArrayOrListProperty, label, true);
            }
            else if (propertyDrawOptions.SkipInInspectorPropertyHierarchy)
            {
                CreateSkipInspectorPropertyHierarchyPropertyDrawer();

                return skipInInspectorPropertyHierarchyPropertyDrawer.GetPropertyHeight(property, label);
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyDrawOptions = (PropertyDrawOptionsAttribute)attribute;

            if (!propertyDrawOptions.IsCustomCollection && !propertyDrawOptions.SkipInInspectorPropertyHierarchy)
            {
                label.text = LabelPropertyDrawer.GetPropertyLabelText(property, label, propertyDrawOptions);
            }

            bool isReadOnly;

            var enabled = GUI.enabled;

            if (propertyDrawOptions.ReadOnly == ReadOnlyPropertyDrawOptions.InheritedFromParent)
            {
                isReadOnly = !enabled;
            }
            else
            {
                isReadOnly = (propertyDrawOptions.ReadOnly & ReadOnlyPropertyDrawOptions.ReadOnlyAlways) == ReadOnlyPropertyDrawOptions.ReadOnlyAlways ||
                    (EditorApplication.isPlaying && (propertyDrawOptions.ReadOnly & ReadOnlyPropertyDrawOptions.ReadOnlyInPlayMode) != 0);
            }

            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(isReadOnly);

            if (propertyDrawOptions.IsCustomCollection)
            {
                var backingArrayOrListProperty = property.FindPropertyRelative(propertyDrawOptions.CustomCollectionBackingArrayOrListFieldPath);

                EditorGUI.PropertyField(position, backingArrayOrListProperty, label, true);
            }
            else if (propertyDrawOptions.SkipInInspectorPropertyHierarchy)
            {
                CreateSkipInspectorPropertyHierarchyPropertyDrawer();

                skipInInspectorPropertyHierarchyPropertyDrawer.OnGUI(position, property, label);
            }
            else if (propertyDrawOptions.SetValueViaPropertyOrMethod && !isReadOnly)
            {
                CreateSetValueViaPropertyOrMethodPropertyDrawer(propertyDrawOptions);

                setValueViaPropertyOrMethodPropertyDrawer.OnGUI(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!enabled);
        }

        private void SetInternalPropertyHandlerForCustomCollectionOptions(
            SerializedProperty backingArrayOrListProperty,
            SerializedProperty customCollectionProperty,
            PropertyDrawOptionsAttribute propertyDrawOptions)
        {
            if (backingArrayOrListProperty == null || !backingArrayOrListProperty.isArray)
            {
                Debug.LogErrorFormat("Cannot draw serialized field as custom collection because field that serves as backing array " +
                    "was not found at path '{0}' or it is not array or List<T>",
                    propertyDrawOptions.CustomCollectionBackingArrayOrListFieldPath);

                return;
            }

            if (backingArrayOrListProperty.arraySize == 0)
            {
                return;
            }

            var arrayItemPropertyDrawAttribute = propertyDrawOptions.Clone();

            arrayItemPropertyDrawAttribute.ReadOnly = propertyDrawOptions.CustomCollectionItemsReadOnly;
            arrayItemPropertyDrawAttribute.IsCustomCollection = false;
            arrayItemPropertyDrawAttribute.CustomCollectionBackingArrayOrListFieldPath = null;
            arrayItemPropertyDrawAttribute.CustomCollectionItemsReadOnly = default;

            var arrayItemFieldInfo = GetCustomCollectionBackingArrayOrListFieldInfo(backingArrayOrListProperty, customCollectionProperty);
            var arrayItemDrawer = CustomEditorUtility.CreatePropertyDrawer<PropertyDrawOptionsPropertyDrawer>(
                arrayItemPropertyDrawAttribute,
                arrayItemFieldInfo);

            for (var i = 0; i < backingArrayOrListProperty.arraySize; i++)
            {
                var arrayItemProperty = backingArrayOrListProperty.GetArrayElementAtIndex(i);

                SetInternalPropertyHandlerForCustomCollectionArrayItem(arrayItemProperty, arrayItemDrawer);

                if (arrayItemProperty.propertyType != SerializedPropertyType.ObjectReference &&
                    arrayItemProperty.propertyType != SerializedPropertyType.ManagedReference)
                {
                    break;
                }
            }
        }

        private static void SetInternalPropertyHandlerForCustomCollectionArrayItem(
            SerializedProperty arrayItemProperty,
            PropertyDrawOptionsPropertyDrawer propertyDrawer)
        {
            try
            {
                var propertyHandlerCache = propertyHandlerCacheGetter.Invoke(null, Array.Empty<object>());
                var propertyHandler = getPropertyHandlerFromCacheMethod.Invoke(propertyHandlerCache, new object[] { arrayItemProperty });

                if (propertyHandler == null)
                {
                    propertyHandler = Activator.CreateInstance(getPropertyHandlerFromCacheMethod.ReturnType);
                }
                else if (configuredPropertyHandlers.Contains(propertyHandler))
                {
                    return;
                }
#if UNITY_2021_1_OR_NEWER
                var propertyHandlerPropertyDrawers = (IList)propertyHandlerPropertyDrawersField.GetValue(propertyHandler);

                if (propertyHandlerPropertyDrawers == null)
                {
                    propertyHandlerPropertyDrawers = Activator.CreateInstance(propertyHandlerPropertyDrawersField.FieldType) as IList;
                    propertyHandlerPropertyDrawersField.SetValue(propertyHandler, propertyHandlerPropertyDrawers);
                }

                propertyHandlerPropertyDrawers.Insert(0, propertyDrawer);
#else
                propertyHandlerPropertyDrawerField.SetValue(propertyHandler, propertyDrawer);
#endif
                setPropertyHandlerInCacheMethod.Invoke(propertyHandlerCache, new object[] { arrayItemProperty, propertyHandler });

                configuredPropertyHandlers.Add(propertyHandler);
            }
            catch
            {
            }
        }

        private FieldInfo GetCustomCollectionBackingArrayOrListFieldInfo(SerializedProperty backingArrayOrListProperty, SerializedProperty customCollectionProperty)
        {
            var fieldPath = backingArrayOrListProperty.propertyPath.Remove(0, customCollectionProperty.propertyPath.Length + 1).Split('.');
            var fieldInfo = base.fieldInfo;

            for (var i = 0; i < fieldPath.Length; i++)
            {
                var fieldName = fieldPath[i];

                if (fieldName.StartsWith("data[", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                fieldInfo = GetPrivateOrPublicInstanceFieldFromTypeOrParentTypes(fieldInfo.FieldType, fieldName);
            }

            return fieldInfo;
        }

        private static FieldInfo GetPrivateOrPublicInstanceFieldFromTypeOrParentTypes(Type type, string fieldName)
        {
            FieldInfo field = null;

            while (type != null && field == null)
            {
                field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                type = type.BaseType;
            }

            return field;
        }
    }
}
