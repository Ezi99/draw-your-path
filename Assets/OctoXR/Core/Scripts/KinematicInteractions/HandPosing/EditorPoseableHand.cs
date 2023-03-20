using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    public class EditorPoseableHand : PoseableHand
    {
        [SerializeField] protected CustomHandPose CurrentHandPose;

        protected void ApplyPose(CustomHandPose customHandPose, bool isInverted)
        {
            var poseData = GetComponent<PoseData>();
            poseData.GetPoseData(customHandPose);

            switch (isInverted)
            {
                case false:
                    ApplyFingerRotations(poseData.FingerRotations);
                    break;
                case true:
                    ApplyMirroredFingerRotations(poseData.FingerRotations);
                    break;
            }
        }

        private void ApplyFingerRotations(List<Quaternion> rotations)
        {
            if (!HasProperCount(rotations)) return;

            for (var i = 0; i < Joints.Count; i++)
                Joints[i].localRotation = rotations[i];
        }

        private void ApplyMirroredFingerRotations(List<Quaternion> rotations)
        {
            if (!HasProperCount(rotations)) return;

            for (var i = 0; i < Joints.Count; i++)
            {
                var invertedQuaternion = InvertQuaternion(rotations, i);
                Joints[i].localRotation = invertedQuaternion;
            }
        }
    }
}
