using OctoXR.KinematicInteractions.Utilities;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Checks if there are any grab points and objects within the range of the grab controller's raycast and adds them to the list of interaction candidates.
    /// </summary>
    [System.Obsolete("This class is no longer used, please attach the InteractionHandler class and add a DistanceInteractionModifier to it instead!")]
    public class DistanceGrabController : InteractionHandler
    {
        [Tooltip("Position from which the actual raycast should be started.")]
        [SerializeField] private Transform raycastSource;
        [Tooltip("Position from which the representation ray should be drawn.")]
        [SerializeField] private Transform rayVisualSource;
        [Tooltip("Line renderer you want to use to draw the ray visuals.")]
        [SerializeField] private LineRenderer lineRenderer;
        [Tooltip("Maximum distance of the raycast.")]
        [SerializeField][Range(0f, 100f)] private float maxRayDistance = 30f;
        [Tooltip("Layer that you want the ray to interact with.")]
        [SerializeField] private LayerMask layerMask;
        [Tooltip("If ticked, the grabbable is detected via a spherecast - this can be easier to find grabbables.")]
        [SerializeField] private bool isSphereCast = true;
        [Tooltip("Should the line renderer be drawn or will the interaction be invisible.")]
        [SerializeField] private bool shouldDrawLineRenderer;

        public override void Update()
        {
            FindClosestGrabbable();
            if (shouldDrawLineRenderer) DrawLineRenderer();
            LastClosestObject = ClosestObject;
            GrabCheck();
        }

        private void DrawLineRenderer()
        {
            if (ClosestObject && !ClosestObject.IsGrabbed)
            {
                lineRenderer.enabled = true;
                RaycastVisuals.DrawLine(rayVisualSource.position, ClosestObject.transform.position, lineRenderer);
            }
            else if (GrabbedObject)
            {
                lineRenderer.enabled = false;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

        private void FindClosestGrabbable()
        {
            if (GrabbedObject) return;
            if (isSphereCast) RaycastManager.SphereCast(raycastSource, 0, maxRayDistance, layerMask);
            else RaycastManager.Raycast(raycastSource, maxRayDistance, layerMask);

            ClosestObject = RaycastManager.isTargetHit ? RaycastManager.target : null;

            if (ClosestObject)
            {
                OnHover.Invoke();
                ClosestObject.OnHover.Invoke();

                GrabPointController.AddGrabbableObject(RaycastManager.target.GetComponent<Collider>());
                GrabPointController.AddGrabPoint(RaycastManager.target.GetComponent<Collider>());

                DistanceCheck.CheckGrabPointDistance(this);
            }
            else
            {
                OnUnhover.Invoke();
                if (ClosestObject) ClosestObject.OnUnhover.Invoke();
                else if (LastClosestObject) LastClosestObject.OnUnhover.Invoke();
                GrabPointsInReach.Clear();
            }
        }
    }
}
