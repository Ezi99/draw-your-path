using UnityEngine;
using UnityEditor;
using OctoXR.UI;

namespace OctoXR.Editor.UI
{
    [CustomEditor(typeof(AutomaticCanvasScaler))]
    public class AutomaticCanvasScalerEditor : UnityEditor.Editor
    {
        SerializedProperty canvasWidthAndHeight;
        SerializedProperty canvasWidthInMeters;

        private void OnEnable()
        {
            canvasWidthAndHeight = serializedObject.FindProperty("canvasWidthAndHeight");
            canvasWidthInMeters = serializedObject.FindProperty("canvasWidthInMeters");
        }


        public override void OnInspectorGUI()
        {
            AutomaticCanvasScaler automaticCanvasScaler = (AutomaticCanvasScaler)target;

            EditorGUILayout.PropertyField(canvasWidthAndHeight, new GUIContent("Canvas Width And Height"));
            EditorGUILayout.PropertyField(canvasWidthInMeters, new GUIContent("Canvas Width In Meters"));

            if (GUILayout.Button("Scale Canvas"))
            {
                automaticCanvasScaler.ScaleCanvas();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
