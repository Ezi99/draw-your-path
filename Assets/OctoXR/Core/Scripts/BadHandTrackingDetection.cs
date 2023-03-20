using OctoXR.Input;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Visual indicator for loss of confidence in hand tracking. 
    /// </summary>
    public class BadHandTrackingDetection : InputConfidenceTrigger
    {
        [Tooltip("Reference to the hand skeleton you want to disable when bad hand tracking is detected.")]
        [SerializeField] private PosingHandSkeleton handSkeleton;
        
        [Tooltip("Object used to notify the user when hand tracking is bad.")] 
        [SerializeField] private GameObject badTrackingCanvas;
        
        [Tooltip("Hand renderer whose material needs to be changed upon detecting bad tracking.")]
        public Renderer objectRenderer;

        private Material originalMaterial;

        [Tooltip("Material which the hand will change to when bad tracking is detected.")] 
        [SerializeField] private Material badTrackingMaterial;

        private void Start()
        {
            originalMaterial = objectRenderer.material;
        }

        protected override void HandleConfidenceLow()
        {
            handSkeleton.enabled = false;
            if (badTrackingCanvas) badTrackingCanvas.SetActive(true);
            if (objectRenderer) objectRenderer.material = badTrackingMaterial;

        }

        protected override void HandleConfidenceHigh()
        {
            handSkeleton.enabled = true;
            if (badTrackingCanvas) badTrackingCanvas.SetActive(false);
            if (objectRenderer) objectRenderer.material = originalMaterial;
        }
    }
}
