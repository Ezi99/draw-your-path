using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.HandPoseDetection
{
    public class HandPose : MonoBehaviour
    {
        [SerializeField] private HandPoseDetectionMode handPoseDetectionMode;
        [Space]
        public UnityEvent OnPoseDetected;
        public UnityEvent OnPoseLost;

        private IHandPoseComponent[] handPoseComponents;
        private bool wasPoseDetected;

        public HandPoseDetectionMode HandPoseDetectionMode { get => handPoseDetectionMode; set => handPoseDetectionMode = value; }

        private void Awake()
        {
            handPoseComponents = GetComponents<IHandPoseComponent>();

            if (handPoseComponents == null || handPoseComponents.Length == 0)
            {
                var log = LogUtility.FormatLogMessageFromComponent(this, "Missing Hand Pose Components!");
                Debug.LogError(log);
            }
        }

        private void Update()
        {
            var validComponents = 0;

            for (var i = 0; i < handPoseComponents.Length; i++)
            {
                if (handPoseComponents[i].Detect())
                {
                    validComponents++;
                }
            }

            var isPoseDetected = validComponents == handPoseComponents.Length;

            if (handPoseDetectionMode == HandPoseDetectionMode.Discrete && isPoseDetected && wasPoseDetected)
            {
                return;
            }

            if (isPoseDetected)
            {
                OnPoseDetected?.Invoke();
            }
            else
            {
                if (wasPoseDetected)
                {
                    OnPoseLost?.Invoke();
                }
            }

            wasPoseDetected = isPoseDetected;
        }

        public void InjectHandSkeleton(HandSkeleton handSkeleton)
        {
            for (var i = 0; i < handPoseComponents.Length; i++)
            {
                handPoseComponents[i].InjectHandSkeleton(handSkeleton);
            }
        }
    }
}
