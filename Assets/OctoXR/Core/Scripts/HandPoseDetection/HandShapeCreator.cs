using UnityEngine;

namespace OctoXR.HandPoseDetection
{
    public class HandShapeCreator : MonoBehaviour
    {
        [SerializeField] private HandSkeleton handSkeleton;
        [SerializeField] private HandBones handBones;
        [SerializeField] private HandShape targetPose;

        public void CreateShape()
        {
            CreateShape(handSkeleton, handBones, targetPose);
        }

        public void CreateShape(HandSkeleton handSkeleton, HandBones handBones, HandShape targetPose)
        {
            var bones = handSkeleton.Bones;

            for (var i = 0; i < bones.Count; i++)
            {
                var handBone = bones[i];
                var handBoneId = (int)handBone.BoneId;
                var writeBone = (HandBones)(1 << handBoneId);

                if ((handBones & writeBone) != HandBones.None)
                {
                    targetPose.AddBonePosition(handSkeleton, handBone);
                }

#if UNITY_EDITOR
                if (Application.IsPlaying(targetPose))
                {
                    if (UnityEditor.EditorUtility.IsPersistent(targetPose))
                    {
                        UnityEditor.EditorUtility.SetDirty(targetPose);
                    }
                }
                else
                {
                    UnityEditor.EditorUtility.SetDirty(targetPose);
                }
#endif
            }
        }
    }
}
