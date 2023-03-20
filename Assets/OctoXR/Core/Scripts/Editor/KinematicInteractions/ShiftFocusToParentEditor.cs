using OctoXR.KinematicInteractions.Utilities;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    [CustomEditor(typeof(ShiftFocusToParent))]
    public class ShiftFocusToParentEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            GameObject targetGO = ((MonoBehaviour) target).gameObject;
            SceneView sceneView = EditorWindow.focusedWindow as SceneView;
            if (targetGO.transform.parent != null && sceneView != null)
            {
                GameObject[] currentSelection = Selection.gameObjects;
                int idx = -1;
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    if (Selection.gameObjects[i].GetInstanceID() == targetGO.GetInstanceID())
                    {
                        idx = i;
                    }
                }

                if (idx != -1)
                {
                    currentSelection[idx] = targetGO.transform.parent.gameObject;
                    Selection.objects = currentSelection;
                }
            }
        }
    }
}
