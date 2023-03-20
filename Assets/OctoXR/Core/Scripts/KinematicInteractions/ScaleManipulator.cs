using System;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    [Serializable]
    public class ScaleManipulator : IManipulable<ScaleableGrabbableModifier>
    {
        private float scaleStartingDistance;
        public float ScaleStartingDistance { get => scaleStartingDistance; set => scaleStartingDistance = value; }

        private float scaleStartingValue;
        private float currentScaleFactor = 1f;

        public void Start(ScaleableGrabbableModifier modifier)
        {
            modifier.IsScaling = true;
            scaleStartingDistance = modifier.GrabDistance;
            scaleStartingValue = currentScaleFactor;
            modifier.LastFrameScale = modifier.transform.localScale.magnitude;
        }

        public void Stop(ScaleableGrabbableModifier modifier)
        {
            modifier.IsScaling = false;
            scaleStartingValue = 0;
            modifier.LastFrameScale = modifier.transform.localScale.magnitude;
        }

        public void Update(ScaleableGrabbableModifier modifier)
        {
            var newScaleFactor = scaleStartingValue + (modifier.PinchDelta * modifier.ScaleDistanceFactor);

            if (newScaleFactor > modifier.MinimumScaleFactor && newScaleFactor < modifier.MaximumScaleFactor)
            {
                currentScaleFactor = newScaleFactor;
                modifier.transform.localScale = Vector3.Lerp(modifier.transform.localScale, modifier.Grabbable.InitialLocalScale * newScaleFactor, 10);
            }
        }
    }
}
