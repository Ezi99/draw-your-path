using UnityEngine;

namespace OctoXR.UI
{
    [RequireComponent(typeof(Canvas))]
    public class AutomaticCanvasScaler : MonoBehaviour
    {
        public Vector2 canvasWidthAndHeight;
        public float canvasWidthInMeters;

        public void ScaleCanvas()
        {
            var rectTransform = GetComponent<RectTransform>();

            rectTransform.sizeDelta = canvasWidthAndHeight;

            var scale = canvasWidthInMeters / canvasWidthAndHeight.x;

            rectTransform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
