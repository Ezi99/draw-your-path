using System;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Stores info about the newly created hand poses.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "NewHandPose", menuName = "OctoXR/Kinematic Interactions/Hand Pose")]
    public class CustomHandPose : ScriptableObject
    {
        public Vector3 PosePosition;
        public Quaternion PoseRotation;
        public List<Quaternion> FingerRotations;

        /// <summary>
        /// Used to save pose data to a scriptable object for later use.
        /// </summary>
        /// <param name="poseData"></param>
        public void SetHandPoseData(PoseData poseData)
        {
            PosePosition = poseData.PosePosition;
            PoseRotation = poseData.PoseRotation;
            FingerRotations = poseData.FingerRotations;
        }
    }
}
