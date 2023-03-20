using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Abstract class housing logic for checking if conditions for grab or release of an object are met.
    /// </summary>
    [RequireComponent(typeof(GrabPointController))]
    public class InteractionHandler : MonoBehaviour, IInteractor
    {
        #region References
        [SerializeField] private InteractionModifier interactorModifier;
        public InteractionModifier InteractorModifier { get => interactorModifier; set => interactorModifier = value; }

        [Tooltip("Reference to the InteractionHand, script that houses private methods for activating hand tracking/controller drivers.")]
        [SerializeField] private InteractionHand interactionHand;
        public InteractionHand InteractionHand { get => interactionHand; set => interactionHand = value; }

        [Tooltip("Reference to the GrabPointController, necessary for kinematic grabbing with snapping.")]
        [SerializeField] private GrabPointController grabPointController;
        public GrabPointController GrabPointController { get => grabPointController; set => grabPointController = value; }

        [Tooltip("Layer that you want the interactor to interact with.")]
        [SerializeField] private LayerMask layerMask;
        public LayerMask LayerMask { get => layerMask; set => layerMask = value; }

        private List<Grabbable> objectsInReach = new List<Grabbable>();
        public List<Grabbable> ObjectsInReach { get => objectsInReach; set => objectsInReach = value; }

        private List<GrabPoint> grabPointsInReach = new List<GrabPoint>();
        public List<GrabPoint> GrabPointsInReach { get => grabPointsInReach; set => grabPointsInReach = value; }

        private GrabPoint closestGrabPoint;
        public GrabPoint ClosestGrabPoint { get => closestGrabPoint; set => closestGrabPoint = value; }

        private Grabbable grabbedObject;
        public Grabbable GrabbedObject { get => grabbedObject; set => grabbedObject = value; }

        private Grabbable closestObject;
        public Grabbable ClosestObject { get => closestObject; set => closestObject = value; }

        private Grabbable lastClosestObject;
        public Grabbable LastClosestObject { get => lastClosestObject; set => lastClosestObject = value; }

        [SerializeField] private PosingHandSkeletonRuntimeHand handTrackingRuntimeHand;

        public UnityEvent OnHover;
        public UnityEvent OnUnhover;
        public UnityEvent OnGrab;
        public UnityEvent OnRelease;
        #endregion

        private void Awake()
        {
            if (interactorModifier == null) interactorModifier = gameObject.AddComponent<ProximityInteractionModifier>();    
        }

        public virtual void Start()
        {
            grabPointController = GetComponent<GrabPointController>();
        }

        public virtual void Update()
        {
            GrabCheck();
        }

        /// <summary>
        /// Checks whether all of the conditions for a successful grab are met and if they are, triggers the grab logic.
        /// </summary>
        public virtual void GrabCheck()
        {
            if (!interactionHand.IsGrabbing && interactionHand.ShouldGrab) StartInteraction();

            if (!InteractionHand.ShouldGrab && InteractionHand.IsGrabbing) EndInteraction();
        }

        private bool hasObject;
        public bool HasObject { get => hasObject; set => hasObject = value; }

        public virtual void StartInteraction()
        {
            interactorModifier.StartInteraction();
        }

        /// <summary>
        /// Handles grabbing and triggers OnGrab events.
        /// </summary>
        public virtual void UpdateInteraction()
        {
            interactorModifier.UpdateInteraction();
        }
        public IEnumerator WaitUntilPositionReached()
        {
            yield return new WaitUntil(() => grabPointController.PositionReached);
            closestObject.Attach(transform);

            hasObject = true;
        }

        /// <summary>
        /// Handles releasing and triggers OnRelease events.
        /// </summary>
        public virtual void EndInteraction()
        {
            interactorModifier.EndInteraction();
        }
    }
}
