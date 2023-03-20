using System.Collections.Generic;
using OctoXR.KinematicInteractions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OctoXR.Editor.KinematicInteractions
{
    [CustomEditor(typeof(Grabbable))]
    [CanEditMultipleObjects]
    public class GrabbableEditor : UnityEditor.Editor
    {
        public HandType grabPointType;
        public GrabbableType grabbableType;
        public Grabbable grabbable;
        public Transform grabbableTransform;
        public readonly GrabPointCreator grabPointCreator;

        [SerializeField] public List<GrabPoint> grabPoints;

        public Transform activeGrabPoint;

        private bool useDefaultPath;
        private string path;

        private MonoBehaviour monoBehaviour;

        public void OnEnable()
        {
            monoBehaviour = (MonoBehaviour)target;
            grabbableTransform = monoBehaviour.GetComponent<Transform>();
            grabbable = grabbableTransform.GetComponent<Grabbable>();

            grabPoints = new List<GrabPoint>(grabbable.GetComponentsInChildren<GrabPoint>());
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();   

            grabbableType = (GrabbableType)EditorGUILayout.EnumPopup("Type of grabbable modifier to add: ", grabbableType);

            if (GUILayout.Button("Add modifier"))
            {
                var currentModifier = monoBehaviour.GetComponent<GrabbableModifier>();
                DestroyImmediate(currentModifier);

                switch (grabbableType)
                {
                    case GrabbableType.Kinematic:
                        currentModifier = monoBehaviour.gameObject.AddComponent<KinematicGrabbableModifier>();
                        break;
                    case GrabbableType.Joint:
                        currentModifier = monoBehaviour.gameObject.AddComponent<JointBasedGrabbableModifier>();
                        break;
                    case GrabbableType.Scaleable:
                        currentModifier = monoBehaviour.gameObject.AddComponent<ScaleableGrabbableModifier>();
                        break;
                    default:
                        break;
                }

                grabbable.GrabbableModifier = currentModifier;
            }

            if (!grabbable.IsPrecisionGrab)
            {
                if (grabPoints.Count <= 0)
                {
                    EditorGUILayout.HelpBox(
                        "No grab points detected! Please add at least one grab point for non-precision interactions to work! " +
                        "Alternatively, tick the IsPrecisionGrab box to make the interaction precision based instead.",
                        MessageType.Warning);
                }

                grabPointType = (HandType)EditorGUILayout.EnumPopup("Orientation of grab point to create: ", grabPointType);

                if (GUILayout.Button("Create Grab Point"))
                {
                    if (grabPointType == HandType.Left)
                    {
                        var grabPointLeft = GrabPointCreator.CreateGrabPoint("GrabPoint_L", grabbableTransform, HandType.Left);
                        grabPoints.Add(grabPointLeft.GetComponent<GrabPoint>());
                    }

                    if (grabPointType == HandType.Right)
                    {
                        var grabPointRight = GrabPointCreator.CreateGrabPoint("GrabPoint_R", grabbableTransform, HandType.Right);
                        grabPoints.Add(grabPointRight.GetComponent<GrabPoint>());
                    }
                }
            }

            GUILayout.Box("Prefab options");

            useDefaultPath = EditorGUILayout.Toggle("Use default path", useDefaultPath);

            if (!useDefaultPath)
            {
                if (GUILayout.Button("Choose folder"))
                {
                    path = EditorUtility.OpenFolderPanel("Choose where you want to save your prefab", "", "");
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Default path is: Assets/{SceneManager.GetActiveScene().name}_Prefabs", MessageType.Info);
            }

            if (GUILayout.Button("Save to project"))
            {
                var previewHands = grabbable.GetComponentsInChildren<PreviewHand>();

                foreach (var ph in previewHands)
                {
                    DestroyImmediate(ph.gameObject);
                }

                PrefabSaver.CreatePrefab(grabbable.gameObject, path, useDefaultPath);
            }
        }

        private void DrawGrabPointGizmos()
        {
            foreach (var grabPoint in grabPoints)
            {
                Handles.color = grabPoint.handType switch
                {
                    HandType.Left => Color.red,
                    HandType.Right => Color.yellow,
                    _ => Handles.color
                };

                var pressed = Handles.Button(grabPoint.transform.position, grabPoint.transform.rotation, 0.01f, 0.005f, Handles.SphereHandleCap);

                if (pressed)
                {
                    activeGrabPoint = IsSelected(grabPoint.transform) ? null : grabPoint.transform;
                    Selection.activeObject = activeGrabPoint;
                }
            }
        }

        public void OnSceneGUI()
        {
            DrawGrabPointGizmos();
        }

        private bool IsSelected(Transform grabPoint)
        {
            return grabPoint == activeGrabPoint;
        }
    }
}
