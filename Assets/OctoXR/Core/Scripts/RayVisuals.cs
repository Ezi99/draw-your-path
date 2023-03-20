using System.Collections.Generic;
using UnityEngine;

namespace OctoXR
{
    public enum RayType { Line, Curve }

    [RequireComponent(typeof(LineRenderer))]
    public class RayVisuals : MonoBehaviour
    {
        [Header("Ray Options")]
        [SerializeField] [Range(0.0001f, 0.05f)] private float rayStartWidth = 0.02f;
        [SerializeField] [Range(0.0001f, 0.05f)] private float rayEndWidth = 0.02f;
        [Tooltip("Maximum value that ReduceLineWidthByPercentage() can reduce the line width.")]
        [SerializeField] [Range(0.0001f, 0.02f)] private float rayWidthDelta = 0.012f;
        [SerializeField] private RayType rayType = RayType.Curve;
        [SerializeField] [Range(2, 100)] private int raySegments = 40;
        [SerializeField] private Gradient rayValidGradient;
        [SerializeField] private Gradient raySelectGradient;
        [SerializeField] private Gradient rayInvalidGradient;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = rayStartWidth;
            lineRenderer.endWidth = rayEndWidth;
            lineRenderer.colorGradient = rayValidGradient;
            DisableLineRenderer();
        }

        public void EnableLineRenderer() => lineRenderer.enabled = true;
        public void DisableLineRenderer() => lineRenderer.enabled = false;

        public void DrawValidRay(Vector3 start, Vector3 end)
        {
            if (!lineRenderer.enabled)
            {
                EnableLineRenderer();
            }

            DrawRay(start, end, rayValidGradient);
        }

        public void DrawSelectRay(Vector3 start, Vector3 end)
        {
            if (!lineRenderer.enabled)
            {
                EnableLineRenderer();
            }

            DrawRay(start, end, raySelectGradient);
        }

        public void DrawInvalidRay(Vector3 start, Vector3 end)
        {
            if (!lineRenderer.enabled)
            {
                EnableLineRenderer();
            }

            DrawRay(start, end, rayInvalidGradient);
        }

        /// <param name="percentage">Value between 0 and 1</param>
        public void ReduceLineWidthByPercentage(float percentage)
        {
            if (percentage < 0)
            {
                Debug.LogWarning("Percentage provided is less than zero. Clamping to 0");
            }
            else if (percentage > 1)
            {
                Debug.LogWarning("Percentage provided is greater than one. Clamping to 1");
            }

            percentage = Mathf.Clamp01(percentage);

            var percentageScaled = (1 - percentage) * rayStartWidth;
            lineRenderer.startWidth = Mathf.Clamp(percentageScaled, rayStartWidth - rayWidthDelta, rayStartWidth);
        }

        private void DrawRay(Vector3 start, Vector3 end, Gradient gradient)
        {
            if (rayType == RayType.Line)
            {
                DrawLine(start, end, gradient);
            }
            else
            {
                DrawCurve(start, end, gradient);
            }
        }

        private void DrawLine(Vector3 lineStart, Vector3 lineEnd, Gradient gradient)
        {
            lineRenderer.colorGradient = gradient;
            lineRenderer.positionCount = raySegments;

            var unitVector = (lineEnd - lineStart) / (raySegments - 1);
            var previousPoint = lineStart;

            for (var i = 0; i < raySegments; i++)
            {
                lineRenderer.SetPosition(i, previousPoint);
                previousPoint += unitVector;
            }
        }

        private void DrawCurve(Vector3 curveStart, Vector3 curveEnd, Gradient gradient)
        {
            var points = new List<Vector3>();
            var middle = new Vector3((curveStart.x + curveEnd.x) / 2, curveStart.y, (curveStart.z + curveEnd.z) / 2);

            for (float ratio = 0; ratio <= 1; ratio += 1.0f / raySegments)
            {
                var firstTangent = Vector3.Lerp(curveStart, middle, ratio);
                var secondTangent = Vector3.Lerp(middle, curveEnd, ratio);
                var curve = Vector3.Lerp(firstTangent, secondTangent, ratio);

                points.Add(curve);
            }

            lineRenderer.colorGradient = gradient;
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }
}
