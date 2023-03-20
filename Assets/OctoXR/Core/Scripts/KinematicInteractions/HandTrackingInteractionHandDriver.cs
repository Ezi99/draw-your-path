using OctoXR.Collections;
using OctoXR.Input;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// An implementation of the InteractionHandDriver class used for grab detection while using hand tracking.
    /// </summary>
    /// <param name="HandTrackingInteractionHandDriver"></param>

    public class HandTrackingInteractionHandDriver : InteractionHandDriver
    {
        [Tooltip("Dictates how much of a grip your hand needs to have in order to grab.")]

        [Range(0.09f, 0.12f)]
        [SerializeField] private float grabThreshold = 0.09f;

        [Tooltip("Reference to the hand data provider, necessary to check for pinching.")]
        [SerializeField] private HandInputDataProvider handInputDataProvider;

        private HandBoneKeyedReadOnlyCollection<Pose> bonePoses;

        private void Start()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Detects grabbing when the distance between fingers and wrists goes below a certain threshold (0.05f by default).
        /// </summary>
        public override void DetectGrab()
        {
            if (!InteractionHand.IsPinch)
            {
                bonePoses = handInputDataProvider.GetBoneAbsolutePoses();
                var wrist = bonePoses[0];
                var distanceBetweenFingersAndWrist = 0.0f;

                foreach (var bonePose in bonePoses)
                {
                    distanceBetweenFingersAndWrist += Mathf.Abs((wrist.position - bonePose.position).magnitude);
                }

                var averageDistance = distanceBetweenFingersAndWrist / bonePoses.Count;
                InteractionHand.ShouldGrab = averageDistance <= grabThreshold;
            }

            if (InteractionHand.IsPinch)
            {
                DetectPinch(HandFinger.Index);
            }
        }

        /// <summary>
        /// Detects pinching of the given finger from the referenced hand data provider.
        /// </summary>
        /// <param name="finger"></param>
        /// <returns></returns>
        public void DetectPinch(HandFinger finger)
        {
            InteractionHand.ShouldGrab = handInputDataProvider.Fingers[finger].IsPinching;
        }
    }
}