using System;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class HandBone : MonoBehaviour, IHandBoneIdentifiable
    {
        private const string handBoneNotAddedToHandSkeletonMessage = nameof(HandBone) + " has not been added to a " + nameof(OctoXR.HandSkeleton);
        private const string handBoneNotRemovedFromHandSkeletonMessage = nameof(HandBone) + " has not been removed from its current " + 
            nameof(OctoXR.HandSkeleton);
        private const string handBoneAddedToAnotherHandSkeletonMessage = nameof(HandBone) + " has been added to a " + nameof(OctoXR.HandSkeleton);
        private const string handBoneNotAddedToHandSkeletonAtKeyMessage = nameof(HandBone) + " has not been added to the specified " +
            nameof(OctoXR.HandSkeleton) + " at the specified bone identity key";

        [SerializeField]
        [Tooltip("Hand skeleton that the bone is added to")]
        [PropertyDrawOptions(ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways)]
        private HandSkeleton handSkeleton;
        /// <summary>
        /// Hand skeleton that the bone is added to
        /// </summary>
        public HandSkeleton HandSkeleton => handSkeleton;

        [SerializeField]
        [Tooltip("Identity of the hand bone")]
        [PropertyDrawOptions(ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways)]
        private HandBoneId boneId;
        /// <summary>
        /// Identity of the hand bone
        /// </summary>
        public HandBoneId BoneId => boneId;

        /// <summary>
        /// Specifies whether the hand bone is the root bone of a hand skeleton
        /// </summary>
        public bool IsRootBone => boneId == HandBoneId.WristRoot;

        [SerializeField]
        [HideInInspector]
        private new Transform transform;
        /// <summary>
        /// Transform component of the hand bone
        /// </summary>
        public Transform Transform => transform;

        [SerializeField]
        [Tooltip("Parent bone of the hand bone in the bone's hand skeleton hierarchy")]
        [PropertyDrawOptions(ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways)]
        private HandBone parentBone;
        /// <summary>
        /// Parent bone of the hand bone in the bone's hand skeleton hierarchy
        /// </summary>
        public HandBone ParentBone => parentBone;

        [SerializeField]
        [HideInInspector]
        private bool isParentBoneClosestAncestor;
        /// <summary>
        /// Indicates whether the hand bone's parent bone is its closest possible ancestor in the hand bone hierarchy.
        /// This is false if the bone has no parent bone
        /// </summary>
        public bool IsParentBoneClosestAncestor => isParentBoneClosestAncestor;

        [SerializeField]
        [HideInInspector]
        private Pose bindPose = Pose.identity;
        /// <summary>
        /// Hand bone pose relative to the bone's parent that serves as bone's initial or default pose. This pose is reset every 
        /// time the bone is added to a hand skeleton. If the bone has no parent bone, i.e. it is root bone, then this value represents 
        /// bone's initial pose relative to world origin
        /// </summary>
        public Pose BindPose
        {
            get => bindPose;
            set
            {
                if (!Mathf.Approximately(Quaternion.Dot(value.rotation, value.rotation), 1f))
                {
                    throw new ArgumentException("Specified value's rotation must be quaternion of unit length");
                }
#if UNITY_EDITOR
                var setDirty = bindPose != value;
#endif
                bindPose = value;
#if UNITY_EDITOR
                if (setDirty)
                {
                    ObjectUtility.SetObjectDirty(this);
                }
#endif
            }
        }

        [SerializeField]
        [Tooltip("Specifies whether the bone's bind pose should be captured from bone's current transform. Bind pose is going to " +
            "be constantly updated with the bone's current transform state as long as this is enabled. This process occurs only " +
            "inside Unity editor while not in play mode, it has no effect while the application is playing")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private bool captureBindPose;
        /// <summary>
        /// Specifies whether the bone's bind pose should be captured from bone's current transform. Bind pose is going to 
        /// be constantly updated with the bone's current transform state as long as this is enabled. This process occurs only 
        /// inside Unity editor while not in play mode, it has no effect while the application is playing
        /// </summary>
        public bool CaptureBindPose
        {
            get => captureBindPose;
            set
            {
                if (captureBindPose != value)
                {
                    captureBindPose = value;
#if UNITY_EDITOR
                    if (Application.IsPlaying(this))
                    {
                        return;
                    }

                    if (captureBindPose)
                    {
                        ResetBindPose();
                    }

                    ObjectUtility.SetObjectDirty(this);
#endif
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        private bool notifiedAddedToHandSkeleton;

        private bool isBeingDestroyed;

        protected virtual void Reset()
        {
            if (transform)
            {
                return;
            }

            transform = base.transform;

            var scene = gameObject.scene;

            if (!scene.IsValid())
            {
                return;
            }

            var objects = ListPool<List<GameObject>>.Get();
            var handSkeletons = ListPool<List<HandSkeleton>>.Get();

            scene.GetRootGameObjects(objects);

            for (var i = 0; i < objects.Count; ++i)
            {
                objects[i].GetComponentsInChildren(true, handSkeletons);

                for (var j = 0; j < handSkeletons.Count; ++j)
                {
                    var handSkeleton = handSkeletons[j];
                    var boneIndex = handSkeleton.Bones.IndexOf(this);

                    if (boneIndex != -1)
                    {
                        this.handSkeleton = handSkeleton;
                        boneId = handSkeleton.Bones.GetBoneIdAt(boneIndex);
                        parentBone = handSkeleton.GetParentBone(boneId);
                        isParentBoneClosestAncestor = parentBone ? HandSkeletonConfiguration.ParentBones[boneId] == parentBone.BoneId : false;
                        notifiedAddedToHandSkeleton = true;

                        if (boneId != HandBoneId.WristRoot)
                        {
                            handSkeleton.RemoveBoneAt(boneIndex);
                        }

                        break;
                    }
                }
            }

            ListPool<List<HandSkeleton>>.Recycle(handSkeletons);
            ListPool<List<GameObject>>.Recycle(objects);
        }

        protected virtual void OnValidate()
        {
            Initialize();
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!transform || transform.gameObject != gameObject)
            {
                transform = base.transform;
            }

            if (handSkeleton && handSkeleton.Bones[boneId] != this)
            {
                // this scenario can happen when for example bone is copy-pasted in editor or Object.Instantiated
                handSkeleton = null;
                parentBone = null;
                isParentBoneClosestAncestor = false;
                notifiedAddedToHandSkeleton = false;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        protected virtual void OnDestroy()
        {
            isBeingDestroyed = true;

            if (handSkeleton)
            {
                var index = handSkeleton.Bones.IndexOf(this);

                if (index > 0)
                {
                    handSkeleton.RemoveBoneAt(index);
                }
            }
        }

        /// <summary>
        /// Initializes the <see cref="HandBone"/> upon the bone being added to a <see cref="OctoXR.HandSkeleton"/>. This is called 
        /// by the <see cref="OctoXR.HandSkeleton"/> after the bone has been added to it, calling it from any other context will 
        /// result in an exception being thrown
        /// </summary>
        /// <param name="handSkeleton">The hand skeleton the bone has been added to</param>
        /// <param name="boneId">The identity of the bone in the hand skeleton it has been added to</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void HandleAddedToHandSkeleton(HandSkeleton handSkeleton, HandBoneId boneId)
        {
            if (isBeingDestroyed)
            {
                throw new ObjectDisposedException(nameof(HandBone));
            }

            if (!handSkeleton)
            {
                throw new ArgumentNullException(nameof(handSkeleton));
            }

            if (this.handSkeleton)
            {
                throw new InvalidOperationException(handBoneAddedToAnotherHandSkeletonMessage);
            }

            if (handSkeleton.Bones[boneId] != this)
            {
                throw new ArgumentException(handBoneNotAddedToHandSkeletonAtKeyMessage);
            }

            this.handSkeleton = handSkeleton;
            this.boneId = boneId;
            parentBone = FindParentBoneInHandSkeleton(boneId, handSkeleton, out isParentBoneClosestAncestor);
            ResetBindPose();

            ObjectUtility.SetObjectDirty(this);

            var bones = handSkeleton.Bones;

            for (var i = 1; i < bones.Count; i++)
            {
                var bone = bones[i];
                var boneParent = FindParentBoneInHandSkeleton(bone.BoneId, handSkeleton, out bone.isParentBoneClosestAncestor);

                if (boneParent == this)
                {
                    bone.parentBone = this;
                    bone.ResetBindPose();

                    ObjectUtility.SetObjectDirty(bone);
                }
            }
        }

        /// <summary>
        /// Uninitializes the <see cref="HandBone"/> upon the bone being removed from its current <see cref="OctoXR.HandSkeleton"/>.
        /// This is called by the <see cref="OctoXR.HandSkeleton"/> after the bone has been removed from it, calling it from any other 
        /// context will result in an exception being thrown
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void HandleRemovedFromHandSkeleton()
        {
            if (!handSkeleton)
            {
                throw new InvalidOperationException(handBoneNotAddedToHandSkeletonMessage);
            }

            if (handSkeleton.Bones[boneId] == this)
            {
                throw new InvalidOperationException(handBoneNotRemovedFromHandSkeletonMessage);
            }

            var bones = handSkeleton.Bones;

            for (var i = 1; i < bones.Count; i++)
            {
                var bone = bones[i];

                if (bone.parentBone == this)
                {
                    bone.parentBone = parentBone;
                    bone.isParentBoneClosestAncestor = false;
                    bone.ResetBindPose();

                    ObjectUtility.SetObjectDirty(bone);
                }
            }

            handSkeleton = null;
            parentBone = null;
            isParentBoneClosestAncestor = false;

            ObjectUtility.SetObjectDirty(this);
        }

        /// <summary>
        /// Sends out the internal callback on the <see cref="HandBone"/> that signals to the derived behaviours that the bone
        /// has been added to a hand skeleton. This is called by the <see cref="OctoXR.HandSkeleton"/> after the bone has been
        /// added to it, calling it from any other context will either result in an exception being thrown or the method will
        /// have no effect at all
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void NotifyAddedToHandSkeleton()
        {
            if (!handSkeleton)
            {
                throw new InvalidOperationException(handBoneNotAddedToHandSkeletonMessage);
            }

            if (!notifiedAddedToHandSkeleton)
            {
                notifiedAddedToHandSkeleton = true;

                try
                {
                    OnAddedToHandSkeleton();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
#if UNITY_EDITOR
                if (notifiedAddedToHandSkeleton)
                {
                    ObjectUtility.SetObjectDirty(this);
                }
#endif
            }
        }

        /// <summary>
        /// Sends out the internal callback on the <see cref="HandBone"/> that signals to the derived behaviours that the bone
        /// has been removed from the specified hand skeleton. This is called by the <see cref="OctoXR.HandSkeleton"/> after 
        /// the bone has been removed from it, calling it from any other context will either result in an exception being thrown 
        /// or the method will have no effect at all
        /// </summary>
        /// <param name="handSkeleton">The hand skeleton that the bone has been removed from</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void NotifyRemovedFromHandSkeleton(HandSkeleton handSkeleton)
        {
            if (this.handSkeleton)
            {
                throw new InvalidOperationException(handBoneNotRemovedFromHandSkeletonMessage);
            }

            if (notifiedAddedToHandSkeleton)
            {
                notifiedAddedToHandSkeleton = false;

                try
                {
                    OnRemovedFromHandSkeleton(handSkeleton);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
#if UNITY_EDITOR
                if (!notifiedAddedToHandSkeleton)
                {
                    ObjectUtility.SetObjectDirty(this);
                }
#endif
            }
        }

        /// <summary>
        /// Called when the bone is added to a hand skeleton. Used as callback only
        /// </summary>
        protected virtual void OnAddedToHandSkeleton() { }

        /// <summary>
        /// Called after the bone has been removed from the specified hand skeleton. Used as callback only
        /// </summary>
        /// <param name="handSkeleton"></param>
        protected virtual void OnRemovedFromHandSkeleton(HandSkeleton handSkeleton) { }

        private void ResetBindPose()
        {
            if (parentBone)
            {
                bindPose.position = parentBone.Transform.InverseTransformPoint(transform.position);
                bindPose.rotation = Quaternion.Inverse(parentBone.Transform.rotation) * transform.rotation;
            }
            else
            {
                bindPose.position = transform.position;
                bindPose.rotation = transform.rotation;
            }
        }

        private static HandBone FindParentBoneInHandSkeleton(HandBoneId boneId, HandSkeleton handSkeleton, out bool isParentBoneClosestAncestor)
        {
            var bones = handSkeleton.Bones;
            var parentBoneId = HandSkeletonConfiguration.ParentBones[boneId];

            isParentBoneClosestAncestor = true;

            while (parentBoneId.HasValue)
            {
                var parentBoneIdValue = parentBoneId.Value;

                if (bones.TryGetItem(parentBoneIdValue, out var parentBone))
                {
                    return parentBone;
                }

                parentBoneId = HandSkeletonConfiguration.ParentBones[parentBoneIdValue];
                isParentBoneClosestAncestor = false;
            }

            isParentBoneClosestAncestor = false;

            return null;
        }
    }
}
