using System;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    public class PoseWindow : EditorWindow
    {
        public static GameObject previewHand;
        private static PoseWindow poseWindow;
        private GameObject grabPoint;

        private void OnEnable()
        {
            grabPoint = Selection.activeGameObject;
            SceneView.FrameLastActiveSceneView();
            poseWindow = this;
        }

        private void OnDisable()
        {
            DeleteObject(previewHand);
            Tools.current = Tool.Move;
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Edit pose"))
            {
                Selection.SetActiveObjectWithContext(previewHand, null);
                SceneView.FrameLastActiveSceneView();

                Tools.current = Tool.None;
            }

            if (GUILayout.Button("Edit grab point"))
            {
                Selection.SetActiveObjectWithContext(grabPoint, null);
                SceneView.FrameLastActiveSceneView();

                Tools.current = Tool.Move;
            }
        }

        public void DestroyAllOnLostFocus()
        {
            DeleteObject(previewHand);
            poseWindow.Close();
            Tools.current = Tool.Move;
        }

        public static void DeleteObject(GameObject gameObject)
        {
            DestroyImmediate(gameObject);
        }

        public static void Open(GameObject gameObject)
        {
            GetWindow<PoseWindow>(false, "Grab Point Window");
            previewHand = gameObject;
        }

        public static void CloseWindow()
        {
            poseWindow.Close();
        }
    }
}
