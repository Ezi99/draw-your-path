using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    [System.Obsolete("This class is no longer used, please attach the Grabbable class and add a KinematicGrabbableModifier to it instead!")]
    public class KinematicGrabbable : Grabbable
    {
        public override void Attach(Transform grabParent)
        {
            base.Attach(grabParent);

            transform.SetParent(grabParent);

            SetNewRigidbodyValues(RigidBody);
            IsGrabbed = true;
        }

        public override void Detach()
        {
            base.Detach();
            IsGrabbed = false;
            SetDefaultRigidbodyValues(RigidBody, transform);

            OnDetach.Invoke();
        }
    }
}
