using UnityEditor;

namespace OctoXR.Editor.KinematicInteractions
{
    public class InspectorLock
    {
        public static void ToggleInspectorLock()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        public static void LockEditor()
        {
            ActiveEditorTracker.sharedTracker.isLocked = true;
        }

        public static void UnlockEditor()
        {
            ActiveEditorTracker.sharedTracker.isLocked = false;
        }
    }
}