using System.Text;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute), true)]
    public class LabelPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelAttribute = (LabelAttribute)attribute;

            label.text = GetPropertyLabelText(property, label, labelAttribute);

            EditorGUI.PropertyField(position, property, label, true);
        }

        public static string GetPropertyLabelText(
            SerializedProperty property,
            GUIContent label,
            LabelAttribute labelAttribute)
        {
            string labelText;

            if (labelAttribute.LabelText != null)
            {
                labelText = labelAttribute.LabelText;
            }
            else
            {
                labelText = label.text;
            }

            if (TryGetArrayIndexOfSerializedProperty(property, out var index))
            {
                if (labelAttribute.TryGetCustomFormattedLabelText(index, out var customFormattedLabelText))
                {
                    labelText = customFormattedLabelText;
                }
                else
                {
                    if (labelAttribute.ArrayItemLabelIndexFormat != null)
                    {
                        index += labelAttribute.ArrayItemLabelBaseIndex;

                        var labelBuilder = labelAttribute.LabelText == null ?
                            GetSerializedPropertyLabelWithIndexPartRemoved(label.text) :
                            new StringBuilder(labelText);

                        labelBuilder.Append(string.Format(labelAttribute.ArrayItemLabelIndexFormat, index));

                        labelText = labelBuilder.ToString();
                    }
                    else if (labelAttribute.ArrayItemLabelBaseIndex != 0)
                    {
                        var labelBuilder = labelAttribute.LabelText == null ?
                            GetSerializedPropertyLabelWithIndexPartRemoved(label.text) :
                            new StringBuilder(labelText);

                        labelBuilder.Append(' ');
                        labelBuilder.Append(index + labelAttribute.ArrayItemLabelBaseIndex);

                        labelText = labelBuilder.ToString();
                    }
                }
            }

            return labelText;
        }

        private static bool TryGetArrayIndexOfSerializedProperty(SerializedProperty serializedProperty, out int index)
        {
            var propertyPath = serializedProperty.propertyPath;

            if (propertyPath[propertyPath.Length - 1] == ']')
            {
                var bracketIndex = propertyPath.IndexOf('[');
                var indexString = propertyPath.Substring(bracketIndex + 1, propertyPath.Length - bracketIndex - 2);

                index = int.Parse(indexString);

                return true;
            }

            index = -1;

            return false;
        }

        private static StringBuilder GetSerializedPropertyLabelWithIndexPartRemoved(string label)
        {
            var stringBuilder = new StringBuilder(label);
            var breakOnNonWhiteSpaceChar = false;
            var removeFromIndex = -1;

            for (var i = stringBuilder.Length - 1; i >= 0; i--)
            {
                if (stringBuilder[i] == ' ')
                {
                    breakOnNonWhiteSpaceChar = true;
                }
                else if (breakOnNonWhiteSpaceChar)
                {
                    break;
                }

                removeFromIndex = i;
            }

            if (removeFromIndex != -1)
            {
                stringBuilder.Remove(removeFromIndex, stringBuilder.Length - removeFromIndex);
            }

            return stringBuilder;
        }
    }
}
