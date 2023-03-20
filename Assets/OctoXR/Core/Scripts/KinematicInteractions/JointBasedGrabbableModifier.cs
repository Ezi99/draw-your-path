using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// An implementation of the class which gives the grabbable joint based properties.
    /// </summary>
    public class JointBasedGrabbableModifier : GrabbableModifier
    {
        [Header("Joint values")]
        [SerializeField]
        private float spring = 3000;

        [SerializeField] private float angularSpring = 1000;
        [SerializeField] private float damper = 50;
        [SerializeField] private float maximumAllowedForce = 10000;
        [SerializeField] private float breakForce = 5000;
        [SerializeField] private float allowedMaximumDistance;

        private ConfigurableJoint configurableJoint;
        private ConfigurableJointMotion configurableJointMotion;

        public override void Attach(Transform grabParent)
        {
            base.Attach(grabParent);
            Grabbable.SetDefaultRigidbodyValues(Grabbable.RigidBody, transform);
            Grabbable.InteractionHandler = grabParent.GetComponent<InteractionHandler>();
            CreateJoint();
        }

        private void CreateJoint()
        {
            if (configurableJoint != null)
            {
                Destroy(configurableJoint);
                configurableJoint = null;
            }

            var rb = gameObject.GetComponent<Rigidbody>();
            transform.parent = Grabbable.InteractionHandler.InteractionHand.TrackingSpace;

            configurableJoint = Grabbable.CurrentGrabber.InteractionHand.HandSkeleton.gameObject.AddComponent<ConfigurableJoint>();
            configurableJoint.connectedBody = rb;

            configurableJoint.autoConfigureConnectedAnchor = true;
            configurableJoint.enablePreprocessing = true;

            transform.InverseTransformPoint(Grabbable.CurrentGrabber.transform.position);

            configurableJointMotion = ConfigurableJointMotion.Locked;

            rb.useGravity = false;

            configurableJoint.xMotion = configurableJoint.yMotion = configurableJoint.zMotion = configurableJointMotion;
            configurableJoint.angularXMotion = configurableJoint.angularYMotion =
                configurableJoint.angularZMotion = configurableJointMotion;

            SetMotionDrive();
            SetAngularDrive();
            SetBreakForces(breakForce);

            Grabbable.IsGrabbed = true;
        }

        private void SetBreakForces(float breakForce)
        {
            configurableJoint.breakForce = breakForce;
            configurableJoint.breakTorque = breakForce;
        }

        private void SetMotionDrive()
        {
            var motionDrive = new JointDrive
            {
                positionSpring = spring,
                positionDamper = damper,
                maximumForce = maximumAllowedForce
            };
            configurableJoint.xDrive = configurableJoint.yDrive = configurableJoint.zDrive = motionDrive;
        }

        private void SetAngularDrive()
        {
            var angularDrive = new JointDrive
            {
                positionSpring = angularSpring,
                positionDamper = damper,
                maximumForce = maximumAllowedForce
            };
            configurableJoint.angularXDrive = configurableJoint.angularYZDrive = angularDrive;
        }

        public override void Detach()
        {
            base.Detach();

            Destroy(configurableJoint);
            configurableJoint = null;
            Grabbable.RigidBody.useGravity = true;
            Grabbable.IsGrabbed = false;
            Grabbable.SetDefaultRigidbodyValues(Grabbable.RigidBody, transform);

            transform.parent = null;

            Grabbable.OnDetach.Invoke();
        }
    }
}
