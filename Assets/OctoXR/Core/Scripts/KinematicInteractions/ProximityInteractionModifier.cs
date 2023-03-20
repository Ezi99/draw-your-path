
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// An implementation of the InteractionModifier class, used to look for objects in the close proximity of the interactor.
    /// </summary>
    public class ProximityInteractionModifier : InteractionModifier
    {
        private Collider firstColliderHit;
        [SerializeField] private float overlapSphereRadius = 0.01f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Grabbable>())
            {
                InteractionHandler.OnHover.Invoke();
            }
        }

        public override void StartInteraction()
        {
            InteractionHandler.InteractionHand.IsGrabbing = true;

            var check = ContactCheck();

            if (check.Length > 0)
            {
                firstColliderHit = check[0];
                CheckForGrabbable(firstColliderHit);
                UpdateInteraction();
            }
        }

        public override void EndInteraction()
        {
            base.EndInteraction();
            CheckForRelease();
        }

        private void CheckForGrabbable(Collider foundCollider)
        {
            if (InteractionHandler.GrabbedObject) return;
            if (!foundCollider.attachedRigidbody) return;

            InteractionHandler.GrabPointController.AddGrabbableObject(foundCollider);
            InteractionHandler.GrabPointController.AddGrabPoint(foundCollider);
        }

        private void CheckForRelease()
        {
            if (!firstColliderHit) return;
            if (!firstColliderHit.attachedRigidbody) return;

            var grabbable = firstColliderHit.attachedRigidbody.GetComponent<Grabbable>();
            if (!grabbable) return;

            var grabPoints = grabbable.GetComponentsInChildren<GrabPoint>();

            if (grabbable && InteractionHandler.ObjectsInReach.Contains(grabbable))
            {
                InteractionHandler.GrabPointController.RemoveGrabbableObject(grabbable);
                InteractionHandler.ObjectsInReach.Remove(grabbable);
                InteractionHandler.GrabPointController.RemoveGrabPoint(grabPoints);
            }

            if (InteractionHandler.ObjectsInReach.Count == 0 && !grabbable.IsGrabbed)
            {
                InteractionHandler.ClosestObject = null;
                InteractionHandler.ClosestGrabPoint = null;
            }
        }

        public virtual Collider[] ContactCheck()
        {
            var contactResult = UnityEngine.Physics.OverlapSphere(transform.position, overlapSphereRadius, InteractionHandler.LayerMask);
            return contactResult;
        }
    }
}
