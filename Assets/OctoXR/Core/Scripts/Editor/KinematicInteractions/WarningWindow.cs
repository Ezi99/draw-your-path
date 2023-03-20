using UnityEditor;

namespace OctoXR.Editor.KinematicInteractions
{
    public class WarningWindow : EditorWindow
    {
        public static void Open(string title ,string text)
        {
            EditorUtility.DisplayDialog(
                title,
                text,
                "Ok");
        }
    
        public static void CloseWindow(WarningWindow warningWindow)
        {
            warningWindow.Close();
        } 
    }
}
