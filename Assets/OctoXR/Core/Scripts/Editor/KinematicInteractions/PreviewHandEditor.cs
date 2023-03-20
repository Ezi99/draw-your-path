using OctoXR.KinematicInteractions;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    [CustomEditor(typeof(PreviewHand))]
    public class PreviewHandEditor : UnityEditor.Editor
    {
        private PreviewHand previewHand;
        private Transform activeJoint;

        private void OnEnable()
        {
            previewHand = target as PreviewHand;
        }

        private void OnSceneGUI()
        {
            DrawJointGizmos();
            DrawJointGizmo();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Edit grab point"))
            {
                InspectorLock.ToggleInspectorLock();
                
                Selection.SetActiveObjectWithContext(previewHand.GetComponentInParent<GrabPoint>(), null);
                SceneView.FrameLastActiveSceneView();
                
                Tools.current = Tool.Move;

                InspectorLock.ToggleInspectorLock();
            }
        }

        private void DrawJointGizmos()
        {
            foreach (var joint in previewHand.Joints)
            {
                var pressed = Handles.Button(joint.position, joint.rotation, 0.01f, 0.005f, Handles.SphereHandleCap);

                if (pressed)
                    activeJoint = IsSelected(joint) ? null : joint;
            }
        }

        private bool IsSelected(Transform joint)
        {
            return joint == activeJoint;
        }

        private void DrawJointGizmo()
        {
            if (HasActiveJoint())
            {
                var currentRotation = activeJoint.rotation;
                var newRotation = Handles.RotationHandle(currentRotation, activeJoint.position);
            
                if (HandleRotated(currentRotation, newRotation))
                {
                    activeJoint.rotation = newRotation;
                    Undo.RecordObject(target, "HandleRotated");
                }
            }
        }

        private bool HasActiveJoint()
        {
            return activeJoint;
        }

        private bool HandleRotated(Quaternion currentRotation, Quaternion newRotation)
        {
            return currentRotation != newRotation;
        }
    }
}
