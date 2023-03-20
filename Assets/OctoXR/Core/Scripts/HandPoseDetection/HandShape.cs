using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.HandPoseDetection
{
    [CreateAssetMenu(fileName = "HandShape", menuName = "OctoXR/Hand Shape")]
    public class HandShape : ScriptableObject
    {
        [Tooltip("If the distance between current bone position and saved bone position is below threshold, hand shape will be detected. " +
            "The lower the threshold, the more precise shape detection will be (but harder to get right).")]
        [SerializeField, Range(0.0001f, 0.01f)] private float threshold = 0.005f;
        [SerializeField] private List<HandShapeData> shapeDatas;
        private HandType handType;

        public void AddBonePosition(HandSkeleton handSkeleton, HandBone handBone)
        {
            for (var i = 0; i < shapeDatas.Count; i++)
            {
                if (shapeDatas[i].HandBoneId == handBone.BoneId)
                {
                    shapeDatas[i].RelativeBonePosition = handSkeleton.Transform.InverseTransformPoint(handBone.Transform.position);

                    return;
                }
            }

            var shapeData = new HandShapeData(handBone.BoneId, handSkeleton.Transform.InverseTransformPoint(handBone.Transform.position));
            shapeDatas.Add(shapeData);

            if (handType == 0 || handType != handSkeleton.HandType)
            {
                handType = handSkeleton.HandType;
            }
        }

        public bool IsDetected(HandSkeleton handSkeleton)
        {
            var bones = handSkeleton.Bones;
            var handSkeletonTransform = handSkeleton.Transform;

            for (var i = 0; i < shapeDatas.Count; i++)
            {
                var bone = bones[shapeDatas[i].HandBoneId];

                var position = handSkeletonTransform.InverseTransformPoint(bone.Transform.position);

                Vector3 bonePosition = shapeDatas[i].RelativeBonePosition;

                if (handType != handSkeleton.HandType)
                {
                    bonePosition = new Vector3(-bonePosition.x, bonePosition.y, bonePosition.z);
                }

                var isInThreshold = (position - bonePosition).sqrMagnitude < threshold;

                if (!isInThreshold) return false;
            }

            return true;
        }
    }
}
