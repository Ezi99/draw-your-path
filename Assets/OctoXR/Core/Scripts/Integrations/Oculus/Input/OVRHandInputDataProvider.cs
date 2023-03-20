using OctoXR.Collections;
using OctoXR.Input;
using System;
using UnityEngine;

namespace OctoXR.Integrations.Oculus.Input
{
    [DefaultExecutionOrder(-90)]
    public class OVRHandInputDataProvider : HandInputDataProvider
    {
        private const int idOvrThumbTrapezium = (int)OVRPlugin.BoneId.Hand_Thumb0;
        private const int idOvrThumbMetacarpal = (int)OVRPlugin.BoneId.Hand_Thumb1;
        private const int idOvrThumbProximal = (int)OVRPlugin.BoneId.Hand_Thumb2;
        private const int idOvrThumbDistal = (int)OVRPlugin.BoneId.Hand_Thumb3;
        private const int idOvrPinkyMetacarpal = (int)OVRPlugin.BoneId.Hand_Pinky0;
        private const int idOvrPinkyProximal = (int)OVRPlugin.BoneId.Hand_Pinky1;
        private const int idOvrPinkyMiddle = (int)OVRPlugin.BoneId.Hand_Pinky2;

        private const int idOffsetToOvrForBonesFromThumbProximalToPinkyStart = (int)HandBoneId.ThumbFingerProximalPhalanx - (int)OVRPlugin.BoneId.Hand_Thumb2;
        private const int idOffsetToOvrForBonesFromPinkyMiddleToEnd = (int)HandBoneId.PinkyFingerMiddlePhalanx - (int)OVRPlugin.BoneId.Hand_Pinky2;

        private const int wristRootId = (int)HandBoneId.WristRoot;
        private const int thumbMetacarpalId = (int)HandBoneId.ThumbFingerMetacarpal;
        private const int thumbProximalId = (int)HandBoneId.ThumbFingerProximalPhalanx;
        private const int pinkyProximalId = (int)HandBoneId.PinkyFingerProximalPhalanx;
        private const int firstFingerBoneId = (int)HandBoneId.ThumbFingerMetacarpal;

        private const int boneCount = HandSkeletonConfiguration.BoneCount;
        private const int fingerCount = HandSkeletonConfiguration.FingerCount;

        private class HandSkeleton
        {
            private bool isInitialized;
            public OVRPlugin.Skeleton2 OvrSkeleton;
            public readonly Pose[] Bones = new Pose[boneCount];

            public bool IsInitialized => isInitialized;

            public bool EnsureInitialized(HandType handType)
            {
                if (!isInitialized && OVRPlugin.GetSkeleton2(OVRUtility.HandTypeToOvrSkeletonType(handType), ref OvrSkeleton))
                {
                    ref var wristRoot = ref Bones[0];

                    wristRoot = Pose.identity;

                    ref var thumbMetacarpal = ref Bones[thumbMetacarpalId];

                    var ovrThumbTrapezium = OVRUtility.GetBonePoseFromOvrBonePose(OvrSkeleton.Bones[idOvrThumbTrapezium].Pose, handType);
                    var ovrThumbMetacarpal = OVRUtility.GetBonePoseFromOvrBonePose(OvrSkeleton.Bones[idOvrThumbMetacarpal].Pose, handType);
                    var ovrThumbProximal = OVRUtility.GetBonePoseFromOvrBonePose(OvrSkeleton.Bones[idOvrThumbProximal].Pose, handType);

                    thumbMetacarpal.position = ovrThumbTrapezium.position;
                    thumbMetacarpal.rotation = ovrThumbTrapezium.rotation * ovrThumbMetacarpal.rotation;

                    var ovrTrapeziumMetacarpalComposed = ovrThumbMetacarpal.TransformedBy(ovrThumbTrapezium);
                    var thumbProximalAbsolutePosition = 
                        ovrTrapeziumMetacarpalComposed.rotation * ovrThumbProximal.position + ovrTrapeziumMetacarpalComposed.position;

                    ref var thumbProximal = ref Bones[thumbProximalId];

                    thumbProximal.position = 
                        Quaternion.Inverse(thumbMetacarpal.rotation) * (thumbProximalAbsolutePosition - ovrThumbTrapezium.position);
                    thumbProximal.rotation = ovrThumbProximal.rotation;

                    SetBonePosesFromOvrBones(
                       handType,
                       OvrSkeleton.Bones,
                       idOvrThumbDistal,
                       idOvrPinkyMetacarpal,
                       idOffsetToOvrForBonesFromThumbProximalToPinkyStart,
                       Bones);

                    ref var pinkyProximal = ref Bones[pinkyProximalId];

                    var ovrPinkyMetacarpal = OVRUtility.GetBonePoseFromOvrBonePose(OvrSkeleton.Bones[idOvrPinkyMetacarpal].Pose, handType);
                    var ovrPinkyProximal = OVRUtility.GetBonePoseFromOvrBonePose(OvrSkeleton.Bones[idOvrPinkyProximal].Pose, handType);

                    pinkyProximal.position = ovrPinkyMetacarpal.position + ovrPinkyMetacarpal.rotation * ovrPinkyProximal.position;
                    pinkyProximal.rotation = ovrPinkyMetacarpal.rotation * ovrPinkyProximal.rotation;

                    SetBonePosesFromOvrBones(
                        handType,
                        OvrSkeleton.Bones,
                        idOvrPinkyMiddle,
                        (int)OVRPlugin.BoneId.Hand_End,
                        idOffsetToOvrForBonesFromPinkyMiddleToEnd,
                        Bones);

                    isInitialized = true;
                }

                return isInitialized;
            }

            private static void SetBonePosesFromOvrBones(
                HandType handType,
                OVRPlugin.Bone[] ovrBones,
                int idOvrBoneStart,
                int idOvrBoneEnd,
                int idOffsetToOvrBones,
                Pose[] bonePoses)
            {
                for (var i = idOvrBoneStart; i < idOvrBoneEnd; ++i)
                {
                    var ovrBone = ovrBones[i];
                    var boneId = i + idOffsetToOvrBones;
                    ref var bone = ref bonePoses[boneId];

                    if (handType == HandType.Left)
                    {
                        OVRUtility.SetBonePositionFromOvrBonePositionLeft(in ovrBone.Pose.Position, ref bone.position);
                        OVRUtility.SetBoneRotationFromOvrBoneRotationLeft(in ovrBone.Pose.Orientation, ref bone.rotation);
                    }
                    else
                    {
                        OVRUtility.SetBonePositionFromOvrBonePositionRight(in ovrBone.Pose.Position, ref bone.position);
                        OVRUtility.SetBoneRotationFromOvrBoneRotationRight(in ovrBone.Pose.Orientation, ref bone.rotation);
                    }
                }
            }
        }

        [SerializeField]
        [Tooltip("Specifies for which hand is input provided by the input data provider")]
        private HandType handType;
        /// <summary>
        /// Specifies for which hand is input provided by the input data provider
        /// </summary>
        public HandType HandType 
        { 
            get => handType;
            set
            {
#if UNITY_EDITOR
                if (handType == value)
                {
                    return;
                }
#endif
                handType = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        /// <summary>
        /// Specifies for which hand is input provided by the input data provider
        /// </summary>
        /// <returns></returns>
        public sealed override HandType GetHandType() => handType;

        [SerializeField]
        [Tooltip("Bind poses of the hand bones. These are initial poses of the hand bones, relative to parent bones. " +
            "This value is optional, if nothing is assigned then pose of the default OVR skeleton is used")]
        private HandSkeletonPose bindPose;
        /// <summary>
        /// Bind poses of the hand bones. These are initial poses of the hand bones, relative to parent bones.
        /// This value is optional, if nothing is assigned then pose of the default OVR skeleton is used
        /// </summary>
        public HandSkeletonPose BindPose 
        { 
            get => bindPose;
            set
            {
#if UNITY_EDITOR
                if (bindPose == value)
                {
                    return;
                }
#endif
                bindPose = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        private static readonly HandSkeleton[] ovrSkeletonBindPoses = new[]
        {
            new HandSkeleton(),
            new HandSkeleton()
        };

        private static readonly Quaternion rootRotationFixupLeft = new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f);
        private static readonly Quaternion rootRotationFixupRight = new Quaternion(0.5f, -0.5f, 0.5f, 0.5f);

        private OVRPlugin.HandState handState;

        private readonly Pose[] bonePoses = new Pose[HandSkeletonConfiguration.BoneCount];

        private bool bonePosesInitialized;

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
#if UNITY_EDITOR
            if (Application.IsPlaying(this))
            {
#endif
                EnsureOvrSkeletonBindPoseInitialized();
                SetIsTracking(false);
#if UNITY_EDITOR
            }
#endif
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (Application.IsPlaying(this))
            {
#endif
                UpdateInputAndPoseState(OVRPlugin.Step.Render);
#if UNITY_EDITOR
            }
            else if (bindPose && IsTracking)
            {
                var rootPose = GetRootPose(Pose.identity);

                bonePoses[wristRootId] = rootPose;

                for (var i = firstFingerBoneId; i < boneCount; i++)
                {
                    bonePoses[i] = bindPose[i];
                }

                SetBoneRelativePoses(bonePoses);
            }
#endif
        }

        protected virtual void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.IsPlaying(this))
            {
                return;
            }
#endif
            if (OVRPlugin.nativeXrApi != OVRPlugin.XrApi.OpenXR)
            {
                UpdateInputAndPoseState(OVRPlugin.Step.Physics);
            }
        }

        private void UpdateInputAndPoseState(OVRPlugin.Step step)
        {
            if (!bindPose && !EnsureOvrSkeletonBindPoseInitialized())
            {
                SetIsTracking(false);

                return;
            }

            if (!bonePosesInitialized)
            {
                InitializeBonePoses();
            }

            if (OVRPlugin.GetHandState(step, OVRUtility.HandTypeToOvrHand(handType), ref handState))
            {
                var isTracking = (handState.Status & OVRPlugin.HandStatus.HandTracked) != 0;

                if (!isTracking)
                {
                    SetIsTracking(false);

                    return;
                }

                try
                {
                    SetIsTracking(true);
                }
                finally
                {
                    if (IsTracking)
                    {
                        SetInputAndPoseState();
                    }
                }
            }
            else
            {
                SetIsTracking(false);
            }
        }

        private bool EnsureOvrSkeletonBindPoseInitialized()
        {
            return ovrSkeletonBindPoses[(int)handType].EnsureInitialized(handType);
        }

        private void SetInputAndPoseState()
        {
            var confidence =
                (GetConfidenceValueFromOvrTrackingConfidence(handState.HandConfidence) +
                SetFingerStatesAndGetFingerConfidenceAverage())
                / 2;

            Confidence = confidence;

            SetPoseState();

            var isSystemGestureInProgress = (handState.Status & OVRPlugin.HandStatus.SystemGestureInProgress) != 0;
            
            SetIsSystemGestureInProgress(isSystemGestureInProgress);
        }

        private void SetPoseState()
        {
            var scale = handState.HandScale;

            var ovrRootPose = handState.RootPose;

            var rootPose = new Pose(
                new Vector3(ovrRootPose.Position.x, ovrRootPose.Position.y, -ovrRootPose.Position.z),
                new Quaternion(
                    -ovrRootPose.Orientation.x,
                    -ovrRootPose.Orientation.y,
                    ovrRootPose.Orientation.z,
                    ovrRootPose.Orientation.w));

            if (handType == HandType.Left)
            {
                rootPose.rotation *= rootRotationFixupLeft;

                rootPose = GetRootPose(rootPose);

                bonePoses[wristRootId] = rootPose;

                if (bindPose)
                {
                    SetFingerPosesForLeftHandUsingCustomBindPose();
                }
                else
                {
                    SetFingerPosesForLeftHandUsingOvrSkeleton();
                }
            }
            else
            {
                rootPose.rotation *= rootRotationFixupRight;

                rootPose = GetRootPose(rootPose);

                bonePoses[wristRootId] = rootPose;

                if (bindPose)
                {
                    SetFingerPosesForRightHandUsingCustomBindPose();
                }
                else
                {
                    SetFingerPosesForRightHandUsingOvrSkeleton();
                }
            }

            Scale = scale;
            SetBoneRelativePoses(bonePoses);
        }

        private void SetFingerPosesForLeftHandUsingCustomBindPose()
        {
            SetFingerBonePoseComposedFromOvrConnectedBonesForLeftHand(
                thumbMetacarpalId,
                idOvrThumbTrapezium,
                idOvrThumbMetacarpal,
                in bindPose[thumbMetacarpalId].position);
            SetFingerBonePosesFromOvrBonesForLeftHandUsingCustomBindPose(
                idOvrThumbProximal,
                idOvrPinkyMetacarpal,
                idOffsetToOvrForBonesFromThumbProximalToPinkyStart);
            SetFingerBonePoseComposedFromOvrConnectedBonesForLeftHand(
                pinkyProximalId,
                idOvrPinkyMetacarpal,
                idOvrPinkyProximal,
                in bindPose[pinkyProximalId].position);
            SetFingerBonePosesFromOvrBonesForLeftHandUsingCustomBindPose(
                idOvrPinkyMiddle,
                (int)OVRPlugin.BoneId.Hand_End,
                idOffsetToOvrForBonesFromPinkyMiddleToEnd);
        }

        private void SetFingerPosesForLeftHandUsingOvrSkeleton()
        {
            var skeleton = ovrSkeletonBindPoses[(int)HandType.Left];

            SetFingerBonePoseComposedFromOvrConnectedBonesForLeftHand(
                thumbMetacarpalId,
                idOvrThumbTrapezium,
                idOvrThumbMetacarpal,
                in skeleton.Bones[thumbMetacarpalId].position);
            SetFingerBonePosesFromOvrBonesForLeftHandUsingOvrSkeleton(
                idOvrThumbProximal,
                idOvrPinkyMetacarpal,
                idOffsetToOvrForBonesFromThumbProximalToPinkyStart,
                skeleton);
            SetFingerBonePoseComposedFromOvrConnectedBonesForLeftHand(
                pinkyProximalId,
                idOvrPinkyMetacarpal,
                idOvrPinkyProximal,
                in skeleton.Bones[pinkyProximalId].position);
            SetFingerBonePosesFromOvrBonesForLeftHandUsingOvrSkeleton(
                idOvrPinkyMiddle,
                (int)OVRPlugin.BoneId.Hand_End,
                idOffsetToOvrForBonesFromPinkyMiddleToEnd,
                skeleton);
        }

        private void SetFingerPosesForRightHandUsingCustomBindPose()
        {
            SetFingerBonePoseComposedFromOvrConnectedBonesForRightHand(
                thumbMetacarpalId,
                idOvrThumbTrapezium,
                idOvrThumbMetacarpal,
                in bindPose[thumbMetacarpalId].position);
            SetFingerBonePosesFromOvrBonesForRightHandUsingCustomBindPose(
                idOvrThumbProximal,
                idOvrPinkyMetacarpal,
                idOffsetToOvrForBonesFromThumbProximalToPinkyStart);
            SetFingerBonePoseComposedFromOvrConnectedBonesForRightHand(
                pinkyProximalId,
                idOvrPinkyMetacarpal,
                idOvrPinkyProximal,
                in bindPose[pinkyProximalId].position);
            SetFingerBonePosesFromOvrBonesForRightHandUsingCustomBindPose(
                idOvrPinkyMiddle,
                (int)OVRPlugin.BoneId.Hand_End,
                idOffsetToOvrForBonesFromPinkyMiddleToEnd);
        }

        private void SetFingerPosesForRightHandUsingOvrSkeleton()
        {
            var skeleton = ovrSkeletonBindPoses[(int)HandType.Right];

            SetFingerBonePoseComposedFromOvrConnectedBonesForRightHand(
                thumbMetacarpalId,
                idOvrThumbTrapezium,
                idOvrThumbMetacarpal,
                in skeleton.Bones[thumbMetacarpalId].position);
            SetFingerBonePosesFromOvrBonesForRightHandUsingOvrSkeleton(
                idOvrThumbProximal,
                idOvrPinkyMetacarpal,
                idOffsetToOvrForBonesFromThumbProximalToPinkyStart,
                skeleton);
            SetFingerBonePoseComposedFromOvrConnectedBonesForRightHand(
                pinkyProximalId,
                idOvrPinkyMetacarpal,
                idOvrPinkyProximal,
                in skeleton.Bones[pinkyProximalId].position);
            SetFingerBonePosesFromOvrBonesForRightHandUsingOvrSkeleton(
                idOvrPinkyMiddle,
                (int)OVRPlugin.BoneId.Hand_End,
                idOffsetToOvrForBonesFromPinkyMiddleToEnd,
                skeleton);
        }

        private void SetFingerBonePoseComposedFromOvrConnectedBonesForLeftHand(
            int boneId,
            int ovrParentBoneId,
            int ovrChildBoneId,
            in Vector3 boneBindPosePosition)
        {
            var boneRelativePose = new Pose
            {
                position = boneBindPosePosition
            };

            OVRUtility.SetBoneRotationComposedFromOvrConnectedBonesLeft(
                in handState.BoneRotations[ovrParentBoneId],
                in handState.BoneRotations[ovrChildBoneId],
                ref boneRelativePose.rotation);

            SetFingerBonePose(boneId, in boneRelativePose);
        }

        private void SetFingerBonePosesFromOvrBonesForLeftHandUsingCustomBindPose(
            int ovrBoneStartId,
            int ovrBoneEndId,
            int idOffsetToOvrBones)
        {
            var boneRelativePose = new Pose();

            for (var i = ovrBoneStartId; i < ovrBoneEndId; i++)
            {
                var boneId = i + idOffsetToOvrBones;
                var ovrBoneRotation = handState.BoneRotations[i];

                boneRelativePose.position = bindPose[boneId].position;
                OVRUtility.SetBoneRotationFromOvrBoneRotationLeft(in ovrBoneRotation, ref boneRelativePose.rotation);

                SetFingerBonePose(boneId, in boneRelativePose);
            }
        }

        private void SetFingerBonePosesFromOvrBonesForLeftHandUsingOvrSkeleton(
            int ovrBoneStartId,
            int ovrBoneEndId,
            int idOffsetToOvrBones,
            HandSkeleton skeleton)
        {
            var boneRelativePose = new Pose();

            for (var i = ovrBoneStartId; i < ovrBoneEndId; i++)
            {
                var boneId = i + idOffsetToOvrBones;
                var ovrBoneRotation = handState.BoneRotations[i];

                boneRelativePose.position = skeleton.Bones[boneId].position;
                OVRUtility.SetBoneRotationFromOvrBoneRotationLeft(in ovrBoneRotation, ref boneRelativePose.rotation);

                SetFingerBonePose(boneId, in boneRelativePose);
            }
        }

        private void SetFingerBonePoseComposedFromOvrConnectedBonesForRightHand(
            int boneId,
            int ovrParentBoneId,
            int ovrChildBoneId,
            in Vector3 boneBindPosePosition)
        {
            var boneRelativePose = new Pose();

            boneRelativePose.position = boneBindPosePosition;

            OVRUtility.SetBoneRotationComposedFromOvrConnectedBonesRight(
                in handState.BoneRotations[ovrParentBoneId],
                in handState.BoneRotations[ovrChildBoneId],
                ref boneRelativePose.rotation);

            SetFingerBonePose(boneId, in boneRelativePose);
        }

        private void SetFingerBonePosesFromOvrBonesForRightHandUsingCustomBindPose(
            int ovrBoneStartId,
            int ovrBoneEndId,
            int idOffsetToOvrBones)
        {
            var boneRelativePose = new Pose();

            for (var i = ovrBoneStartId; i < ovrBoneEndId; i++)
            {
                var boneId = i + idOffsetToOvrBones;
                var ovrBoneRotation = handState.BoneRotations[i];

                boneRelativePose.position = bindPose[boneId].position;
                OVRUtility.SetBoneRotationFromOvrBoneRotationRight(in ovrBoneRotation, ref boneRelativePose.rotation);

                SetFingerBonePose(boneId, in boneRelativePose);
            }
        }

        private void SetFingerBonePosesFromOvrBonesForRightHandUsingOvrSkeleton(
            int ovrBoneStartId,
            int ovrBoneEndId,
            int idOffsetToOvrBones,
            HandSkeleton skeleton)
        {
            var boneRelativePose = new Pose();

            for (var i = ovrBoneStartId; i < ovrBoneEndId; i++)
            {
                var boneId = i + idOffsetToOvrBones;
                var ovrBoneRotation = handState.BoneRotations[i];

                boneRelativePose.position = skeleton.Bones[boneId].position;
                OVRUtility.SetBoneRotationFromOvrBoneRotationRight(in ovrBoneRotation, ref boneRelativePose.rotation);

                SetFingerBonePose(boneId, in boneRelativePose);
            }
        }

        private void SetFingerBonePose(int boneId, in Pose boneRelativePose)
        {
            ref var bonePose = ref bonePoses[boneId];

            bonePose.position = boneRelativePose.position;

            var boneRelativeRotation = Quaternion.Normalize(boneRelativePose.rotation);

            if (!float.IsNaN(boneRelativeRotation.x))
            {
                bonePose.rotation = boneRelativeRotation;
            }
        }

        private static float GetConfidenceValueFromOvrTrackingConfidence(OVRPlugin.TrackingConfidence trackingConfidence)
            => trackingConfidence == OVRPlugin.TrackingConfidence.High ? 1 : 0;

        private float SetFingerStatesAndGetFingerConfidenceAverage()
        {
            var fingerConfidenceAverage = 0f;

            if (handState.PinchStrength == null || handState.PinchStrength.Length != (int)OVRPlugin.HandFinger.Max ||
                handState.FingerConfidences == null || handState.FingerConfidences.Length != (int)OVRPlugin.HandFinger.Max)
            {
                for (var finger = 0; finger < fingerCount; finger++)
                {
                    SetFingerState(finger, default);
                }
            }
            else
            {
                var fingerState = new HandFingerState();

                for (var finger = 0; finger < fingerCount; finger++)
                {
                    fingerState.IsPinching = ((int)handState.Pinches & (1 << finger)) != 0;
                    fingerState.PinchStrength = handState.PinchStrength[finger];
                    fingerState.Confidence = GetConfidenceValueFromOvrTrackingConfidence(handState.FingerConfidences[finger]);

                    fingerConfidenceAverage += fingerState.Confidence;

                    SetFingerState(finger, fingerState);
                }

                fingerConfidenceAverage /= fingerCount;
            }

            return fingerConfidenceAverage;
        }

        private void InitializeBonePoses()
        {
            if (bindPose)
            {
                for (var i = 0; i < bonePoses.Length; i++)
                {
                    bonePoses[i] = bindPose[i];
                }
            }
            else
            {
                var skeleton = ovrSkeletonBindPoses[(int)handType];

                if (!skeleton.IsInitialized)
                {
                    return;
                }

                var bindPoses = skeleton.Bones;

                for (var i = 0; i < bonePoses.Length; i++)
                {
                    bonePoses[i] = bindPoses[i];
                }
            }

            bonePosesInitialized = true;
        }
    }
}