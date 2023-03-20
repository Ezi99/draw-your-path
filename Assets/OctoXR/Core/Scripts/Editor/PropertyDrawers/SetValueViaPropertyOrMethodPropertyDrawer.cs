using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor
{
    [CustomPropertyDrawer(typeof(SetValueViaPropertyOrMethodAttribute), true)]
    public class SetValueViaPropertyOrMethodPropertyDrawer : PropertyDrawer
    {
        private struct FieldData
        {
            public string Name;
            public Type Type;
            public FieldInfo FieldInfo;
            public int Index;
            public bool IsContainedInArrayOrList;

            public object GetValue(object containerObjectValue)
            {
                if (containerObjectValue == null)
                {
                    return null;
                }

                if (IsContainedInArrayOrList)
                {
                    return ((IList)containerObjectValue)[Index];
                }
                else
                {
                    return FieldInfo.GetValue(containerObjectValue);
                }
            }

            public void SetValue(object containerObjectValue, object value)
            {
                if (containerObjectValue == null)
                {
                    return;
                }

                if (IsContainedInArrayOrList)
                {
                    ((IList)containerObjectValue)[Index] = value;
                }
                else
                {
                    FieldInfo.SetValue(containerObjectValue, value);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var setValueViaPropertyOrMethod = (SetValueViaPropertyOrMethodAttribute)attribute;

            if (GUI.enabled)
            {
                var baseFieldPath = GetFieldPathFromSerializedPropertyPath(property.propertyPath);
                var fieldPaths = GetDataEnrichedFieldPathFromBasePath(baseFieldPath, property);

                var fieldValueSetChainStartIndices = GetValueSetChainStartFieldIndices(fieldPaths);

                var fieldChainOriginalValues = GetTargetFieldValues(property, fieldPaths, null, null);

                EditorGUI.PropertyField(position, property, label, true);

                // Have to do this here in order to catch changes below
                if (!property.serializedObject.ApplyModifiedProperties())
                {
                    return;
                }

                var fieldChainNewValues = GetTargetFieldValues(property, fieldPaths, fieldValueSetChainStartIndices, fieldChainOriginalValues);

                SetTargetFieldValues(property, fieldPaths, fieldValueSetChainStartIndices, fieldChainOriginalValues, fieldChainOriginalValues);

                for (var i = 0; i < fieldPaths.Length; ++i)
                {
                    var path = fieldPaths[i];

                    var targetPropertyOrMethodFieldIndex = GetTargetMethodOrPropertyContainingFieldIndex(path, out var indexParamCount);
                    var methodToCall = FindTargetMethodOrPropertySetter(
                        setValueViaPropertyOrMethod.PropertyOrMethodName,
                        targetPropertyOrMethodFieldIndex != -1 ?
                            path[targetPropertyOrMethodFieldIndex].Type : property.serializedObject.targetObject.GetType(),
                        path[path.Length - 1].Name,
                        path[path.Length - 1].Type,
                        property.serializedObject.targetObject.GetType(),
                        ref indexParamCount,
                        out var hasRootObjectArg);

                    if (methodToCall == null)
                    {
                        Debug.LogErrorFormat(
                            "Cannot set serialized field value in the inspector via property or method for field '{0}' " +
                            "because no suitable property or method was found in the target type {1}",
                            path[path.Length - 1].Name,
                            targetPropertyOrMethodFieldIndex != -1 ?
                            path[targetPropertyOrMethodFieldIndex].Type : property.serializedObject.targetObject.GetType());

                        continue;
                    }

                    InvokeTragetMethodOrPropertySetter(
                        methodToCall,
                        targetPropertyOrMethodFieldIndex,
                        indexParamCount,
                        hasRootObjectArg,
                        path,
                        fieldChainOriginalValues[i],
                        fieldChainNewValues[i],
                        property.serializedObject.targetObjects[i]);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private static FieldData[] GetFieldPathFromSerializedPropertyPath(string serializedPropertyPath)
        {
            var pathString = serializedPropertyPath.Replace(".Array.data[", "[");
            var arrayBrackets = pathString.Count(c => c == '[');
            var pathSplits = pathString.Split('.');

            var fieldPath = new FieldData[pathSplits.Length + arrayBrackets];
            var j = 0;

            for (var i = 0; i < pathSplits.Length; i++)
            {
                var fieldName = pathSplits[i];
                var arrayBracketIndex = fieldName.IndexOf('[');
                ref var fieldData = ref fieldPath[j++];

                if (arrayBracketIndex != -1)
                {
                    fieldData.Name = fieldName.Substring(0, arrayBracketIndex);

                    fieldData = ref fieldPath[j++];

                    fieldData.Name = fieldName;
                    fieldData.Index = int.Parse(
                        fieldName.Substring(arrayBracketIndex + 1, fieldName.IndexOf(']', arrayBracketIndex + 1) - arrayBracketIndex - 1));
                    fieldData.IsContainedInArrayOrList = true;
                }
                else
                {
                    fieldData.Name = fieldName;
                }
            }

            return fieldPath;
        }

        private static FieldData[][] GetDataEnrichedFieldPathFromBasePath(FieldData[] fieldPath, SerializedProperty serializedProperty)
        {
            var targetObjects = serializedProperty.serializedObject.targetObjects;
            var enrichedPath = new FieldData[targetObjects.Length][];
            var rootContainingType = targetObjects[0].GetType();

            for (var i = 0; i < enrichedPath.Length; i++)
            {
                var path = new FieldData[fieldPath.Length];
                ref var fieldData = ref path[0];
                var srcFieldData = fieldPath[0];

                fieldData.Name = srcFieldData.Name;
                fieldData.Index = srcFieldData.Index;
                fieldData.IsContainedInArrayOrList = srcFieldData.IsContainedInArrayOrList;
                fieldData.FieldInfo = GetPrivateOrPublicInstanceFieldFromTypeOrParentTypes(rootContainingType, fieldData.Name);

                var fieldValue = fieldData.GetValue(targetObjects[i]);

                fieldData.Type = fieldValue != null ? fieldValue.GetType() : fieldData.FieldInfo.FieldType;

                var containingObjectValue = fieldValue;
                var containingType = fieldData.Type;

                for (var j = 1; j < path.Length; ++j)
                {
                    fieldData = ref path[j];
                    srcFieldData = fieldPath[j];

                    fieldData.Name = srcFieldData.Name;
                    fieldData.Index = srcFieldData.Index;
                    fieldData.IsContainedInArrayOrList = srcFieldData.IsContainedInArrayOrList;
                    fieldData.FieldInfo = GetPrivateOrPublicInstanceFieldFromTypeOrParentTypes(containingType, fieldData.Name);

                    fieldValue = fieldData.GetValue(containingObjectValue);

                    SetFieldDataType(ref fieldData, fieldValue, containingType);

                    containingObjectValue = fieldValue;
                    containingType = fieldData.Type;
                }

                enrichedPath[i] = path;
            }

            return enrichedPath;
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

        private static void SetFieldDataType(ref FieldData fieldData, object fieldValue, Type parentType)
        {
            if (fieldValue != null)
            {
                fieldData.Type = fieldValue.GetType();
            }
            else
            {
                if (fieldData.IsContainedInArrayOrList)
                {
                    fieldData.Type = parentType.IsArray ? parentType.GetElementType() : parentType.GetGenericArguments()[0];
                }
                else
                {
                    fieldData.Type = fieldData.FieldInfo.FieldType;
                }
            }
        }

        private static int GetTargetMethodOrPropertyContainingFieldIndex(FieldData[] fieldPath, out int indexParamCount)
        {
            indexParamCount = 0;

            for (var i = fieldPath.Length - 1; i >= 0; i--)
            {
                if (fieldPath[i].IsContainedInArrayOrList)
                {
                    ++indexParamCount;

                    continue;
                }

                return i - 1;
            }

            return -1;
        }

        private static int[] GetValueSetChainStartFieldIndices(FieldData[][] fieldPaths)
        {
            var indices = new int[fieldPaths.Length];

            for (var j = 0; j < fieldPaths.Length; ++j)
            {
                var fieldPath = fieldPaths[j];

                for (var i = fieldPath.Length - 2; i >= 0; i--)
                {
                    if (fieldPath[i].Type.IsValueType)
                    {
                        continue;
                    }

                    indices[j] = i + 1;

                    break;
                }
            }

            return indices;
        }

        private static object[][] GetTargetFieldValues(
            SerializedProperty serializedProperty,
            FieldData[][] fieldPaths,
            int[] fieldPathStartIndices,
            object[][] existingFullChainValues)
        {
            if (existingFullChainValues == null && fieldPathStartIndices != null)
            {
                throw new ArgumentException();
            }

            var rootObjects = serializedProperty.serializedObject.targetObjects;
            var fieldValues = new object[rootObjects.Length][];

            for (var i = 0; i < fieldValues.Length; i++)
            {
                var valueChainLength = fieldPaths[i].Length - (fieldPathStartIndices != null ? fieldPathStartIndices[i] : 0);

                fieldValues[i] = new object[valueChainLength];
            }

            for (var i = 0; i < fieldValues.Length; i++)
            {
                var fieldValueChain = fieldValues[i];

                object containingObjectValue;

                if (fieldPathStartIndices == null || fieldPathStartIndices[i] == 0)
                {
                    containingObjectValue = rootObjects[i];
                }
                else
                {
                    // existingFullChainValues should be full here
                    containingObjectValue = existingFullChainValues[i][fieldPathStartIndices[i] - 1];
                }

                var fieldPath = fieldPaths[i];

                for (var j = 0; j < fieldValueChain.Length; j++)
                {
                    var fieldData = fieldPath[fieldPathStartIndices != null ? j + fieldPathStartIndices[i] : j];
                    var fieldValue = fieldData.GetValue(containingObjectValue);

                    fieldValueChain[j] = fieldValue;

                    containingObjectValue = fieldValue;
                }
            }

            return fieldValues;
        }

        private static void SetTargetFieldValues(
            SerializedProperty serializedProperty,
            FieldData[][] fieldPaths,
            int[] fieldValueSetChainStartIndices,
            object[][] fieldValuesFullChain,
            object[][] fieldValuesChainToSet)
        {
            var fieldValueChainToSetOffset = fieldValuesFullChain[0].Length - fieldValuesChainToSet[0].Length;
            var rootObjects = serializedProperty.serializedObject.targetObjects;

            for (var i = 0; i < fieldValuesChainToSet.Length; i++)
            {
                var fieldValueFullChain = fieldValuesFullChain[i];
                var fieldValueChainToSet = fieldValuesChainToSet[i];
                var fieldValueSetChainStartIndex = fieldValueSetChainStartIndices[i];
                var fieldPath = fieldPaths[i];

                for (var j = fieldValueFullChain.Length - 1; j >= fieldValueSetChainStartIndex; j--)
                {
                    var fieldValueChainToSetIndex = j - fieldValueChainToSetOffset;
                    var containingObjectIndex = fieldValueChainToSetIndex - 1;
                    object containingObjectValue;
                    var objectValueToSet = fieldValueChainToSet[fieldValueChainToSetIndex];

                    if (containingObjectIndex == -1)
                    {
                        containingObjectValue = fieldValueSetChainStartIndex != 0 ?
                            fieldValueFullChain[fieldValueSetChainStartIndex - 1] :
                            rootObjects[i];
                    }
                    else
                    {
                        containingObjectValue = fieldValueChainToSet[containingObjectIndex];
                    }

                    fieldPath[j].SetValue(containingObjectValue, objectValueToSet);
                }
            }
        }

        private static MethodInfo FindTargetMethodOrPropertySetter(
            string methodOrPropertyName,
            Type methodOrPropertyContainingType,
            string sourceFieldName,
            Type sourceFieldType,
            Type rootObjectType,
            ref int indexParamCount,
            out bool hasRootObjectArg)
        {
            var tryAlternativeNames = false;
            var searchMethod = true;

            hasRootObjectArg = false;

            if (string.IsNullOrEmpty(methodOrPropertyName))
            {
                if (sourceFieldName.StartsWith("m_"))
                {
                    sourceFieldName = sourceFieldName.Substring(2);
                }

                methodOrPropertyName = sourceFieldName.TrimStart('_');

                tryAlternativeNames = true;
                searchMethod = false;
            }

            const BindingFlags propertyBindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var property = methodOrPropertyContainingType.GetProperty(methodOrPropertyName, propertyBindingFlags);

            if (property != null && property.PropertyType.IsAssignableFrom(sourceFieldType))
            {
                indexParamCount = 0;

                return property.GetSetMethod(true);
            }

            if (tryAlternativeNames)
            {
                methodOrPropertyName = methodOrPropertyName.Substring(0, 1).ToUpperInvariant() +
                    methodOrPropertyName.Substring(1, methodOrPropertyName.Length - 1);

                property = methodOrPropertyContainingType.GetProperty(methodOrPropertyName, propertyBindingFlags);

                if (property != null && property.PropertyType.IsAssignableFrom(sourceFieldType))
                {
                    indexParamCount = 0;

                    return property.GetSetMethod(true);
                }
            }

            if (!searchMethod)
            {
                return null;
            }

            return FindTargetMethod(
                methodOrPropertyName, methodOrPropertyContainingType, sourceFieldType, rootObjectType, ref indexParamCount, out hasRootObjectArg);
        }

        private static MethodInfo FindTargetMethod(
            string methodOrPropertyName,
            Type methodOrPropertyContainingType,
            Type sourceFieldType,
            Type rootObjectType,
            ref int indexParamCount,
            out bool hasRootObjectArg)
        {
            var methods = methodOrPropertyContainingType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo foundMethod = null;
            var indexParamCountFinal = 0;
            var hasRootObjectArgFinal = false;
            var minParams = 1;
            var indexParamsOffset = 1;

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];

                if (method.Name == methodOrPropertyName)
                {
                    var methodParams = method.GetParameters();

                    if (methodParams.Length >= minParams && methodParams[0].ParameterType.IsAssignableFrom(sourceFieldType))
                    {
                        if (methodParams.Length == 1)
                        {
                            foundMethod = method;
                            indexParamCountFinal = 0;
                            hasRootObjectArgFinal = false;
                        }
                        else
                        {
                            if (methodParams[1].ParameterType.IsAssignableFrom(rootObjectType))
                            {
                                foundMethod = method;
                                hasRootObjectArgFinal = true;
                                indexParamsOffset = 2;
                                minParams = 2;
                            }

                            if (methodParams.Length == indexParamCount + indexParamsOffset)
                            {
                                var additionalParamsAreCompatibleIndexTypes = true;

                                for (var j = indexParamsOffset; j < methodParams.Length; j++)
                                {
                                    if (methodParams[j].ParameterType != typeof(int))
                                    {
                                        additionalParamsAreCompatibleIndexTypes = false;

                                        break;
                                    }
                                }

                                if (additionalParamsAreCompatibleIndexTypes)
                                {
                                    foundMethod = method;
                                    indexParamCountFinal = indexParamCount;
                                    minParams = methodParams.Length;
                                }
                            }
                        }
                    }
                }
            }

            indexParamCount = indexParamCountFinal;
            hasRootObjectArg = hasRootObjectArgFinal;

            return foundMethod;
        }

        private static void InvokeTragetMethodOrPropertySetter(
            MethodInfo methodOrPropertySetterToCall,
            int targetPropertyOrMethodFieldIndex,
            int indexParamCount,
            bool hasRootObjectArg,
            FieldData[] fieldPath,
            object[] fieldChainOriginalValues,
            object[] fieldChainNewValues,
            object rootObject)
        {
            var propertyOrMethodTargetObjectValue = targetPropertyOrMethodFieldIndex != -1 ?
                fieldChainOriginalValues[targetPropertyOrMethodFieldIndex] : rootObject;

            var valueToSet = fieldChainNewValues[fieldChainNewValues.Length - 1];

            object[] invokeParams;

            if (indexParamCount == 0)
            {
                invokeParams = hasRootObjectArg ? new object[] { valueToSet, rootObject } : new object[] { valueToSet };
            }
            else
            {
                var indexParamsOffset = hasRootObjectArg ? 2 : 1;

                invokeParams = new object[indexParamCount + indexParamsOffset];
                invokeParams[0] = valueToSet;

                if (hasRootObjectArg)
                {
                    invokeParams[1] = rootObject;
                }

                for (var j = 0; j < indexParamCount; j++)
                {
                    invokeParams[j + indexParamsOffset] = fieldPath[fieldPath.Length - indexParamCount + j].Index;
                }
            }

            try
            {
                methodOrPropertySetterToCall.Invoke(propertyOrMethodTargetObjectValue, invokeParams);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);

                return;
            }

            var valueAfterSet = propertyOrMethodTargetObjectValue;

            for (var j = targetPropertyOrMethodFieldIndex; j >= 0; j--)
            {
                var containingObjectValue = j != 0 ? fieldChainOriginalValues[j - 1] : rootObject;

                if (!fieldPath[j].Type.IsValueType)
                {
                    break;
                }

                fieldPath[j].SetValue(containingObjectValue, valueAfterSet);

                valueAfterSet = containingObjectValue;
            }
        }
    }
}
