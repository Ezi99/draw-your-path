using OctoXR.Collections;
using System;
using UnityEngine;

namespace OctoXR.Rendering
{
    [Serializable]
    public class ArmatureVisualizedHandSkeletonVisualizationComponent
    {
        [SerializeField]
        [HideInInspector]
        private Transform transform;
        public Transform Transform => transform;

        [SerializeField]
        [HideInInspector]
        private MeshRenderer renderer;
        public MeshRenderer Renderer => renderer;

        [SerializeField]
        [HideInInspector]
        private MeshFilter meshFilter;
        public MeshFilter MeshFilter => meshFilter;

        public ArmatureVisualizedHandSkeletonVisualizationComponent(GameObject gameObject)
        {
            if (!gameObject)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            transform = gameObject.transform;
            renderer = gameObject.GetComponent<MeshRenderer>();
            meshFilter = gameObject.GetComponent<MeshFilter>();
        }
    }

    [Serializable]
    public class ArmatureVisualizedHandBoneSegment : ArmatureVisualizedHandSkeletonVisualizationComponent
    {
        [SerializeField]
        [HideInInspector]
        private HandBoneId boneId;
        public HandBoneId BoneId => boneId;

        public ArmatureVisualizedHandBoneSegment(GameObject gameObject, HandBoneId boneId)
            : base(gameObject)
        {
            this.boneId = boneId;
        }
    }

    public class ArmatureVisualizedHandBone : HandBone
    {
        public const string JointObjectName = "JointVisualization";
        public const string SegmentObjectName = "SegmentVisualization";

        [Serializable]
        private class SegmentVisualizationCollection : ReadOnlyCollection<ArmatureVisualizedHandBoneSegment>
        {
            public void EnsureMinimumCapacity(int capacity)
            {
                if (items.Length < capacity)
                {
                    Array.Resize(ref items, capacity);
                }
            }

            public void Add(ArmatureVisualizedHandBoneSegment item)
            {
                items[count++] = item;
            }

            public void Clear() => List.Clear(items, ref count);
        }

        [SerializeField]
        [HideInInspector]
        private ArmatureVisualizedHandSkeletonVisualizationComponent joint;
        /// <summary>
        /// Gives access to objects and properties used for rendering joint of the hand armature bone. Joint in this context is 
        /// a connection point between this bone and its parent bone, i.e. the origin point of the bone. This value is initialized
        /// when the bone is added to a hand skeleton
        /// </summary>
        public ArmatureVisualizedHandSkeletonVisualizationComponent Joint => joint;

        [SerializeField]
        [HideInInspector]
        private SegmentVisualizationCollection segments = new SegmentVisualizationCollection();
        /// <summary>
        /// Gives access to objects and properties used for rendering segments of the hand armature bone. One segment in this context is 
        /// the line that goes from the bone's origin to the origin point of its child bone, or, in case there is no child bone in the 
        /// skeleton, to where its child bone's origin point would have been. These values are initialized when the bone is added to a hand 
        /// skeleton
        /// </summary>
        public ReadOnlyCollection<ArmatureVisualizedHandBoneSegment> Segments => segments;

        [SerializeField]
        [Tooltip("Radius of the joint visualization")]
        [Min(0)]
        private float jointRadius = ArmatureVisualizedHandSkeleton.DefaultJointRadius;
        /// <summary>
        /// Radius of the joint visualization
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

                ApplyJointRadius();

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Radius of bone segment visualizations")]
        [Min(0)]
        private float segmentRadius = ArmatureVisualizedHandSkeleton.DefaultSegmentRadius;
        /// <summary>
        /// Radius of bone segment visualizations
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

                ApplySegmentRadius();

                ObjectUtility.SetObjectDirty(this);
            }
        }

        protected override void Reset()
        {
            base.Reset();

            // if bone is still in skeleton (root bone is usually the only one that doesn't get removed from it upon reset)
            var removedFromSkeleton = !HandSkeleton;

            var jointTransform = Transform.Find(JointObjectName);

            if (jointTransform)
            {
                if (removedFromSkeleton)
                {
                    jointTransform.gameObject.SetActive(false);
                }
                else
                {
                    joint = new ArmatureVisualizedHandSkeletonVisualizationComponent(jointTransform.gameObject);
                }
            }

            var childBones = HandSkeletonConfiguration.ChildBones[BoneId];

            for (var i = 0; i < childBones.Count; i++)
            {
                var segmentName = GetSegmentName(childBones, i);
                var segmentTransform = Transform.Find(segmentName);

                if (segmentTransform)
                {
                    if (removedFromSkeleton)
                    {
                        segmentTransform.gameObject.SetActive(false);
                    }
                    else
                    {
                        segments.EnsureMinimumCapacity(childBones.Count);
                        segments.Add(new ArmatureVisualizedHandBoneSegment(segmentTransform.gameObject, childBones[i]));
                    }
                }
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!HandSkeleton)
            {
                joint = null;
                segments.Clear();
            }
            else
            {
                ApplyJointRadius();
                ApplySegmentRadius();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (!HandSkeleton)
            {
                joint = null;
                segments.Clear();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            DeactivateBoneVisualizations();
        }

        protected override void OnAddedToHandSkeleton()
        {
            var handSkeleton = HandSkeleton;
            var boneId = BoneId;
            var transform = Transform;

            Vector3 segmentScaleXZ;
            Material segmentMaterial = null;
            var assignSegmentMaterial = false;

            var jointTransform  = transform.Find(JointObjectName);
            var jointVisualizationObject = jointTransform ? jointTransform.gameObject : GameObject.CreatePrimitive(PrimitiveType.Sphere);

            jointVisualizationObject.name = JointObjectName;
            
            ObjectUtility.DestroyObject(jointVisualizationObject.GetComponent<Collider>());

            joint = new ArmatureVisualizedHandSkeletonVisualizationComponent(jointVisualizationObject);

            jointTransform = joint.Transform;

            jointTransform.SetParent(transform, false);

            if (handSkeleton is ArmatureVisualizedHandSkeleton armatureVisualizedSkeleton)
            {
                jointRadius = armatureVisualizedSkeleton.JointRadius;
                segmentRadius = armatureVisualizedSkeleton.SegmentRadius;

                if (joint.Renderer)
                {
                    joint.Renderer.sharedMaterial = armatureVisualizedSkeleton.JointMaterial;
                }

                segmentScaleXZ = Vector3.one * (armatureVisualizedSkeleton.SegmentRadius * 2f);
                segmentMaterial = armatureVisualizedSkeleton.SegmentMaterial;
                assignSegmentMaterial = true;
            }
            else
            {
                jointRadius = ArmatureVisualizedHandSkeleton.DefaultJointRadius;
                segmentScaleXZ = Vector3.one * (ArmatureVisualizedHandSkeleton.DefaultSegmentRadius * 2f);
            }

            if (boneId == HandBoneId.WristRoot)
            {
                jointRadius = ArmatureVisualizedHandSkeleton.DefaultRootRadius;
            }

            jointTransform.localScale = Vector3.one * (jointRadius * 2f);

            jointVisualizationObject.SetActive(true);

            var childBones = HandSkeletonConfiguration.ChildBones[boneId];

            segments.EnsureMinimumCapacity(childBones.Count);

            for (var i = 0; i < childBones.Count; i++)
            {
                var childBoneId = childBones[i];
                var segmentTransform = transform.Find(GetSegmentName(childBones, i));
                var segmentVisualizationObject = segmentTransform ? segmentTransform.gameObject : GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                segmentVisualizationObject.name = GetSegmentName(childBones, i);

                ObjectUtility.DestroyObject(segmentVisualizationObject.GetComponent<Collider>());

                var segment = new ArmatureVisualizedHandBoneSegment(segmentVisualizationObject, childBoneId);

                segmentTransform = segment.Transform;

                segmentTransform.SetParent(transform, false);

                // Cylinder created this way is aligned along y-axis, we'll leave it at that as this whole package prefers hand bones
                // aligned along y-axis as well
                segmentTransform.localScale = segmentScaleXZ;

                if (assignSegmentMaterial && segment.Renderer)
                {
                    segment.Renderer.sharedMaterial = segmentMaterial;
                }

                segmentVisualizationObject.SetActive(true);
                segments.Add(segment);
            }

            ResetBoneSegmentVisualizationPoses();
        }

        /// <summary>
        /// Resets the position and orientation of the bone segment visualization objects. This method only works if the bone is added
        /// to a hand skeleton and it is not a finger tip bone
        /// </summary>
        public void ResetBoneSegmentVisualizationPoses()
        {
            var handSkeleton = HandSkeleton;
            var boneId = BoneId;

            if (!handSkeleton || HandSkeletonConfiguration.IsFingerTipBone(boneId))
            {
                return;
            }

            var sourceSkeletonBones = handSkeleton.Bones;
            HandBoneKeyedReadOnlyCollection<Pose> sourcePoses = null;
            float segmentFallbackLength;

            if (handSkeleton is ArmatureVisualizedHandSkeleton armatureVisualizedSkeleton)
            {
                var visualizationOf = armatureVisualizedSkeleton.VisualizationOf;

                if (visualizationOf)
                {
                    sourceSkeletonBones = visualizationOf.Bones;

                    if (visualizationOf.PoseProvider)
                    {
                        sourcePoses = visualizationOf.PoseProvider.GetBoneAbsolutePoses();
                    }
                }
                else
                {
                    sourcePoses = handSkeleton.PoseProvider ? handSkeleton.PoseProvider.GetBoneAbsolutePoses() : null;
                }

                segmentFallbackLength = armatureVisualizedSkeleton.SegmentFallbackLength;
            }
            else
            {
                sourcePoses = handSkeleton.PoseProvider ? handSkeleton.PoseProvider.GetBoneAbsolutePoses() : null;
                segmentFallbackLength = ArmatureVisualizedHandSkeleton.DefaultSegmentFallbackLength;
            }

            var transform = Transform;
            var boneRotation = transform.rotation;
            var bonePose = sourcePoses != null ? sourcePoses[boneId] : new Pose(transform.position, boneRotation);
            var upDirection = Vector3.up;

            for (var i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];

                if (segment == null)
                {
                    continue;
                }

                var segmentTransform = segment.Transform;

                if (!segmentTransform)
                {
                    continue;
                }

                if (segmentTransform.parent != transform)
                {
                    segmentTransform.SetParent(transform, false);
                }

                var childBoneId = segment.BoneId;
                Vector3 segmentEndLocalPosition;

                if (sourcePoses != null)
                {
                    var childBonePose = sourcePoses[childBoneId];

                    segmentEndLocalPosition = childBonePose.position.RelativeTo(bonePose);
                }
                else if (sourceSkeletonBones.TryGetItem(childBoneId, out var childBone))
                {
                    segmentEndLocalPosition = childBone.BindPose.position;
                }
                else
                {
                    segmentEndLocalPosition = new Vector3(0, segmentFallbackLength, 0);
                }

                // Because segments are created via create primitive as cylinders, we know they start with the height of 2 units
                // and it is aligned along y-axis and the cylinder is also centered (it stretches from -1 to 1 on local y-axis),
                // so we are going to continue with that assumption...
                var segmentLength = segmentEndLocalPosition.magnitude;

                segmentTransform.localPosition = 0.5f * segmentEndLocalPosition;
                segmentTransform.localRotation = Quaternion.FromToRotation(upDirection, segmentEndLocalPosition);

                var segmentLocalScale = segmentTransform.localScale;

                segmentLocalScale.y = 0.5f * segmentLength;

                segmentTransform.localScale = segmentLocalScale;
            }
        }

        protected override void OnRemovedFromHandSkeleton(HandSkeleton handSkeleton)
        {
            DeactivateBoneVisualizations();
        }

        private void DeactivateBoneVisualizations()
        {
            if (joint != null && joint.Transform)
            {
                joint.Transform.gameObject.SetActive(false);
            }

            for (var i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];

                if (segment != null && segment.Transform)
                {
                    segment.Transform.gameObject.SetActive(false);
                }
            }

            segments.Clear();
        }

        private static string GetSegmentName(ReadOnlyCollection<HandBoneId> childBones, int childBoneIndex) 
            => childBones.Count == 1 ? SegmentObjectName : $"{SegmentObjectName}[{childBoneIndex}]";

        private void ApplyJointRadius()
        {
            if (joint != null && joint.Transform)
            {
                var scale = 2f * JointRadius;

                joint.Transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        private void ApplySegmentRadius()
        {
            var scale = 2f * segmentRadius;

            for (var i = 0; i < segments.Count; i++)
            {
                var segment = segments[i];

                if (segment != null && segment.Transform)
                {
                    var localScale = segment.Transform.localScale;

                    localScale.x = scale;
                    localScale.z = scale;

                    segment.Transform.localScale = localScale;
                }
            }
        }
    }
}
