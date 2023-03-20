using System;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    [Serializable]
    public class PositionManipulator : IManipulable<ScaleableGrabbableModifier>
    {
        public void Start(ScaleableGrabbableModifier modifier)
        {
            modifier.IsMoving = true;
        }

        public void Stop(ScaleableGrabbableModifier modifier)
        {
            modifier.IsMoving = false;
            modifier.PositionAnchor.transform.SetParent(modifier.transform);
            modifier.PositionAnchor.transform.localPosition = Vector3.zero;
        }

        public void Update(ScaleableGrabbableModifier modifier)
        {
            modifier.PositionAnchor.transform.SetParent(null);
            var middlePoint = ((modifier.SecondPoint.position + modifier.FirstPoint.position)) / 2;
            modifier.PositionAnchor.transform.position = middlePoint;
            modifier.transform.SetParent(modifier.PositionAnchor.transform);
        }
    }
}
