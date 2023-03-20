
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    [System.Obsolete("This class is no longer used, please attach the InteractionHandler class and add a ProximityInteractionModifier to it instead!")]
    public class ProximityGrabController : InteractionHandler
    {
        private Collider firstColliderHit;

        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float overlapSphereRadius = 0.01f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Grabbable>())
            {
                OnHover.Invoke();
            }
        }

        public override void StartInteraction()
        {
            InteractionHand.IsGrabbing = true;

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
            if (GrabbedObject) return;
            if (!foundCollider.attachedRigidbody) return;

            GrabPointController.AddGrabbableObject(foundCollider);
            GrabPointController.AddGrabPoint(foundCollider);
        }

        private void CheckForRelease()
        {
            //if (!firstColliderHit.attachedRigidbody) return;

            var grabbable = firstColliderHit.attachedRigidbody.GetComponent<Grabbable>();
            if (!grabbable) return;

            var grabPoints = grabbable.GetComponentsInChildren<GrabPoint>();

            if (grabbable && ObjectsInReach.Contains(grabbable))
            {
                GrabPointController.RemoveGrabbableObject(grabbable);
                ObjectsInReach.Remove(grabbable);
                GrabPointController.RemoveGrabPoint(grabPoints);
            }

            if (ObjectsInReach.Count == 0 && !grabbable.IsGrabbed)
            {
                ClosestObject = null;
                ClosestGrabPoint = null;
            }
        }

        public virtual Collider[] ContactCheck()
        {
            var contactResult = UnityEngine.Physics.OverlapSphere(transform.position, overlapSphereRadius, layerMask);
            return contactResult;
        }
    }
}   
