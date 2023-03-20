using System.Collections;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Moves a grabbable object to match the position and rotation of the grab controller transform.
    /// </summary>
    public class GrabPointController : MonoBehaviour
    {
        private bool positionReached;
        public bool PositionReached { get => positionReached; set => positionReached = value; }

        [SerializeField] private float smoothingValue = 0.1f;
        public float SmoothingValue { get => smoothingValue; set => smoothingValue = value; }

        private Coroutine activeCoroutine;
        public Coroutine ActiveCoroutine { get => activeCoroutine; set => activeCoroutine = value; }

        private InteractionHandler interactionHandler;
        private Vector3 velocity = Vector3.zero;

        private void Awake()
        {
            interactionHandler = GetComponent<InteractionHandler>();
        }

        /// <summary>
        /// Matches the position and rotation of a grabbed object to the given grab point. Smoothness of the transition is determined by max delta.
        /// </summary>
        /// <param name="connectedObject"></param>
        /// <param name="grabPoint"></param>
        /// <param name="maxDelta"></param>
        public void MoveToGrabPoint(Grabbable connectedObject, GrabPoint grabPoint)
        {
            AdjustObjectPosition(connectedObject, grabPoint);
        }

        private void AdjustObjectPosition(Grabbable connectedObject, GrabPoint grabPoint)
        {
            if (activeCoroutine != null)
            {
                KillCoroutine();
            }

            activeCoroutine = StartCoroutine(ObjectPositionCoroutine(connectedObject, grabPoint));
        }

        private IEnumerator ObjectPositionCoroutine(Grabbable connectedObject, GrabPoint grabPoint)
        {
            if (positionReached) yield return null;

            connectedObject.transform.SetParent(transform, true);

            Vector3 difference = connectedObject.transform.position - (transform.position - connectedObject.transform.rotation * grabPoint.Offset);

            while (difference.sqrMagnitude > 0.00001)
            {
                connectedObject.transform.rotation = Quaternion.Lerp(connectedObject.transform.rotation,
                   transform.rotation * Quaternion.Inverse(grabPoint.RotationOffset), smoothingValue);

                connectedObject.transform.position = Vector3.SmoothDamp(connectedObject.transform.position,
                    transform.position - connectedObject.transform.rotation * grabPoint.Offset, ref velocity, smoothingValue);

                difference = connectedObject.transform.position - (transform.position - connectedObject.transform.rotation * grabPoint.Offset);
                yield return null;
            }

            connectedObject.transform.position = transform.position - interactionHandler.GrabbedObject.transform.rotation * interactionHandler.ClosestGrabPoint.Offset;
            connectedObject.transform.rotation = transform.rotation * Quaternion.Inverse(interactionHandler.ClosestGrabPoint.RotationOffset);

            positionReached = true;

            yield return null;
        }


        /// <summary>
        /// Adds a grab point to an array of grab points within reach detected by the grab controller.
        /// </summary>
        /// <param name="grabPointCollider"></param>
        public void AddGrabPoint(Collider grabPointCollider)
        {
            var grabPoints = grabPointCollider.GetComponentsInChildren<GrabPoint>();

            foreach (var grabPoint in grabPoints)
            {
                if (interactionHandler.ClosestObject != grabPoint.ParentGrabbable) return;
                if (interactionHandler.GrabPointsInReach.Contains(grabPoint)) return;

                if (grabPoint.IsGrabbed) return;

                if (interactionHandler.InteractionHand.HandSkeletonType == grabPoint.HandType)
                {
                    interactionHandler.GrabPointsInReach.Add(grabPoint);
                }

                DistanceCheck.CheckGrabPointDistance(interactionHandler);
            }
        }

        public void RemoveGrabPoint(GrabPoint[] grabPoints)
        {
            foreach (var grabPoint in grabPoints)
            {
                interactionHandler.GrabPointsInReach.Remove(grabPoint);
            }
        }

        public void KillCoroutine()
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        /// <summary> 
        /// Adds a grabbable to an array of grabbables within reach detected by the grab controller.
        /// </summary>
        /// <param name="grabbableCollider"></param>
        public void AddGrabbableObject(Collider grabbableCollider)
        {
            var objectInReach = grabbableCollider.GetComponentInChildren<Grabbable>();

            if (objectInReach)
            {
                if (objectInReach.IsGrabbed) return;
                if (!interactionHandler.ObjectsInReach.Contains(objectInReach)) interactionHandler.ObjectsInReach.Add(objectInReach);
            }

            DistanceCheck.CheckGrabbableObjectDistance(interactionHandler);
        }

        public void RemoveGrabbableObject(Grabbable grabbable)
        {
            interactionHandler.ObjectsInReach.Remove(grabbable);
        }
    }
}
