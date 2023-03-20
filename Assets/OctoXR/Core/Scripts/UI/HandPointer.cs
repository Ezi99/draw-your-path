using UnityEngine;
using OctoXR.Input;

namespace OctoXR.UI
{
    public class HandPointer : MonoBehaviour, IPointer
    {
        [SerializeField] private HandInputDataProvider handInputDataProvider;
        [Tooltip("The higher the number, the more stable the ray will be. High value also makes ray more body forward oriented (like the ray goes forward from your chest, not your hand).")]
        [SerializeField, Range(0, 10)] private float rayStabilizationMultiplier = 3;
        [Tooltip("The higher the value, the ray will be more hand forward oriented. High value also brings more sensitivity and less precision.")]
        [SerializeField, Range(0, 10)] private float rayForwardDirectionMultiplier = 1;

        private static Transform rayStart;
        private Transform palmCenter;
        public bool IsProviderTracking => handInputDataProvider.IsTracking;

        private void Start()
        {
            if (rayStart == null)
            {
                var uIRayStart = new GameObject("UI Ray Start");
                var cameraFollower = uIRayStart.AddComponent<CameraFollower>();
                cameraFollower.Offset = new Vector3(0, -0.30f, 0);
                rayStart = uIRayStart.transform;
            }
        }

        public float GetSelectActionStrength() => handInputDataProvider.Fingers[HandFinger.Index].PinchStrength;

        public Vector3 CalculateRayDirection() => 
            ((rayStabilizationMultiplier * (palmCenter.position - rayStart.position)) + (rayForwardDirectionMultiplier * palmCenter.forward)).normalized;

        public void InjectPalmCenter(Transform palmCenter) => this.palmCenter = palmCenter;
    }
}
