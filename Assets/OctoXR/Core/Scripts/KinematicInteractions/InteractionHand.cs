using System;
using OctoXR.Input;
using OctoXR.Integrations.Oculus.Input;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Script that detects which input method is currently used (hand tracking or controllers), and switches between them in runtime.
    /// </summary>
    public class InteractionHand : MonoBehaviour
    {
        [SerializeField] private Transform trackingSpace;
        public Transform TrackingSpace { get => trackingSpace; set => trackingSpace = value; }

        [SerializeField] private Transform palmCenter;
        [SerializeField] private InteractionHandler interactionHandler;
        [SerializeField] private HandSkeleton handSkeleton;
        public HandSkeleton HandSkeleton { get => handSkeleton; set => handSkeleton = value; }
        public InteractionHandler InteractionHandler { get => interactionHandler; set => interactionHandler = value; }

        [SerializeField] private HandType handSkeletonType;
        public HandType HandSkeletonType { get => handSkeletonType; set => handSkeletonType = value; }

        [HideInInspector][SerializeField] private bool shouldGrab;
        public bool ShouldGrab { get => shouldGrab; set => shouldGrab = value; }

        [HideInInspector][SerializeField] private bool isGrabbing;
        public bool IsGrabbing { get => isGrabbing; set => isGrabbing = value; }

        [Tooltip("Tick if you want to pinch objects instead of grabbing them, this disables snap grab.")]
        [SerializeField] private bool isPinch;
        public bool IsPinch { get => isPinch; set => isPinch = value; }

        [SerializeField] private MultiSourceInputDataProvider multiSourceInputDataProvider;
        public MultiSourceInputDataProvider MultiSourceInputDataProvider { get => multiSourceInputDataProvider; set => multiSourceInputDataProvider = value; }

        [SerializeField] private HandTrackingInteractionHandDriver handTrackingInteractionHandDriver;
        [SerializeField] private ControllerInteractionHandDriver controllerInteractionHandDriver;

        private UnityXRControllerInputDataProvider unityXRControllerInputDataProvider;
        private OVRHandInputDataProvider ovrHandInputDataProvider;

        public UnityEvent OnHandTrackingActivated;
        public UnityEvent OnControllersActivated;

        private void Awake()
        {
            transform.SetParent(palmCenter);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        private void Start()
        {
            foreach (var inputDataProvider in MultiSourceInputDataProvider.SourceInputDataProviders)
            {
                var controllerInputDataProvider = inputDataProvider.GetComponent<UnityXRControllerInputDataProvider>();
                if (inputDataProvider.GetComponent<UnityXRControllerInputDataProvider>())
                {
                    unityXRControllerInputDataProvider = controllerInputDataProvider;
                }

                var handInputDataProvider = inputDataProvider.GetComponent<OVRHandInputDataProvider>();
                if (inputDataProvider.GetComponent<OVRHandInputDataProvider>())
                {
                    ovrHandInputDataProvider = handInputDataProvider;
                }
            }

            unityXRControllerInputDataProvider.OnTrackingStart.AddListener(ActivateControllers);
            ovrHandInputDataProvider.OnTrackingStart.AddListener(ActivateHandTracking);
        }

        private void ActivateHandTracking()
        {
            handTrackingInteractionHandDriver.enabled = true;
            controllerInteractionHandDriver.enabled = false;

            OnHandTrackingActivated.Invoke();
        }

        private void ActivateControllers()
        {
            handTrackingInteractionHandDriver.enabled = false;
            controllerInteractionHandDriver.enabled = true;

            OnControllersActivated.Invoke();
        }
    }
}
