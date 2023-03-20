using OctoXR.KinematicInteractions.Utilities;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// An implementation of the InteractionModifier class, used to look for objects at a set distance from the interactor.
    /// </summary>
    public class DistanceInteractionModifier : InteractionModifier
    {
        [Tooltip("Position from which the actual raycast should be started.")]
        [SerializeField] private Transform raycastSource;
        [Tooltip("Position from which the representation ray should be drawn.")]
        [SerializeField] private Transform rayVisualSource;
        
        [SerializeField][Range(0f, .1f)] private float radius = 0.01f;
        [Tooltip("Line renderer you want to use to draw the ray visuals.")]
        [SerializeField] private LineRenderer lineRenderer;
        [Tooltip("Maximum distance of the raycast.")]
        [SerializeField][Range(0f, 100f)] private float maxRayDistance = 30f;
        [Tooltip("If ticked, the grabbable is detected via a spherecast - this can be easier to find grabbables.")]
        [SerializeField] private bool isSphereCast = true;
        [Tooltip("Should the line renderer be drawn or will the interaction be invisible.")]
        [SerializeField] private bool shouldDrawLineRenderer;

        public void Update()
        {
            FindClosestGrabbable();
            if (shouldDrawLineRenderer) DrawLineRenderer();
            InteractionHandler.LastClosestObject = InteractionHandler.ClosestObject;
            InteractionHandler.GrabCheck();
        }

        private void DrawLineRenderer()
        {
            if (InteractionHandler.ClosestObject && !InteractionHandler.ClosestObject.IsGrabbed)
            {
                lineRenderer.enabled = true;
                RaycastVisuals.DrawLine(rayVisualSource.position, InteractionHandler.ClosestObject.transform.position, lineRenderer);
            }
            else if (InteractionHandler.GrabbedObject)
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
            if (InteractionHandler.GrabbedObject) return;
            if (isSphereCast) RaycastManager.SphereCast(raycastSource, radius, maxRayDistance, InteractionHandler.LayerMask);
            else RaycastManager.Raycast(raycastSource, maxRayDistance, InteractionHandler.LayerMask);

            InteractionHandler.ClosestObject = RaycastManager.isTargetHit ? RaycastManager.target : null;

            if (InteractionHandler.ClosestObject)
            {
                InteractionHandler.OnHover.Invoke();
                InteractionHandler.ClosestObject.OnHover.Invoke();

                InteractionHandler.GrabPointController.AddGrabbableObject(RaycastManager.target.GetComponent<Collider>());
                InteractionHandler.GrabPointController.AddGrabPoint(RaycastManager.target.GetComponent<Collider>());

                DistanceCheck.CheckGrabPointDistance(InteractionHandler);
            }
            else
            {
                InteractionHandler.OnUnhover.Invoke();
                if (InteractionHandler.ClosestObject) InteractionHandler.ClosestObject.OnUnhover.Invoke();
                else if (InteractionHandler.LastClosestObject) InteractionHandler.LastClosestObject.OnUnhover.Invoke();
                InteractionHandler.GrabPointsInReach.Clear();
            }
        }
    }
}
