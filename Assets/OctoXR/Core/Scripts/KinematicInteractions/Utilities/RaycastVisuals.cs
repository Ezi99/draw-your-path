using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    public abstract class RaycastVisuals
    {
        public static void DrawCurve(Vector3 start, Vector3 end, LineRenderer lineRenderer)
        {
            var points = new List<Vector3>();
            var middle = new Vector3((start.x + end.x) / 2, start.y, (start.z + end.z) / 2);

            for (float ratio = 0; ratio <= 1; ratio += 1.0f / 30.0f)
            {
                var firstTangent = Vector3.Lerp(start, middle, ratio);
                var secondTangent = Vector3.Lerp(middle, end, ratio);
                var curve = Vector3.Lerp(firstTangent, secondTangent, ratio);

                points.Add(curve);
            }

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }

        public static void DrawLine(Vector3 start, Vector3 end, LineRenderer lineRenderer)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
    }
}
