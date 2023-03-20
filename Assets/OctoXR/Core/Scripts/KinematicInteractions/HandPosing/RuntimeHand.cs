using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    public abstract class RuntimeHand : PoseableHand
    {
        /// <summary>
        /// Applies the given hand pose to the target.
        /// </summary>
        /// <param name="customHandPose"></param>
        protected void ApplyPose(CustomHandPose customHandPose, bool isInverted, float lerpDuration, InteractionHand interactionHand)
        {
            var poseData = GetComponent<PoseData>();
            poseData.GetPoseData(customHandPose);

            if (!isInverted) ApplyFingerRotations(poseData.FingerRotations, lerpDuration, interactionHand);
            if (isInverted) ApplyMirroredFingerRotations(poseData.FingerRotations, lerpDuration, interactionHand);
        }

        /// <summary>
        /// Applies the given finger rotations to all of the joints.
        /// </summary>
        /// <param name="rotations"></param>
        private void ApplyFingerRotations(List<Quaternion> rotations, float lerpDuration, InteractionHand interactionHand)
        {
            StartCoroutine(ApplyFingerRotationCoroutine(rotations, lerpDuration, interactionHand));
        }

        private IEnumerator ApplyFingerRotationCoroutine(List<Quaternion> rotations, float lerpDuration, InteractionHand interactionHand)
        {
            float time = 0;

            if (HasProperCount(rotations))
            {
                while (time < lerpDuration && interactionHand.ShouldGrab)
                {
                    for (var i = 0; i < Joints.Count; i++)
                        Joints[i].localRotation = Quaternion.Lerp(Joints[i].localRotation, rotations[i], time / lerpDuration);

                    time += Time.deltaTime;
                    yield return null;
                }

                for (var i = 0; i < Joints.Count; i++) Joints[i].localRotation = rotations[i];
            }
        }

        private void ApplyMirroredFingerRotations(List<Quaternion> rotations, float lerpDuration, InteractionHand interactionHand)
        {
            StartCoroutine(ApplyMirroredFingerRotationsCoroutine(rotations, lerpDuration, interactionHand));
        }

        private IEnumerator ApplyMirroredFingerRotationsCoroutine(List<Quaternion> rotations, float lerpDuration, InteractionHand interactionHand)
        {
            float time = 0;

            if (HasProperCount(rotations))
            {
                while (time < lerpDuration && interactionHand.ShouldGrab)
                {
                    for (var i = 0; i < Joints.Count; i++)
                    {
                        var invertedQuaternion = InvertQuaternion(rotations, i);
                        Joints[i].localRotation = Quaternion.Lerp(Joints[i].localRotation, invertedQuaternion, time / lerpDuration);
                    }

                    time += Time.deltaTime;
                    yield return null;
                }

                for (var i = 0; i < Joints.Count; i++)
                {
                    var invertedQuaternion = InvertQuaternion(rotations, i);
                    Joints[i].localRotation = invertedQuaternion;
                }
            }
        }
    }
}
