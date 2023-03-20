using System.IO;
using OctoXR.KinematicInteractions;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    [CustomEditor(typeof(PoseData))]
    public class PoseDataEditor : UnityEditor.Editor
    {
        private PoseData poseData;
        private GrabPoint grabPoint;
        private string defaultPoseName;

        private bool changesApplied;
        private bool poseSaved;

        GameObject gameObject;
        UnityEditor.Editor gameObjectEditor;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var monoBehaviour = (MonoBehaviour) target;
            
            poseData = monoBehaviour.GetComponent<PoseData>();
            grabPoint = monoBehaviour.GetComponentInParent<GrabPoint>();

            var previewHand = monoBehaviour.GetComponent<PreviewHand>();

            GUIStyle bgColor = new GUIStyle();
            var image = Resources.Load<Texture>("oxr_banner");
            bgColor.normal.background = (Texture2D) image;

            if (grabPoint.InstantiatedPreviewHand != null)
            {
                if (gameObjectEditor == null)
                    gameObjectEditor = UnityEditor.Editor.CreateEditor(grabPoint.InstantiatedPreviewHand);

                gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
                gameObjectEditor.ReloadPreviewInstances();
            }
            
            if(!changesApplied) EditorGUILayout.HelpBox("Changes not applied!", MessageType.Warning);

            if (GUILayout.Button("Apply changes"))
            {
                poseData.Save(previewHand);
                changesApplied = true;
            }

            if(!poseSaved) EditorGUILayout.HelpBox("Pose not saved!", MessageType.Warning);

            if (GUILayout.Button("Save Changes"))
            {
                CreatePose();
                poseSaved = true;
            }
        }

        private void CreatePose()
        {
            CreatePoseAsset();
        }

        private void CreatePoseAsset()
        {
            var customPose = CreateInstance<CustomHandPose>();

            customPose.SetHandPoseData(poseData);

            var path = EditorUtility.SaveFilePanelInProject("Save Pose", "New Pose", "asset", "Pose saved!");
            
            if (string.IsNullOrEmpty(path)) return;

            customPose.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(customPose, path);

            grabPoint.GrabPose = customPose;
        }
    }
}
