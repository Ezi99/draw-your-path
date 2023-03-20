
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Abstract class which can be used to create different types of interaction hand drivers responsible for grab detection.
    /// </summary>
    public abstract class InteractionModifier : MonoBehaviour, IInteractor
    {
        private InteractionHandler interactionHandler;
        public InteractionHandler InteractionHandler { get => interactionHandler; set => interactionHandler = value; }

        private void Awake()
        {
            interactionHandler = GetComponent<InteractionHandler>();
        }

        public virtual void StartInteraction()
        {
            interactionHandler.InteractionHand.IsGrabbing = true;
            UpdateInteraction();
        }

        public virtual void UpdateInteraction()
        {
            if (!interactionHandler.ClosestObject) return;
            if (interactionHandler.ClosestObject.IsGrabbed) return;

            if (!interactionHandler.HasObject && !interactionHandler.ClosestObject.IsPrecisionGrab)
            {
                interactionHandler.ClosestObject.SetNewRigidbodyValues(interactionHandler.ClosestObject.RigidBody);
                interactionHandler.GrabPointController.MoveToGrabPoint(interactionHandler.ClosestObject, interactionHandler.ClosestGrabPoint);
                StartCoroutine(interactionHandler.WaitUntilPositionReached());
            }
            else if (!interactionHandler.HasObject && interactionHandler.ClosestObject.IsPrecisionGrab)
            {
                interactionHandler.ClosestObject.Attach(transform);
            }

            interactionHandler.GrabbedObject = interactionHandler.ClosestObject;
            interactionHandler.OnGrab.Invoke();
        }

        public virtual void EndInteraction()
        {
            if (interactionHandler.GrabPointController.ActiveCoroutine != null) interactionHandler.GrabPointController.KillCoroutine();

            if (!interactionHandler.InteractionHand.IsGrabbing) return;
            interactionHandler.OnRelease.Invoke();
            interactionHandler.InteractionHand.IsGrabbing = false;

            if (!interactionHandler.GrabbedObject) return;
            interactionHandler.GrabbedObject.Detach();

            interactionHandler.GrabPointController.PositionReached = false;

            interactionHandler.GrabbedObject = null;

            interactionHandler.HasObject = false;
            DistanceCheck.CheckGrabbableObjectDistance(interactionHandler);
        }
    }
}
