using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.HandPoseDetection
{
    public class HandPoseHandRef : MonoBehaviour
    {
        [SerializeField] private HandSkeleton handSkeleton;
        [SerializeField] private List<HandPose> handPoses;

        private void Start()
        {
            InjectHandSkeletonInHandPoses(handSkeleton);
        }

        public void InjectHandSkeletonInHandPoses(HandSkeleton skeleton)
        {
            for (var i = 0; i < handPoses.Count; i++)
            {
                handPoses[i].InjectHandSkeleton(skeleton);
            }
        }

        public void AddHandPose(HandPose handPose)
        {
            handPoses ??= new List<HandPose>();

            handPoses.Add(handPose);
        }
    }
}
