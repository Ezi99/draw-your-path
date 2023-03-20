using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OctoXR.Collections;

namespace OctoXR
{
    /// <summary>
    /// Hand skeleton pose provider that gets its poses from a hand skeleton
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(HandSkeleton))]
    public class HandSkeletonSourcedPoseProvider : HandSkeletonPoseProvider
    {
        [SerializeField]
        [HideInInspector]
        private HandSkeleton handSkeleton;
        /// <summary>
        /// Hand skeleton that serves as source of poses provided by the pose provider
        /// </summary>
        public HandSkeleton HandSkeleton => handSkeleton;

        private bool doNotUpdatePoseProviderState;
        private UnityAction handSkeletonPoseUpdatedHandler;
        
        private static readonly List<HandBone>[] handFingerBones = new List<HandBone>[]
            {
                new List<HandBone>(HandSkeletonConfiguration.BonesPerFinger),
                new List<HandBone>(HandSkeletonConfiguration.BonesPerFinger),
                new List<HandBone>(HandSkeletonConfiguration.BonesPerFinger),
                new List<HandBone>(HandSkeletonConfiguration.BonesPerFinger),
                new List<HandBone>(HandSkeletonConfiguration.BonesPerFinger)
            };

        private readonly Pose[] poses = new Pose[HandSkeletonConfiguration.BoneCount];

#if UNITY_EDITOR
        private bool logSourceHandSkeletonNotCompleteAndWithoutPoseProvider;
#endif
        public sealed override HandType GetHandType() => handSkeleton.HandType;

        private void Reset()
        {
            handSkeleton = GetComponent<HandSkeleton>();
        }

        private void OnValidate()
        {
            InitializeHandSkeleton();
        }

        private void Awake()
        {
            InitializeHandSkeleton();
        }

        private void InitializeHandSkeleton()
        {
            if (!handSkeleton || handSkeleton.gameObject != gameObject)
            {
                handSkeleton = GetComponent<HandSkeleton>();
            }
        }

        private void OnEnable()
        {
            SubscribeHandSkeletonPoseUpdatedHandler();
            doNotUpdatePoseProviderState = false;
        }

        private void OnDisable()
        {
            UnsubscribeHandSkeletonPoseUpdatedHandler();
            doNotUpdatePoseProviderState = true;
        }

        private void LogSourceHandSkeletonNotCompleteAndWithoutPoseProvider()
        {
            Debug.LogWarning(LogUtility.FormatLogMessageFromComponent(
                this,
                "Hand skeleton used as a source for the pose provider's poses does not have a pose provider assigned and it is not complete. " +
                "Hand skeleton sourced pose provider cannot function with such hand skeleton"));
        }

        private void SubscribeHandSkeletonPoseUpdatedHandler()
        {
            UnsubscribeHandSkeletonPoseUpdatedHandler();

            handSkeleton.OnPoseUpdated.AddListener(handSkeletonPoseUpdatedHandler);
        }

        private void UnsubscribeHandSkeletonPoseUpdatedHandler()
        {
            if (handSkeletonPoseUpdatedHandler == null)
            {
                handSkeletonPoseUpdatedHandler = OnHandSkeletonPoseUpdated;
            }

            handSkeleton.OnPoseUpdated.RemoveListener(handSkeletonPoseUpdatedHandler);
        }

        private void OnHandSkeletonPoseUpdated() => doNotUpdatePoseProviderState = false;

        protected override void OnGetBonePoses()
        {
            if (doNotUpdatePoseProviderState)
            {
                return;
            }

            UpdatePoseProviderState();

            doNotUpdatePoseProviderState = true;
        }

        private void UpdatePoseProviderState()
        {
            var sourcePoseProvider = handSkeleton.PoseProvider;

            if (!handSkeleton.IsComplete && !sourcePoseProvider)
            {
#if UNITY_EDITOR
                if (Application.IsPlaying(this))
                {
                    if (logSourceHandSkeletonNotCompleteAndWithoutPoseProvider)
                    {
                        LogSourceHandSkeletonNotCompleteAndWithoutPoseProvider();

                        logSourceHandSkeletonNotCompleteAndWithoutPoseProvider = false;
                    }
                }
#endif
                return;
            }
#if UNITY_EDITOR
            if (Application.IsPlaying(this))
            {
                logSourceHandSkeletonNotCompleteAndWithoutPoseProvider = true;
            }
#endif
            var root = handSkeleton.Transform;
            var rootPose = new Pose(root.position, root.rotation);

            var rootScale = root.lossyScale;
            var scale = Mathf.Sqrt(Mathf.Sqrt(rootScale.x * rootScale.y) * rootScale.z);
            var scaleInv = 1f / scale;

            if (float.IsNaN(scaleInv) || float.IsInfinity(scaleInv))
            {
                scaleInv = 1f;
            }

            poses[0] = rootPose;

            var handSkeletonBones = handSkeleton.Bones;

            if (handSkeleton.IsComplete)
            {
                SetFingerBonePosesWhenHandSkeletonComplete(rootPose.position, scaleInv);
            }
            else if (handSkeletonBones.Count == 1)
            {
                SetBonePosesWhenHandSkeletonHasRootBoneOnly(in rootPose);
            }
            else
            {
                var sourceProviderBoneAbsolutePoses = sourcePoseProvider.GetBoneAbsolutePoses();
                var sourceProviderRootPose = sourceProviderBoneAbsolutePoses[0];

                SetFingerAbsolutePosesFromHandSkeletonBonesAndInitializeFingers(
                    handSkeletonBones,
                    ref rootPose,
                    scaleInv);

                SetFingerBonePoses(sourceProviderBoneAbsolutePoses, rootPose);
            }

            Scale = scale;

            SetBoneAbsolutePoses(poses);
        }

        private void SetFingerBonePoses(HandBoneKeyedReadOnlyCollection<Pose> sourceProviderBoneAbsolutePoses, in Pose rootPose)
        {
            var providerRootPose = sourceProviderBoneAbsolutePoses[0];
            Pose targetBonePose;
            Pose sourceProviderTargetBonePose;

            for (var i = 0; i < HandSkeletonConfiguration.FingerCount; i++)
            {
                var finger = handFingerBones[i];
                var fingerBoneIds = HandSkeletonConfiguration.FingerBones[i];

                var targetBoneId = (int)HandBoneId.WristRoot;

                if (finger.Count != 0)
                {
                    targetBoneId = (int)finger[0].BoneId;
                    targetBonePose = poses[targetBoneId];
                    sourceProviderTargetBonePose = sourceProviderBoneAbsolutePoses[targetBoneId];
                }
                else
                {
                    targetBonePose = rootPose;
                    sourceProviderTargetBonePose = providerRootPose;
                }

                var fingerBoneIndex = 0;
                var lastFingerBoneIndex = finger.Count - 1;

                for (var j = 0; j < HandSkeletonConfiguration.BonesPerFinger; j++)
                {
                    var boneId = (int)fingerBoneIds[j];

                    if (boneId == targetBoneId)
                    {
                        if (fingerBoneIndex < lastFingerBoneIndex)
                        {
                            targetBoneId = (int)finger[++fingerBoneIndex].BoneId;
                            targetBonePose = poses[targetBoneId];
                            sourceProviderTargetBonePose = sourceProviderBoneAbsolutePoses[targetBoneId];
                        }

                        continue;
                    }

                    var providerBonePose = sourceProviderBoneAbsolutePoses[boneId];
                    var bonePoseRelativeToTargetBone = providerBonePose.RelativeTo(sourceProviderTargetBonePose);

                    ref var boneAbsolutePose = ref poses[boneId];

                    boneAbsolutePose = bonePoseRelativeToTargetBone.TransformedBy(in targetBonePose);
                }

                finger.Clear();
            }
        }

        private void SetFingerAbsolutePosesFromHandSkeletonBonesAndInitializeFingers(
            HandBoneKeyedSparseReadOnlyCollection<HandBone> handSkeletonBones,
            ref Pose rootPose,
            float scaleInv)
        {
            for (var i = 1; i < handSkeletonBones.Count; i++)
            {
                var bone = handSkeletonBones[i];
                ref var boneAbsolutePose = ref poses[(int)bone.BoneId];

                GetBoneAbsolutePosesFromHandBone(bone, rootPose.position, scaleInv, out boneAbsolutePose);

                var fingerIndex = (int)HandSkeletonConfiguration.GetBoneFinger(bone.BoneId);

                handFingerBones[fingerIndex].Add(bone);
            }
        }

        private void SetFingerBonePosesWhenHandSkeletonComplete(Vector3 rootPosition, float scaleInv)
        {
            var handSkeletonBones = handSkeleton.Bones;

            for (var i = 1; i < handSkeletonBones.Count; i++)
            {
                var bone = handSkeletonBones[i];
                ref var boneAbsolutePose = ref poses[(int)bone.BoneId];

                GetBoneAbsolutePosesFromHandBone(bone, rootPosition, scaleInv, out boneAbsolutePose);
            }
        }

        private void SetBonePosesWhenHandSkeletonHasRootBoneOnly(in Pose rootPose)
        {
            var sourcePoseProvider = handSkeleton.PoseProvider;
            var sourceProviderAbsolutePoses = sourcePoseProvider.GetBoneAbsolutePoses();
            var sourceProviderRootPose = sourceProviderAbsolutePoses[0];

            Pose bonePoseRelativeToRoot;

            for (var boneId = (int)HandSkeletonConfiguration.FirstFingerBoneId; boneId < HandSkeletonConfiguration.BoneCount; boneId++)
            {
                var sourceProviderBoneAbsolutePose = sourceProviderAbsolutePoses[boneId];

                bonePoseRelativeToRoot = sourceProviderBoneAbsolutePose.RelativeTo(sourceProviderRootPose);

                ref var boneAbsolutePose = ref poses[boneId];

                boneAbsolutePose = bonePoseRelativeToRoot.TransformedBy(in rootPose);
            }
        }

        private void GetBoneAbsolutePosesFromHandBone(
            HandBone bone,
            Vector3 rootPosition,
            float scaleInv,
            out Pose boneAbsolutePose)
        {
            var boneTransform = bone.Transform;

            boneAbsolutePose = new Pose(rootPosition + scaleInv * (boneTransform.position - rootPosition), boneTransform.rotation);
        }
    }
}
