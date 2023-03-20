using OctoXR.Collections;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    public class PosingHandSkeleton : HandSkeleton
    {
        [SerializeField] private bool shouldUpdateTransforms = true;
        public bool ShouldUpdateTransforms { get => shouldUpdateTransforms; set => shouldUpdateTransforms = value; }

        [SerializeField] private bool shouldRunUpdate = true;
        public bool ShouldRunUpdate { get => shouldRunUpdate; set => shouldRunUpdate = value; }

        protected virtual void FixedUpdate()
        {
            RunUpdate();
        }

        private void RunUpdate()
        {
            if (shouldUpdateTransforms)
            {
                UpdatePose();
            }

            if (!shouldUpdateTransforms)
            {
                UpdatePoseWithoutBoneTransforms();
            }

            SetBoneBindPoses();
            FinalizePoseUpdate();
        }

        private void UpdatePoseWithoutBoneTransforms()
        {
            if (!PoseProvider)
            {
                SetBoneScalesAndScaledPositions(Scale);

                return;
            }

            var totalScale = Scale;
            HandBoneKeyedReadOnlyCollection<Pose> bonePoses;

            if (ApplyPoseProviderScale)
            {
                totalScale *= PoseProvider.Scale;
                bonePoses = PoseProvider.GetBoneAbsoluteScaledPoses();
            }
            else
            {
                bonePoses = PoseProvider.GetBoneAbsolutePoses();
            }

            var rootPose = bonePoses[0];

            transform.SetPositionAndRotation(rootPose.position, rootPose.rotation);
            transform.localScale = GetBoneScale(transform, totalScale);

            for (var i = 1; i < Bones.Count; i++)
            {
                var bone = Bones[i];
                var boneTransform = bone.Transform;

                bonePoses.GetItem(bone.BoneId, out var bonePose);

                if (shouldUpdateTransforms)
                {
                    boneTransform.SetPositionAndRotation(
                        position: rootPose.position + Scale * (bonePose.position - rootPose.position),
                        rotation: bonePose.rotation);
                }

                boneTransform.localScale = GetBoneScale(boneTransform, totalScale);
            }
        }
    }
}