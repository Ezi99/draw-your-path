using System;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    [Serializable]
    public class RotationManipulator : IManipulable<ScaleableGrabbableModifier>
    {
        private Quaternion initialRotation;
        private Quaternion previousFirstPointRotation;
        private Quaternion previousSecondPointRotation;
        private Quaternion targetRotation;
        private Quaternion activeRotation;
        private Quaternion finalRotation;

        private bool isFirstFrameOfRotation;

        public void Start(ScaleableGrabbableModifier modifier)
        {
            if (modifier.IsRotating) return;

            Vector3 diff = modifier.SecondPoint.position - modifier.FirstPoint.position;
            activeRotation = Quaternion.LookRotation(diff, Vector3.up).normalized;
            modifier.IsRotating = true;
            isFirstFrameOfRotation = true;
        }

        public void Stop(ScaleableGrabbableModifier modifier)
        {
            modifier.IsRotating = false;
            isFirstFrameOfRotation = false;
        }

        public void Update(ScaleableGrabbableModifier modifier)
        {
            var worldAxisCurrent = modifier.SecondPoint.position - modifier.FirstPoint.position;

            if (!isFirstFrameOfRotation)
            {
                initialRotation = activeRotation;
                var targetTransformRotation = modifier.transform.rotation;
                modifier.BaseRigidbody.isKinematic = false;

                modifier.BaseRigidbody.velocity = Vector3.zero;
                modifier.BaseRigidbody.angularVelocity = Vector3.zero;

                var previousUpAxis = initialRotation * Vector3.up;
                var newUpAxis = worldAxisCurrent.magnitude * previousUpAxis;

                var point1Angle = GetRotationDegrees(newUpAxis, previousFirstPointRotation, modifier.FirstPoint.rotation, worldAxisCurrent);
                var point1AngleHalf = point1Angle / 2;

                var point2Angle = GetRotationDegrees(newUpAxis, previousSecondPointRotation, modifier.SecondPoint.rotation, worldAxisCurrent);
                var point2AngleHalf = point2Angle / 2;

                var avgAngle = point1AngleHalf + point2AngleHalf;
                newUpAxis = Quaternion.AngleAxis(avgAngle, worldAxisCurrent) * previousUpAxis;

                targetRotation = Quaternion.LookRotation(worldAxisCurrent, newUpAxis);
                activeRotation = targetRotation;

                var rotationInTargetSpace = Quaternion.Inverse(initialRotation) * targetTransformRotation;
                finalRotation = targetRotation * rotationInTargetSpace;

                modifier.transform.rotation = finalRotation;
            }
            else
            {
                isFirstFrameOfRotation = false;
            }

            previousFirstPointRotation = modifier.FirstPoint.rotation;
            previousSecondPointRotation = modifier.SecondPoint.rotation;
        }

        private float GetRotationDegrees(Vector3 originalUpVector, Quaternion previousPointRotation, Quaternion currentPointRotation, Vector3 axis)
        {
            Vector3 newUpVector = (currentPointRotation * Quaternion.Inverse(previousPointRotation)) * originalUpVector;
            newUpVector = Vector3.ProjectOnPlane(newUpVector, axis);

            return Vector3.SignedAngle(originalUpVector, newUpVector, axis);
        }
    }
}
