using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// An implementation of the class which gives the grabbable object kinematic properties.
    /// </summary>
    public class KinematicGrabbableModifier : GrabbableModifier
    {
        public override void Attach(Transform grabParent)
        {
            base.Attach(grabParent);

            transform.SetParent(grabParent);

            Grabbable.SetNewRigidbodyValues(Grabbable.RigidBody);
            Grabbable.IsGrabbed = true;
        }

        public override void Detach()
        {
            base.Detach();
            Grabbable.IsGrabbed = false;
            Grabbable.SetDefaultRigidbodyValues(Grabbable.RigidBody, transform);

            Grabbable.OnDetach.Invoke();
        }
    }
}
