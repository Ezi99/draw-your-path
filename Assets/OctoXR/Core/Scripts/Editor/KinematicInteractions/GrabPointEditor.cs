using OctoXR.KinematicInteractions;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    [CustomEditor(typeof(GrabPoint))]
    [CanEditMultipleObjects]
    public class GrabPointEditor : UnityEditor.Editor
    {
        [HideInInspector] [SerializeField] private GameObject[] allObjects;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var monoBeh = (MonoBehaviour) target;
            var createButton = false;
            var closeButton = false;
            var editButton = false;

            var grabPoint = monoBeh.GetComponent<GrabPoint>();
            
            if (!grabPoint.InstantiatedPreviewHand)
            {
                createButton = GUILayout.Button("Preview Pose");
            }
            else
            {
                editButton = GUILayout.Button("Edit pose");
                closeButton = GUILayout.Button("Hide Pose");
            }

            if (createButton)
            {
                grabPoint.InstantiateHandPose();
                
                ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                ActiveEditorTracker.sharedTracker.ForceRebuild();
                
                ShowAll();
            }
            
            if (editButton)
            {
                InspectorLock.LockEditor();
                
                Selection.SetActiveObjectWithContext(grabPoint.InstantiatedPreviewHand, null);
                SceneView.FrameLastActiveSceneView();

                Tools.current = Tool.None;

                InspectorLock.UnlockEditor();
            }
            
            if (closeButton)
            {
                ShowAll();
                DeleteObject(grabPoint.InstantiatedPreviewHand);
                Tools.current = Tool.Move;

                InspectorLock.UnlockEditor();
            }
        }

        public void ShowAll()
        {
            //BROKEN CODE
            /*
            allObjects = FindObjectsOfType<GameObject> ();
            
            foreach (var gameObject in allObjects)
            {
                if (gameObject.hideFlags == HideFlags.HideInHierarchy)
                {
                    gameObject.hideFlags = HideFlags.None;
                }
                else if (gameObject.hideFlags == HideFlags.None)
                {
                    gameObject.hideFlags = HideFlags.HideInHierarchy; 
                }
            }
            */
        }

        public static void DeleteObject(GameObject gameObject)
        {
            DestroyImmediate(gameObject);
        }
    }
}
