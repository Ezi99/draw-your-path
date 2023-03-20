using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    public class PosingHandSkeletonRuntimeHand : RuntimeHand
    {
        [SerializeField] private PosingHandSkeleton posingHandSkeleton;
        [SerializeField] private float rotationSpeed = 1f;

        [Tooltip("Reference to the grab controller which does the grabbing, this is automatically set on enabling the grab controller which has the reference to this component.")]
        private InteractionHandler interactionHandler;
        public InteractionHandler InteractionHandler { get => interactionHandler; set => interactionHandler = value; }

        [SerializeField] private InteractionHand interactionHand;

        private HandTrackingInteractionHandDriver handTrackingInteractionHandDriver;
        public HandTrackingInteractionHandDriver HandTrackingInteractionHandDriver { get => handTrackingInteractionHandDriver; set => handTrackingInteractionHandDriver = value; }
              
        private GrabPoint grabPoint;
        public GrabPoint GrabPoint { get => grabPoint; set => grabPoint = value; }

        protected static readonly Vector3 palmCenterOffset = new Vector3(-0.0042f, -0.0811f, -0.0189f);

        protected override void Awake()
        {
            handTrackingInteractionHandDriver = interactionHand.GetComponent<HandTrackingInteractionHandDriver>();
            interactionHandler = interactionHand.InteractionHandler;
            HandType = posingHandSkeleton.HandType;

            base.Awake();
        }

        private void Start()
        {
            AddListeners(interactionHandler);
        }

        private void AddListeners(InteractionHandler interactionHandler)
        {
            interactionHandler.OnGrab.AddListener(ApplyPoseAtRuntime);
            interactionHandler.OnRelease.AddListener(DisablePoseAtRuntime);
        }

        private void RemoveListeners(InteractionHandler interactionHandler)
        {
            interactionHandler.OnGrab.RemoveListener(ApplyPoseAtRuntime);
            interactionHandler.OnRelease.RemoveListener(DisablePoseAtRuntime);
        }

        public void ApplyPoseAtRuntime()
        {
            GrabPoint = interactionHandler.ClosestGrabPoint;
            if (!GrabPoint) return;

            if (!GrabPoint.GrabPose) return;

            if (!GrabPoint.ParentGrabbable.IsPrecisionGrab)
            {
                posingHandSkeleton.ShouldUpdateTransforms = false;
                ApplyPose(GrabPoint.GrabPose, GrabPoint.IsPoseInverted, rotationSpeed, interactionHand);
            }
        }

        public void DisablePoseAtRuntime()
        {
            posingHandSkeleton.ShouldUpdateTransforms = true;
        }
    }
}
