using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Stores information about the rotation of the pose as well as It's fingers.
    /// </summary>
    public class PoseData : MonoBehaviour
    {
        [HideInInspector] public Vector3 PosePosition;
        [HideInInspector] public Quaternion PoseRotation;
        [HideInInspector] public List<Quaternion> FingerRotations;

        /// <summary>
        /// Saves the information about the local position and rotation of the given preview hand.
        /// </summary>
        /// <param name="hand"></param>
        public void Save(PreviewHand hand)
        {
            PosePosition = hand.transform.localPosition;
            PoseRotation = hand.transform.localRotation;

            FingerRotations = hand.GetJointRotations();
        }

        public void SaveInRuntime(PoseableHand hand)
        {
            PosePosition = hand.transform.localPosition;
            PoseRotation = hand.transform.localRotation;

            FingerRotations = hand.GetJointRotations();
        }

        /// <summary>
        /// Gets the info about pose rotation, position and the rotation of the fingers from the given pose.
        /// </summary>
        /// <param name="customHandPose"></param>
        public void GetPoseData(CustomHandPose customHandPose)
        {
            PosePosition = customHandPose.PosePosition;
            PoseRotation = customHandPose.PoseRotation;
            FingerRotations = customHandPose.FingerRotations;
        }
    }
}
