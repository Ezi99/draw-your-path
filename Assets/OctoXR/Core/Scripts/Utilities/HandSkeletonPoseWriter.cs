using OctoXR.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.Utilities
{
    [Serializable]
    public class HandSkeletonPoseWriteTarget
    {
        [SerializeField]
        [Tooltip("Should the pose be captured from a pose source and be written to the target pose asset")]
        private bool captureSourcePose;
        /// <summary>
        /// Should the pose be captured from a pose source and be written to the target pose asset
        /// </summary>
        public bool CaptureSourcePose
        {
            get => captureSourcePose;
            set => captureSourcePose = value;
        }

        [SerializeField]
        [Tooltip("Specifies for which hand bones will the poses be written to the target")]
        private HandBones handBones;
        /// <summary>
        /// Specifies for which hand bones will the poses be written to the target
        /// </summary>
        public HandBones HandBones
        {
            get => handBones;
            set => handBones = value;
        }

        [SerializeField]
        [Tooltip("The exact components of the source poses that should be written")]
        private PoseComponents poseComponents;
        /// <summary>
        /// The exact components of the source poses that should be written
        /// </summary>
        public PoseComponents PoseComponents
        {
            get => poseComponents;
            set => poseComponents = value;
        }

        [SerializeField]
        [Tooltip("Target pose asset to write the captured poses to")]
        private HandSkeletonPose writeTo;
        /// <summary>
        /// Target pose asset to write the captured poses to
        /// </summary>
        public HandSkeletonPose WriteTo
        {
            get => writeTo;
            set => writeTo = value;
        }

        [SerializeField]
        [Tooltip("Preview in Unity editor. This is editor-only option")]
        private bool previewInEditor;
        /// <summary>
        /// Preview in Unity editor. This is editor-only option
        /// </summary>
        public bool PreviewInEditor => previewInEditor;

        public HandSkeletonPoseWriteTarget() : this(null) { }

        public HandSkeletonPoseWriteTarget(
            HandSkeletonPose writeTo,
            HandBones handBones = HandBones.FingerBones,
            PoseComponents poseComponents = PoseComponents.PositionAndRotation,
            bool captureSourcePose = false)
        {
            this.captureSourcePose = captureSourcePose;
            this.handBones = handBones;
            this.poseComponents = poseComponents;
            this.writeTo = writeTo;
        }

        /// <summary>
        /// Writes the current pose of the specified hand skeleton to the target pose asset
        /// </summary>
        /// <param name="sourceHandSkeleton"></param>
        /// <returns></returns>
        public bool WritePose(HandSkeleton sourceHandSkeleton)
        {
            if (!sourceHandSkeleton)
            {
                throw new ArgumentNullException(nameof(sourceHandSkeleton));
            }

            if (!writeTo)
            {
                return false;
            }

            var bones = sourceHandSkeleton.Bones;
#if UNITY_EDITOR
            var setDirty = false;
#endif
            for (var i = 0; i < bones.Count; ++i)
            {
                var sourceBone = bones[i];
                var boneId = (int)sourceBone.BoneId;
                var writeBone = (HandBones)(1 << boneId);

                if ((handBones & writeBone) != HandBones.None)
                {
                    var parentBone = sourceBone.ParentBone;
                    Pose boneRelativePose;

                    if (parentBone)
                    {
                        boneRelativePose = new Pose(sourceBone.Transform.position, sourceBone.Transform.rotation);
                        boneRelativePose = boneRelativePose.RelativeTo(parentBone.Transform);
                    }
                    else
                    {
                        boneRelativePose = Pose.identity;
                    }
#if UNITY_EDITOR
                    var previousBonePose = writeTo[boneId];
#endif
                    if ((poseComponents & PoseComponents.Position) == PoseComponents.Position)
                    {
                        writeTo[boneId].position = boneRelativePose.position;
                    }

                    if ((poseComponents & PoseComponents.Rotation) == PoseComponents.Rotation)
                    {
                        writeTo[boneId].rotation = boneRelativePose.rotation;
                    }
#if UNITY_EDITOR
                    setDirty |= writeTo[boneId] != previousBonePose;
#endif
                }
            }
#if UNITY_EDITOR
            if (setDirty)
            {
                if (Application.IsPlaying(writeTo))
                {
                    if (UnityEditor.EditorUtility.IsPersistent(writeTo))
                    {
                        UnityEditor.EditorUtility.SetDirty(writeTo);
                    }
                }
                else
                {
                    UnityEditor.EditorUtility.SetDirty(writeTo);
                }
            }
#endif
            return true;
        }
    }

    [ExecuteAlways]
    [RequireComponent(typeof(HandSkeleton))]
    public class HandSkeletonPoseWriter : HandSkeletonPoseProvider
    {
        [Serializable]
        private class PosePreviewConfiguration
        {
            [PropertyDrawOptions(ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways)]
            public string PreviewingPose;
            public Transform RootPose;
            public HandSkeletonPose StartPose;
            [Range(0, 1)]
            public float Interpolate = 1f;
        }

        [SerializeField]
        [HideInInspector]
        private HandSkeleton handSkeleton;
        /// <summary>
        /// Hand skeleton the pose writer reads source poses from 
        /// </summary>
        public HandSkeleton HandSkeleton => handSkeleton;

        [SerializeField]
        [Tooltip("Pose write targets the source poses can be written to")]
        [PropertyDrawOptions(LabelText = "Target", ArrayItemLabelIndexFormat = " #{0}", ArrayItemLabelBaseIndex = 1)]
        private List<HandSkeletonPoseWriteTarget> targets = new List<HandSkeletonPoseWriteTarget>();
        /// <summary>
        /// Pose write targets the source poses can be written to
        /// </summary>
        public List<HandSkeletonPoseWriteTarget> Targets => targets;

        [SerializeField]
        private PosePreviewConfiguration previewPose;

        private UnityAction handSkeletonPoseUpdatedHandler;
        private readonly HandBoneKeyedCollection<Pose> bonePoses =
#if UNITY_EDITOR
            new HandBoneKeyedCollection<Pose>();
#else
            null;
#endif
        public override HandType GetHandType() => handSkeleton.HandType;

        private void Reset()
        {
            ResetHandSkeleton();
        }

        private void OnValidate()
        {
            Initialize();

            for (var i = 0; i < targets.Count; ++i)
            {
                var target = targets[i];

                if (target == null)
                {
                    targets.RemoveAt(i--);

                    continue;
                }

                if (target.PoseComponents == 0)
                {
                    targets[i] = new HandSkeletonPoseWriteTarget();
                }
                else if (target.CaptureSourcePose)
                {
                    target.WritePose(handSkeleton);
                }
            }
#if UNITY_EDITOR
            if (previewPose != null)
            {
                UpdateTargetPoseFromPosePreview();
            }
#endif
        }

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            SubscribeHandSkeletonPoseUpdatedHandler();
        }

        private void OnDisable()
        {
            UnsubscribeHandSkeletonPoseUpdatedHandler();
        }

        private void Initialize()
        {
            if (!handSkeleton || handSkeleton.gameObject != gameObject)
            {
                ResetHandSkeleton();
            }
        }

        private void ResetHandSkeleton()
        {
            handSkeleton = GetComponent<HandSkeleton>();
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
                handSkeletonPoseUpdatedHandler = WriteSourcePoseToTargets;
            }

            handSkeleton.OnPoseUpdated.RemoveListener(handSkeletonPoseUpdatedHandler);
        }

        private void WriteSourcePoseToTargets()
        {
            for (var i = 0; i < targets.Count; ++i)
            {
                var target = targets[i];

                if (target == null)
                {
                    targets.RemoveAt(i--);
                    ObjectUtility.SetObjectDirty(this);

                    continue;
                }

                if (target.CaptureSourcePose)
                {
                    target.WritePose(handSkeleton);
                }
            }
        }

        private void UpdateTargetPoseFromPosePreview()
        {
            HandSkeletonPose targetPose = null;

            for (var i = 0; i < targets.Count; ++i)
            {
                var target = targets[i];

                if (target.PreviewInEditor)
                {
                    targetPose = target.WriteTo;

                    break;
                }
            }

            var startPose = previewPose.StartPose;

            if (targetPose)
            {
                previewPose.PreviewingPose = targetPose.name;

                if (!startPose)
                {
                    previewPose.Interpolate = 1;
                    SetBonePosesFromTargetPose(targetPose);
                }
                else
                {
                    var lerpedRootPose = new Pose(
                        position: Vector3.LerpUnclamped(startPose[0].position, targetPose[0].position, previewPose.Interpolate),
                        rotation: Quaternion.SlerpUnclamped(startPose[0].rotation, targetPose[0].rotation, previewPose.Interpolate));

                    bonePoses[0] = lerpedRootPose.TransformedBy(GetRootPose());

                    for (var i = 1; i < bonePoses.Count; i++)
                    {
                        ref var bonePose = ref bonePoses[i];
                        var start = startPose[i];
                        var target = targetPose[i];

                        bonePose.position = Vector3.LerpUnclamped(start.position, target.position, previewPose.Interpolate);
                        bonePose.rotation = Quaternion.SlerpUnclamped(start.rotation, target.rotation, previewPose.Interpolate);
                    }

                    SetBoneRelativePoses(bonePoses);
                }
            }
            else
            {
                previewPose.PreviewingPose = null;

                if (startPose)
                {
                    previewPose.Interpolate = 0;
                    SetBonePosesFromTargetPose(startPose);
                }
            }
        }

        private void SetBonePosesFromTargetPose(HandSkeletonPose targetPose)
        {
            bonePoses[0] = targetPose[0].TransformedBy(GetRootPose());

            for (var i = 1; i < bonePoses.Count; i++)
            {
                bonePoses[i] = targetPose[i];
            }

            SetBoneRelativePoses(bonePoses);
        }

        private Pose GetRootPose()
        {
            if (previewPose.RootPose)
            {
                return new Pose(previewPose.RootPose.position, previewPose.RootPose.rotation);
            }

            return Pose.identity;
        }
    }
}