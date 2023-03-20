using UnityEngine;

namespace OctoXR.Scripts.Editor
{
    public class OctoGUIStyles
    {
        public static GUIStyle LabelStyle(TextAnchor textAnchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = 13) {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
            style.alignment = textAnchor;
            style.fontStyle = fontStyle;
            return style;
        }
        
        public static GUIStyle BoxStyle(TextAnchor textAnchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = 13) {
            var style = new GUIStyle(GUI.skin.box);
            style.fontSize = fontSize;
            style.alignment = textAnchor;
            style.fontStyle = fontStyle;
            return style;
        }
    }
}