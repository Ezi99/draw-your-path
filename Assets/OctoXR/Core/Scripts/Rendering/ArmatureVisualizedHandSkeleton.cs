using OctoXR.Collections;
using System;
using UnityEngine;

namespace OctoXR.Rendering
{
    [RequireComponent(typeof(ArmatureVisualizedHandBone))]
    public class ArmatureVisualizedHandSkeleton : VisualizedHandSkeleton
    {
        public const float DefaultRootRadius = 0.007f;
        public const float DefaultJointRadius = 0.005f;
        public const float DefaultSegmentRadius = 0.003f;
        public const float DefaultSegmentFallbackLength = 0.02f;

        [Serializable]
        private class ArmatureVisualizedHandBoneCollection : ReadOnlyCollection<ArmatureVisualizedHandBone>
        {
            public ArmatureVisualizedHandBoneCollection() : base(new ArmatureVisualizedHandBone[HandSkeletonConfiguration.BoneCount], 0) { }

            public void Set(int index, ArmatureVisualizedHandBone bone) => items[index] = bone;
            public void Insert(int index, ArmatureVisualizedHandBone bone) => List.Insert(ref items, index, bone, ref count);
            public void RemoveAt(int index) => List.RemoveAt(items, index, ref count);
            public void Clear() => List.Clear(items, ref count);
            public void RemoveRange(int index)
            {
                for (var i = index; i < count; i++)
                {
                    items[i] = null;
                }

                count = index;
            }
        }

        [SerializeField]
        [HideInInspector]
        private ArmatureVisualizedHandBoneCollection visualizedBones = new ArmatureVisualizedHandBoneCollection();
        /// <summary>
        /// Collection of bones contained in the armature-visualized hand skeleton. These are the same instances as they appear in
        /// the base <see cref="HandSkeleton.Bones"/> collection and in the exact same order, but strongly typed as 
        /// <see cref="ArmatureVisualizedHandBone"/>s 
        /// </summary>
        public ReadOnlyCollection<ArmatureVisualizedHandBone> VisualizedBones => visualizedBones;

        [SerializeField]
        [Tooltip("Hand skeleton being visualized. If this is not assigned the armature-visualized hand skeleton functions as " +
            "a visualization in and of itself and its poses are read from its pose provider as is usual for a hand skeleton and " +
            "bones can be added or removed from it at will")]
        private HandSkeleton visualizationOf;
        /// <summary>
        /// Hand skeleton being visualized. If this is not assigned the armature-visualized hand skeleton functions as 
        /// a visualization in and of itself and its poses are read from its pose provider as is usual for a hand skeleton and 
        /// bones can be added or removed from it at will
        /// </summary>
        public HandSkeleton VisualizationOf
        {
            get => visualizationOf;
            set
            {
#if UNITY_EDITOR
                if (visualizationOf == value)
                {
                    return;
                }
#endif
                visualizationOf = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        /// <summary>
        /// Radius of the skeleton root visualization
        /// </summary>
        public float RootRadius
        {
            get => visualizedBones[0].JointRadius;
            set => visualizedBones[0].JointRadius = value;
        }

        [SerializeField]
        [Tooltip("Default radius of joint visualizations")]
        [Min(0)]
        private float jointRadius = DefaultJointRadius;
        /// <summary>
        /// Default radius of joint visualizations
        /// </summary>
        public float JointRadius
        {
            get => jointRadius;
            set
            {
#if UNITY_EDITOR
                if (jointRadius == value)
                {
                    return;
                }
#endif
                jointRadius = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Default radius of segment visualizations")]
        [Min(0)]
        private float segmentRadius = DefaultSegmentRadius;
        /// <summary>
        /// Default radius of segment visualizations
        /// </summary>
        public float SegmentRadius
        {
            get => segmentRadius;
            set
            {
#if UNITY_EDITOR
                if (segmentRadius == value)
                {
                    return;
                }
#endif
                segmentRadius = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Default material of joint visualizations")]
        private Material jointMaterial;
        /// <summary>
        /// Default material of joint visualizations
        /// </summary>
        public Material JointMaterial
        {
            get => jointMaterial;
            set
            {
#if UNITY_EDITOR
                if (jointMaterial == value)
                {
                    return;
                }
#endif
                jointMaterial = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Default material of segment visualizations")]
        private Material segmentMaterial;
        /// <summary>
        /// Default material of segment visualizations
        /// </summary>
        public Material SegmentMaterial
        {
            get => segmentMaterial;
            set
            {
#if UNITY_EDITOR
                if (segmentMaterial == value)
                {
                    return;
                }
#endif
                segmentMaterial = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Length of the visualized bone segments to use when there is no way to determine target length of the segment. " +
            "Such scenario can happen with some unlikely skeleton configurations")]
        [Min(0)]
        private float segmentFallbackLength = DefaultSegmentFallbackLength;
        /// <summary>
        /// Length of the visualized bone segments to use when there is no way to determine target length of the segment. 
        /// Such scenario can happen with some unlikely skeleton configurations
        /// </summary>
        public float SegmentFallbackLength
        {
            get => segmentFallbackLength;
            set
            {
#if UNITY_EDITOR
                if (segmentFallbackLength == value)
                {
                    return;
                }
#endif
                segmentFallbackLength = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        protected override void Reset()
        {
            base.Reset();

            EnsureBonesAreValid();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            EnsureBonesAreValid();
        }

        protected override void Awake()
        {
            base.Awake();

            EnsureBonesAreValid();
        }

        private void EnsureBonesAreValid()
        {
            var baseBones = Bones;

            visualizedBones.RemoveRange(baseBones.Count);

            for (var i = 0; i < visualizedBones.Count; i++)
            {
                visualizedBones.Set(i, (ArmatureVisualizedHandBone)baseBones[i]);
            }

            for (var i = visualizedBones.Count; i < baseBones.Count; i++)
            {
                visualizedBones.Insert(i, (ArmatureVisualizedHandBone)baseBones[i]);
            }
        }

        protected override void LateUpdate()
        {
            if (visualizationOf)
            {
                SyncBonesWithVisualizationSource();
                UpdateHandSkeletonPoseFromVisualizationSource();
            }
            else
            {
                UpdatePose();

                for (var i = 0; i < visualizedBones.Count; i++)
                {
                    visualizedBones[i].ResetBoneSegmentVisualizationPoses();
                }
            }
            
            SetBoneBindPoses();
            FinalizePoseUpdate();
        }

        protected override HandBone CreateBone(GameObject boneObject)
        {
            if (!boneObject)
            {
                return null;
            }

            return CreateArmatureVisualizedBone(boneObject);
        }

        protected override void BoneAdded(HandBone bone, int index)
        {
            if (bone is ArmatureVisualizedHandBone armatureVisualizedBone)
            {
                visualizedBones.Insert(index, armatureVisualizedBone);
            }
            else
            {
                RemoveBoneAt(index);
            }
        }

        protected override void BoneRemoved(HandBone bone, int removedAtIndex)
        {
            if (removedAtIndex < visualizedBones.Count)
            {
                visualizedBones.RemoveAt(removedAtIndex);
            }
        }

        private static HandBone CreateArmatureVisualizedBone(GameObject boneObject)
        {
            if (!boneObject.TryGetComponent<ArmatureVisualizedHandBone>(out var armatureVisualizedBone))
            {
                if (boneObject.TryGetComponent<HandBone>(out var bone))
                {
                    ObjectUtility.DestroyObject(bone);
                }

                armatureVisualizedBone = boneObject.AddComponent<ArmatureVisualizedHandBone>();
            }

            return armatureVisualizedBone;
        }

        private void SyncBonesWithVisualizationSource()
        {
            var baseBones = Bones;
            var sourceBones = visualizationOf.Bones;
#if UNITY_EDITOR
            var setDirty = false;
#endif

            for (var i = 0; i < baseBones.Count; i++)
            {
                var bone = baseBones[i];

                if (!sourceBones.Contains(bone.BoneId))
                {
                    RemoveBoneAt(i--);
#if UNITY_EDITOR
                    setDirty = true;
#endif
                }
            }

            for (var i = 0; i < sourceBones.Count; i++)
            {
                var srcBone = sourceBones[i];

                if (!baseBones.Contains(srcBone.BoneId))
                {
                    var boneObject = new GameObject($"Bone_{srcBone.BoneId}");

                    boneObject.transform.SetParent(Transform, false);

                    AddBone(srcBone.BoneId, boneObject);
#if UNITY_EDITOR
                    setDirty = true;
#endif
                }
            }
#if UNITY_EDITOR
            if (setDirty)
            {
                ObjectUtility.SetObjectDirty(this);
            }
#endif
        }

        private void UpdateHandSkeletonPoseFromVisualizationSource()
        {
            var transform = Transform;

            var scale = transform.lossyScale;
            var scaleInv = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);
            var parentScaleInv = Vector3.Scale(transform.localScale, scaleInv);

            var baseScale = Scale;
            var sourceTransform = visualizationOf.Transform;
            var sourceTransformPosition = sourceTransform.position;

            transform.SetPositionAndRotation(sourceTransformPosition, sourceTransform.rotation);
            transform.localScale = baseScale * Vector3.Scale(parentScaleInv, sourceTransform.lossyScale);

            visualizedBones[0].ResetBoneSegmentVisualizationPoses();

            var sourceBones = visualizationOf.Bones;

            for (var i = 1; i < sourceBones.Count; i++)
            {
                var srcBoneTransform = sourceBones[i].Transform;
                var bone = visualizedBones[i];
                var boneTransform = bone.Transform;

                scale = boneTransform.lossyScale;
                scaleInv = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);
                parentScaleInv = Vector3.Scale(boneTransform.localScale, scaleInv);

                var bonePosition = sourceTransformPosition + baseScale * (srcBoneTransform.position - sourceTransformPosition);

                boneTransform.SetPositionAndRotation(bonePosition, srcBoneTransform.rotation);
                boneTransform.localScale = baseScale * Vector3.Scale(parentScaleInv, srcBoneTransform.lossyScale);

                bone.ResetBoneSegmentVisualizationPoses();
            }
        }
    }
}
