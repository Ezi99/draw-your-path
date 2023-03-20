using UnityEngine;
using UnityEditor;
using OctoXR.HandPoseDetection;

namespace OctoXR.Editor.PoseDetection
{
    [CustomEditor(typeof(HandShapeCreator))]
    public class HandPoseCreatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var shape = (HandShapeCreator)target;

            if (GUILayout.Button("Create Shape"))
            {
                shape.CreateShape();
            }
        }
    }
}
