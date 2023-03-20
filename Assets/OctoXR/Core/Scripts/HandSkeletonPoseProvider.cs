using OctoXR.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR
{
    [DisallowMultipleComponent]
    public abstract class HandSkeletonPoseProvider : MonoBehaviour
    {
        [Serializable]
        private class InternalHandBonePoseCollection : HandBoneKeyedReadOnlyCollection<Pose>
        {
            public new ref Pose this[int index] => ref items[index];

            public InternalHandBonePoseCollection() : base(CreateInitialBonePoses()) { }

            private static Pose[] CreateInitialBonePoses()
            {
                var bonePoses = new Pose[HandSkeletonConfiguration.BoneCount];

                for (var i = 0; i < bonePoses.Length; i++)
                {
                    bonePoses[i] = Pose.identity;
                }

                return bonePoses;
            }
        }

        [SerializeField]
        [Tooltip("Scale of the hand skeleton")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private float scale = 1;
        /// <summary>
        /// Scale of the hand skeleton
        /// </summary>
        public float Scale
        {
            get => scale;
            protected set
            {
#if UNITY_EDITOR
                if (Mathf.Approximately(scale, value))
                {
                    return;
                }
#endif
                scale = value;
                boneAbsoluteScaledPosesNeedUpdate = true;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [HideInInspector]
        private bool boneRelativePosesNeedUpdate;

        [SerializeField]
        [HideInInspector]
        private InternalHandBonePoseCollection boneRelativePoses = new InternalHandBonePoseCollection();

        [SerializeField]
        [HideInInspector]
        private bool boneAbsolutePosesNeedUpdate;

        [SerializeField]
        [HideInInspector]
        private InternalHandBonePoseCollection boneAbsolutePoses = new InternalHandBonePoseCollection();

        [SerializeField]
        [HideInInspector]
        private bool boneAbsoluteScaledPosesNeedUpdate;

        [SerializeField]
        [HideInInspector]
        private InternalHandBonePoseCollection boneAbsoluteScaledPoses = new InternalHandBonePoseCollection();

        [SerializeField]
        [Tooltip("Event fired when pose provider's pose data is updated")]
        private UnityEvent onPoseDataUpdated = new UnityEvent();
        /// <summary>
        /// Event fired when pose provider's pose data is updated
        /// </summary>
        public UnityEvent OnPoseDataUpdated => onPoseDataUpdated;

        /// <summary>
        /// Specifies for which hand are poses provided by the hand skeleton pose provider
        /// </summary>
        /// <returns></returns>
        public abstract HandType GetHandType();

        /// <summary>
        /// Returns a list of current hand bone poses, relative to parent bones
        /// </summary>
        public HandBoneKeyedReadOnlyCollection<Pose> GetBoneRelativePoses()
        {
            OnGetBonePoses();

            if (boneRelativePosesNeedUpdate)
            {
                var rootPose = boneAbsolutePoses[0];

                boneRelativePoses[0] = rootPose;

                Pose parentBoneAbsolutePose;
                Pose boneAbsolutePose;

                for (var i = 0; i < HandSkeletonConfiguration.FingerCount; i++)
                {
                    var finger = HandSkeletonConfiguration.FingerBones[i];

                    parentBoneAbsolutePose = rootPose;

                    for (var j = 0; j < finger.Count; j++)
                    {
                        var boneId = (int)finger[j];

                        boneAbsolutePose = boneAbsolutePoses[boneId];
                        boneRelativePoses[boneId] = boneAbsolutePose.RelativeTo(parentBoneAbsolutePose);
                        parentBoneAbsolutePose = boneAbsolutePose;
                    }
                }

                boneRelativePosesNeedUpdate = false;
                ObjectUtility.SetObjectDirty(this);
            }

            return boneRelativePoses;
        }

        /// <summary>
        /// Returns a list of current absolute unscaled hand bone poses
        /// </summary>
        public HandBoneKeyedReadOnlyCollection<Pose> GetBoneAbsolutePoses()
        {
            OnGetBonePoses();

            if (boneAbsolutePosesNeedUpdate)
            {
                var rootPose = boneRelativePoses[0];

                boneAbsolutePoses[0] = rootPose;

                Pose boneAbsolutePose;

                for (var i = 0; i < HandSkeletonConfiguration.FingerCount; i++)
                {
                    var finger = HandSkeletonConfiguration.FingerBones[i];

                    boneAbsolutePose = rootPose;

                    for (var j = 0; j < finger.Count; j++)
                    {
                        var boneId = (int)finger[j];

                        boneAbsolutePose = boneRelativePoses[boneId].TransformedBy(in boneAbsolutePose);
                        boneAbsolutePoses[boneId] = boneAbsolutePose;
                    }
                }

                boneAbsolutePosesNeedUpdate = false;
                ObjectUtility.SetObjectDirty(this);
            }

            return boneAbsolutePoses;
        }

        /// <summary>
        /// Returns a list of current absolute scaled hand bone poses
        /// </summary>
        public HandBoneKeyedReadOnlyCollection<Pose> GetBoneAbsoluteScaledPoses()
        {
            OnGetBonePoses();

            if (boneAbsolutePosesNeedUpdate)
            {
                var rootPose = boneRelativePoses[0];

                boneAbsolutePoses[0] = rootPose;

                Pose boneAbsolutePose;

                for (var i = 0; i < HandSkeletonConfiguration.FingerCount; i++)
                {
                    var finger = HandSkeletonConfiguration.FingerBones[i];

                    boneAbsolutePose = rootPose;

                    for (var j = 0; j < finger.Count; j++)
                    {
                        var boneId = (int)finger[j];

                        boneAbsolutePose = boneRelativePoses[boneId].TransformedBy(in boneAbsolutePose);
                        boneAbsolutePoses[boneId] = boneAbsolutePose;

                        ref var boneAbsoluteScaledPose = ref boneAbsoluteScaledPoses[boneId];

                        boneAbsoluteScaledPose.position = rootPose.position + scale * (boneAbsolutePose.position - rootPose.position);
                        boneAbsoluteScaledPose.rotation = boneAbsolutePose.rotation;
                    }
                }

                boneAbsolutePosesNeedUpdate = false;
                boneAbsoluteScaledPosesNeedUpdate = false;
                ObjectUtility.SetObjectDirty(this);

                return boneAbsoluteScaledPoses;
            }

            if (boneAbsoluteScaledPosesNeedUpdate)
            {
                var rootPose = boneAbsolutePoses[0];

                boneAbsoluteScaledPoses[0] = rootPose;

                for (var i = 1; i < boneAbsoluteScaledPoses.Count; i++)
                {
                    ref var boneAbsolutePose = ref boneAbsolutePoses[i];
                    ref var boneAbsoluteScaledPose = ref boneAbsoluteScaledPoses[i];

                    boneAbsoluteScaledPose.position = rootPose.position + scale * (boneAbsolutePose.position - rootPose.position);
                    boneAbsoluteScaledPose.rotation = boneAbsolutePose.rotation;
                }

                boneAbsoluteScaledPosesNeedUpdate = false;
                ObjectUtility.SetObjectDirty(this);
            }

            return boneAbsoluteScaledPoses;
        }

        protected void SetBoneRelativePoses(IReadOnlyList<Pose> poses)
        {
            ThrowIfPosesNullOrNotEnoughItems(poses);

            var posesChanged = CopyPoses(poses, boneRelativePoses);
#if UNITY_EDITOR
            var setDirty = posesChanged ||
                boneRelativePosesNeedUpdate ||
                (boneAbsolutePosesNeedUpdate | posesChanged) != boneAbsolutePosesNeedUpdate ||
                (boneAbsoluteScaledPosesNeedUpdate | posesChanged) != boneAbsoluteScaledPosesNeedUpdate;
#endif
            boneRelativePosesNeedUpdate = false;
            boneAbsolutePosesNeedUpdate |= posesChanged;
            boneAbsoluteScaledPosesNeedUpdate |= posesChanged;

#if UNITY_EDITOR
            if (setDirty)
            {
                ObjectUtility.SetObjectDirty(this);
            }
#endif
            NotifyPoseDataUpdated();
        }

        protected void SetBoneAbsolutePoses(IReadOnlyList<Pose> poses)
        {
            ThrowIfPosesNullOrNotEnoughItems(poses);

            var posesChanged = CopyPoses(poses, boneAbsolutePoses);
#if UNITY_EDITOR
            var setDirty = posesChanged ||
                (boneRelativePosesNeedUpdate | posesChanged) != boneRelativePosesNeedUpdate ||
                boneAbsolutePosesNeedUpdate ||
                (boneAbsoluteScaledPosesNeedUpdate | posesChanged) != boneAbsoluteScaledPosesNeedUpdate;
#endif
            boneRelativePosesNeedUpdate |= posesChanged;
            boneAbsolutePosesNeedUpdate = false;
            boneAbsoluteScaledPosesNeedUpdate |= posesChanged;
#if UNITY_EDITOR
            if (setDirty)
            {
                ObjectUtility.SetObjectDirty(this);
            }
#endif
            NotifyPoseDataUpdated();
        }

        private static void ThrowIfPosesNullOrNotEnoughItems(IReadOnlyList<Pose> poses)
        {
            if (poses == null)
            {
                throw new ArgumentNullException(nameof(poses));
            }

            if (poses.Count < HandSkeletonConfiguration.BoneCount)
            {
                throw new ArgumentException("The number of items cannot be less than the total number of unique hand " +
                    $"skeleton bones ({HandSkeletonConfiguration.BoneCount})", nameof(poses));
            }
        }

        private void NotifyPoseDataUpdated()
        {
            try
            {
                onPoseDataUpdated.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static bool CopyPoses(IReadOnlyList<Pose> sourcePoses, InternalHandBonePoseCollection destinationPoses)
        {
#if UNITY_EDITOR
            var posesChanged = false;
#endif
            for (var i = 0; i < sourcePoses.Count; i++)
            {
                var sourcePose = sourcePoses[i];
                ref var destinationPose = ref destinationPoses[i];
#if UNITY_EDITOR
                posesChanged = posesChanged || sourcePose != destinationPose;
#endif
                destinationPose = sourcePose;
            }
#if UNITY_EDITOR
            return posesChanged;
#else
            return true;
#endif
        }

        /// <summary>
        /// Callback method that gets called whenever a method for getting current bone poses is called, just before relevant
        /// poses are calculated and finally returned by the pose provider
        /// </summary>
        protected virtual void OnGetBonePoses() { }

        [Obsolete("Use Scale property setter and SetBoneRelativePoses method for setting scale and relative bone poses separately")]
        protected void SetScaleAndBoneRelativePoses(float scale, IReadOnlyList<Pose> poses)
        {
            Scale = scale;
            SetBoneRelativePoses(poses);
        }

        [Obsolete("Use Scale property setter and SetBoneAbsolutePoses method for setting scale and absolute bone poses separately")]
        protected void SetScaleAndBoneAbsolutePoses(float scale, IReadOnlyList<Pose> poses)
        {
            Scale = scale;
            SetBoneAbsolutePoses(poses);
        }
    }
}
