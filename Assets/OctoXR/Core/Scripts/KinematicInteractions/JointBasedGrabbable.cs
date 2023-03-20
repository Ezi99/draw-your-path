using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// A type of grabbable which interact with the world and other grabbed objects around it.
    /// </summary>
    [System.Obsolete("This class is no longer used, please attach the Grabbable class and add a JointBasedGrabbableModifier to it instead!")]
    public class JointBasedGrabbable : Grabbable
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

        private void Start()
        {
            GetDefaultRigidbodyValues(RigidBody);
        }

        private void Update()
        {
            if (IsGrabbed && !IsPrecisionGrab) CheckDistanceFromGrabController();
        }

        public override void Attach(Transform grabParent)
        {
            base.Attach(grabParent);
            SetDefaultRigidbodyValues(RigidBody, transform);

            InteractionHandler = grabParent.GetComponent<InteractionHandler>();

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
            transform.parent = InteractionHandler.InteractionHand.TrackingSpace;

            configurableJoint =
                CurrentGrabber.InteractionHand.HandSkeleton.gameObject.AddComponent<ConfigurableJoint>();
            configurableJoint.connectedBody = rb;

            configurableJoint.autoConfigureConnectedAnchor = true;
            configurableJoint.enablePreprocessing = true;

            transform.InverseTransformPoint(CurrentGrabber.transform.position);

            configurableJointMotion = ConfigurableJointMotion.Locked;

            rb.useGravity = false;

            configurableJoint.xMotion = configurableJoint.yMotion = configurableJoint.zMotion = configurableJointMotion;
            configurableJoint.angularXMotion = configurableJoint.angularYMotion =
                configurableJoint.angularZMotion = configurableJointMotion;

            SetMotionDrive();
            SetAngularDrive();
            SetBreakForces(breakForce);

            IsGrabbed = true;
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
            RigidBody.useGravity = true;
            IsGrabbed = false;
            SetDefaultRigidbodyValues(RigidBody, transform);

            transform.parent = null;

            OnDetach.Invoke();
        }

        private void CheckDistanceFromGrabController()
        {
            var distance = (CurrentGrabber.transform.position - transform.position).sqrMagnitude;
            if (distance > allowedMaximumDistance) CurrentGrabber.EndInteraction();
        }
    }
}
