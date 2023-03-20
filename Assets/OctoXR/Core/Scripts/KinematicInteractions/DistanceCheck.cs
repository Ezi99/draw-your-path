using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Used by the grab point controller to check for the closest grabbable and/or grab point.
    /// </summary>
    public class DistanceCheck
    {
        /// <summary>
        /// Checks the distance of grabbable objects in reach of the grab controller and assigns the closest grabbable to the grab controller.
        /// </summary>
        /// <param name="interactionHandler"></param>
        public static void CheckGrabbableObjectDistance(InteractionHandler interactionHandler)
        {
            interactionHandler.ClosestObject = null;
            var closestDistance = float.MaxValue;

            foreach (var grabbable in interactionHandler.ObjectsInReach)
            {
                var distance = (grabbable.transform.position - interactionHandler.transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    interactionHandler.ClosestObject = grabbable;
                }
            }
        }

        /// <summary>
        /// Checks for distance of grab points in reach of the grab controller and assigns the closest grab point's grabbable to the grab controller.
        /// </summary>
        /// <param name="interactionHandler"></param>
        public static void CheckGrabPointDistance(InteractionHandler interactionHandler)
        {
            var closestDistance = float.MaxValue;

            if (interactionHandler.GrabPointsInReach != null)
                foreach (var grabPoint in interactionHandler.GrabPointsInReach)
                {
                    if (grabPoint.ParentGrabbable != interactionHandler.ClosestObject)
                    {
                        interactionHandler.GrabPointsInReach.Remove(grabPoint);
                        return;
                    }

                    var distance = (grabPoint.transform.position - interactionHandler.transform.position).sqrMagnitude;

                    if (distance < closestDistance)
                    {
                        if (interactionHandler.ClosestObject != grabPoint.ParentGrabbable) return;

                        closestDistance = distance;
                        if (grabPoint.ParentGrabbable == interactionHandler.ClosestObject) interactionHandler.ClosestGrabPoint = grabPoint;
                    }
                }
        }
    }
}
