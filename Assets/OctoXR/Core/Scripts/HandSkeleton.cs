using OctoXR.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR
{
    [Serializable]
    public class HandBoneEvent : UnityEvent<HandBoneId> { }

    [Serializable]
    public class HandSkeletonBoneAddedEvent : HandBoneEvent { }

    [Serializable]
    public class HandSkeletonBoneRemovedEvent : HandBoneEvent { }

    [DefaultExecutionOrder(-80)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HandBone))]
    [ExecuteAlways]
    public abstract class HandSkeleton : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Provider of the hand skeleton's target poses")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private HandSkeletonPoseProvider poseProvider;
        /// <summary>
        /// Provider of the hand skeleton's target poses
        /// </summary>
        public HandSkeletonPoseProvider PoseProvider
        { 
            get => poseProvider;
            set
            {
#if UNITY_EDITOR
                if (poseProvider == value)
                {
                    return;
                }
#endif
                poseProvider = value;
                UpdateHandType();
            }
        }

        protected void UpdateHandType()
        {
            handType = poseProvider ? poseProvider.GetHandType() : handType;

            ObjectUtility.SetObjectDirty(this);
        }

        [SerializeField]
        [Tooltip("Is this the hand skeleton of a left or right hand. Note that this can be changed only if there is no pose provider " +
            "assigned for the hand skeleton")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private HandType handType;
        /// <summary>
        /// Is this the hand skeleton of a left or right hand. Note that this can be changed only if there is no pose provider
        /// assigned for the hand skeleton
        /// </summary>
        public HandType HandType 
        { 
            get => poseProvider ? poseProvider.GetHandType() : handType;
            set
            {
                if (handType == value)
                {
                    return;
                }
                
                if (poseProvider)
                {
                    const string message = "Hand type of a hand skeleton cannot be changed when the hand skeleton has pose provider assigned";

                    throw new InvalidOperationException(message);
                }

                handType = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Determines whether the hand skeleton should apply scale obtained from pose provider")]
        private bool applyPoseProviderScale;
        /// <summary>
        /// Determines whether the hand skeleton should apply scale obtained from pose provider
        /// </summary>
        public bool ApplyPoseProviderScale 
        { 
            get => applyPoseProviderScale;
            set
            {
#if UNITY_EDITOR
                if (applyPoseProviderScale == value)
                {
                    return;
                }
#endif
                applyPoseProviderScale = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Scale to apply to hand skeleton pose. This value will be multiplied with the pose provider's scale if there " +
            "is a pose provider assigned to the hand skeleton and the application of its scale is enabled")]
        [Range(0.1f, 5)]
        private float scale = 1f;
        /// <summary>
        /// Scale to apply to hand skeleton pose. This value will be multiplied with the pose provider's scale if there 
        /// is a pose provider assigned to the hand skeleton and the application of its scale is enabled
        /// </summary>
        public float Scale 
        { 
            get => scale;
            set
            {
#if UNITY_EDITOR
                if (scale == value)
                {
                    return;
                }
#endif
                scale = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Event fired when hand skeleton is finished with updating its pose")]
        private UnityEvent onPoseUpdated = new UnityEvent();
        /// <summary>
        /// Event fired when hand skeleton is finished with updating its pose
        /// </summary>
        public UnityEvent OnPoseUpdated => onPoseUpdated;

        [SerializeField]
        [HideInInspector]
        private new Transform transform;
        public Transform Transform => transform;

        [Serializable]
        private class EditorHandBoneCollection
        {
            [Tooltip("Bones that make up the hand skeleton")]
            [HandBonePropertyDrawOptions(
                ReadOnly = ReadOnlyPropertyDrawOptions.NotReadOnly,
                SetValueViaPropertyOrMethod = true,
                SetValueViaPropertyOrMethodName = nameof(SetBone))]
            public GameObject[] Bones
#if !UNITY_EDITOR
            = null;
#else
            = new GameObject[HandSkeletonConfiguration.BoneCount];
#endif
            public void SetBone(GameObject boneObject, HandSkeleton handSkeleton, int index)
            {
                var boneId = (HandBoneId)index;

                if (handSkeleton.bones.TryGetItem(boneId, out var bone) && bone.gameObject == boneObject)
                {
                    return;
                }

                handSkeleton.RemoveBone(boneId);

                if (boneObject)
                {
                    handSkeleton.AddBone(boneId, boneObject);
                }
            }
        }

#if UNITY_EDITOR
        // Bones visible in editor only, not used in runtime
        [SerializeField]
        [Tooltip("Bones that make up the hand skeleton")]
        [PropertyDrawOptions(ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways, SkipInInspectorPropertyHierarchy = true)]
#endif
        private EditorHandBoneCollection editorBoneCollection
#if UNITY_EDITOR
            = new EditorHandBoneCollection();
#else
            = null;
#endif
        [Serializable]
        private class HandBoneCollection : HandBoneKeyedSparseReadOnlyCollection<HandBone>
        {
            public HandBoneKeyedSparseCollection<HandBone> ModifiableCollection => Items;   
        }

        [HideInInspector]
        [SerializeField]
        [Tooltip("Bones that make up the hand skeleton")]
        private HandBoneCollection bones = new HandBoneCollection();
        /// <summary>
        /// Bones that make up the hand skeleton
        /// </summary>
        public HandBoneKeyedSparseReadOnlyCollection<HandBone> Bones => bones;

        // stores parent bone indices (IDs) for each possible hand bone ID, at 0 will always be -1, but from 1 onward IDs are always set and
        // updated to ID of the parent bone (closest parent up the bone hierarchy) that is currently in hand skeleton,
        // if the corresponding parent bone ID is not in the skeleton the stored value is -1
        [SerializeField]
        [HideInInspector]
        private int[] parentBones = CreateInitialParentBoneIndices();

        [SerializeField]
        [Tooltip("Event sent when a bone is added to the hand skeleton")]
        private HandSkeletonBoneAddedEvent onBoneAdded = new HandSkeletonBoneAddedEvent();
        /// <summary>
        /// Event sent when a bone is added to the hand skeleton
        /// </summary>
        public HandSkeletonBoneAddedEvent OnBoneAdded => onBoneAdded;

        [SerializeField]
        [Tooltip("Event sent when a bone is removed from the hand skeleton")]
        private HandSkeletonBoneRemovedEvent onBoneRemoved = new HandSkeletonBoneRemovedEvent();
        /// <summary>
        /// Event sent when a bone is removed from the hand skeleton
        /// </summary>
        public HandSkeletonBoneRemovedEvent OnBoneRemoved => onBoneRemoved;

        private static int[] CreateInitialParentBoneIndices()
        {
            var parentBones = new int[HandSkeletonConfiguration.BoneCount];

            for (var i = 0; i < parentBones.Length; ++i)
            {
                parentBones[i] = -1;
            }

            return parentBones;
        }

        /// <summary>
        /// Indicates whether the hand skeleton contains all possible hand bones
        /// </summary>
        public bool IsComplete => bones.Count == HandSkeletonConfiguration.BoneCount;

        private bool isBeingDestroyed;

        protected virtual void Reset()
        {
            if (!transform)
            {
                transform = base.transform;

                HandleBonesWhenReset();
            }
        }

        protected virtual void OnValidate()
        {
            InitializeAndEnsureStateIsValid();
        }

        protected virtual void Awake()
        {
            InitializeAndEnsureStateIsValid();
        }

        private void InitializeAndEnsureStateIsValid()
        {
            transform = base.transform;
#if UNITY_EDITOR
            var setThisDirty = false;
#endif
            var rootBone = GetComponent<HandBone>();

            // everything checked from here to the end of method below happens if the hand skeleton was copy-pasted
            // in editor or Object.Instantiated or who knows...
            if (rootBone.HandSkeleton && rootBone.HandSkeleton != this && rootBone.HandSkeleton.Bones.Contains(rootBone))
            {
                // skeleton was probably added to an existing skeleton's hierarchy, no other choice but to destroy it
                DestroyThisAndThrow();
            }

            var scene = gameObject.scene;

            for (var i = 0; i < bones.Count; i++)
            {
                var bone = bones[i];

                if (bone.HandSkeleton != this)
                {
                    RemoveBoneFromBoneCollectionDirect(bone.BoneId, i--);
#if UNITY_EDITOR
                    setThisDirty = true;
#endif
                    continue;
                }

                if (bone.gameObject.scene != scene)
                {
                    RemoveBoneFromBoneCollectionDirect(bone.BoneId, i);
                    InvokeAllBoneRemovedCallbacks(bone.BoneId, bone, i--);
#if UNITY_EDITOR
                    setThisDirty = true;
#endif
                }
            }

            if (!bones.Contains(rootBone))
            {
                if (!rootBone.HandSkeleton)
                {
                    AddBone(HandBoneId.WristRoot, gameObject);
                }
                else
                {
                    // add the bone quietly, maybe just some jumbled references occured, but it has been added to this skeleton on some previous occasion
                    bones.ModifiableCollection.Add(HandBoneId.WristRoot, rootBone);
#if UNITY_EDITOR
                    editorBoneCollection.Bones[0] = rootBone.gameObject;
                    setThisDirty = true;
#endif
                }
            }

            for (var i = 0; i < HandSkeletonConfiguration.Bones.Count; i++)
            {
                var boneId = HandSkeletonConfiguration.Bones[i];
                var parentBoneId = FindParentBone(boneId);
                ref var parentBoneIndex = ref parentBones[(int)boneId];

                var newParentBoneIndex = parentBoneId.HasValue ? (int)parentBoneId : -1;
#if UNITY_EDITOR
                if (newParentBoneIndex != parentBoneIndex)
                {
                    setThisDirty = true;
                }
#endif
                parentBoneIndex = newParentBoneIndex;
            }

#if UNITY_EDITOR
            if (setThisDirty)
            {
                ObjectUtility.SetObjectDirty(this);
            }
#endif
        }

        protected virtual void OnDestroy()
        {
            isBeingDestroyed = true;

            for (var i = bones.Count - 1; i >= 0; i--)
            {
                var bone = bones.ModifiableCollection.RemoveAt(i);
                var boneId = bone.BoneId;

                bone.HandleRemovedFromHandSkeleton();
                bone.NotifyRemovedFromHandSkeleton(this);

                NotifyBoneRemoved(boneId);
            }
        }

        private void InvokeAllBoneRemovedCallbacks(HandBoneId boneId, HandBone bone, int removedAtIndex)
        {
            bone.HandleRemovedFromHandSkeleton();

            try
            {
                BoneRemoved(bone, removedAtIndex);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (bone && !bone.HandSkeleton)
            {
                bone.NotifyRemovedFromHandSkeleton(this);
            }

            if (!bones.Contains(boneId))
            {
                NotifyBoneRemoved(boneId);
            }
        }

        private void NotifyBoneRemoved(HandBoneId boneId)
        {
            try
            {
                onBoneRemoved.Invoke(boneId);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void HandleBonesWhenReset()
        {
            var scene = gameObject.scene;

            if (!scene.IsValid())
            {
                return;
            }

            var objects = ListPool<List<GameObject>>.Get();
            var bones = ListPool<List<HandBone>>.Get();

            scene.GetRootGameObjects(objects);

            for (var i = 0; i < objects.Count; ++i)
            {
                objects[i].GetComponentsInChildren(true, bones);

                for (var j = 0; j < bones.Count; ++j)
                {
                    var bone = bones[j];

                    if (bone.HandSkeleton == this)
                    {
#if UNITY_EDITOR
                        editorBoneCollection.Bones[(int)bone.BoneId] = bone.gameObject;
#endif
                        // somehow this happens after reset, mainly with root bone - it remains in collection
                        // despite Reset defaulting all fields, so remove first here just in case
                        this.bones.ModifiableCollection.Remove(bone.BoneId);
                        this.bones.ModifiableCollection.Add(bone.BoneId, bone);
                    }
                }
            }

            if (!this.bones.Contains(HandBoneId.WristRoot))
            {
                var rootBone = GetComponent<HandBone>();

                if (rootBone.HandSkeleton) // also skeleton added to an existing skeleton hierarchy
                {
                    DestroyThisAndThrow();
                }

                AddBone(HandBoneId.WristRoot, rootBone.gameObject);
            }

            ClearBones();

            ListPool<List<HandBone>>.Recycle(bones);
            ListPool<List<GameObject>>.Recycle(objects);
        }

        private void DestroyThisAndThrow()
        {
            var gameObjectName = gameObject.name;

            ObjectUtility.DestroyObject(this);

            throw new InvalidOperationException($"Hand skeleton cannot be added to the game object '{gameObjectName}'. " +
                $"It is likely that the game object is already part of an existing hand skeleton's hierarchy");
        }

        private void RemoveBoneFromBoneCollectionDirect(HandBoneId boneId, int index)
        {
#if UNITY_EDITOR
            editorBoneCollection.Bones[(int)boneId] = null;
#endif
            bones.ModifiableCollection.RemoveAt(index);
        }

        private HandBoneId? FindParentBone(HandBoneId boneId)
        {
            var parentBoneId = HandSkeletonConfiguration.ParentBones[boneId];

            while (parentBoneId.HasValue)
            {
                var parentBoneIdValue = parentBoneId.Value;

                if (bones.Contains(parentBoneIdValue))
                {
                    return parentBoneIdValue;
                }

                parentBoneId = HandSkeletonConfiguration.ParentBones[parentBoneIdValue];
            }

            return parentBoneId;
        }

        private void ResetParentBoneIndices(int startIndex)
        {
            for (var i = startIndex; i < parentBones.Length; ++i)
            {
                var id = (HandBoneId)i;
                ref var parentBoneIndex = ref parentBones[i];
                var newParentBoneId = FindParentBone(id);

                parentBoneIndex = newParentBoneId.HasValue ? (int)newParentBoneId : -1;
            }
        }

        private HandBone CreateBoneSafe(GameObject boneObject)
        {
            var bone = CreateBone(boneObject);

            if (!bone || bone.gameObject != boneObject)
            {
                throw new InvalidOperationException($"{nameof(HandBone)} returned from {nameof(CreateBone)} method must be attached " +
                    "to the object that was specified as parameter to the method");
            }

            return bone;
        }

        /// <summary>
        /// Sets positions and scales of bones in the hand skeleton based on current absolute scales and positions of bones.
        /// These are applied to bone transforms directly. This method is best used for situations when there is no pose provider 
        /// associated with the hand skeleton
        /// </summary>
        /// <param name="handScale">Total scale of the hand, presumably calculated from <see cref="Scale"/> and pose provider scale 
        /// (if applicable)</param>
        protected void SetBoneScalesAndScaledPositions(float handScale)
        {
            var bones = Bones;

            transform.localScale = GetBoneScale(transform, handScale);

            for (var i = 1; i < bones.Count; i++)
            {
                var boneTransform = bones[i].Transform;

                boneTransform.localScale = GetBoneScale(boneTransform, handScale);
                boneTransform.position = GetBoneScaledPosition(boneTransform, handScale);
            }
        }

        /// <summary>
        /// Returns scale to apply to the specified bone based on current absolute scale of the transform and the specified 
        /// total scale
        /// </summary>
        /// <param name="boneTransform">Transform of the bone to retrieve the scale for</param>
        /// <param name="handScale">Total scale of the hand, presumably calculated from <see cref="Scale"/> and pose provider scale 
        /// (if applicable)</param>
        protected Vector3 GetBoneScale(
            Transform boneTransform,
            float handScale)
        {
            var boneScale = boneTransform.lossyScale;
            var scale = handScale * Vector3.Scale(
                boneTransform.localScale, 
                new Vector3(1f / boneScale.x, 1f / boneScale.y, 1f / boneScale.z));

            if (float.IsNaN(scale.x))
            {
                scale.x = 0;
            }

            if (float.IsNaN(scale.y))
            {
                scale.y = 0;
            }

            if (float.IsNaN(scale.z))
            {
                scale.z = 0;
            }

            return scale;
        }

        /// <summary>
        /// Returns position of the specified bone scaled by the specified scale factor
        /// </summary>
        /// <param name="boneUnscaledPosition"></param>
        /// <param name="rootPosition"></param>
        /// <param name="handScale"></param>
        /// <returns></returns>
        protected Vector3 GetBoneScaledPosition(Vector3 boneUnscaledPosition, Vector3 rootPosition, float handScale)
        {
            return rootPosition + handScale * (boneUnscaledPosition - rootPosition);
        }

        /// <summary>
        /// Returns scaled position of the bone based on current absolute position of the bone and the specified total scale of the bone's hand. 
        /// This method is best used for situations when there is no pose provider associated with the hand skeleton as otherwise there are
        /// more efficient ways to obtain final scaled position of a bone
        /// </summary>
        /// <param name="boneTransform">transform of the bone to retrieve the scaled position for</param>
        /// <param name="handScale">Total scale of the hand, presumably calculated from <see cref="Scale"/> and pose provider scale
        /// (if applicable)</param>
        protected Vector3 GetBoneScaledPosition(
            Transform boneTransform,
            float handScale)
        {
            var bonePosition = boneTransform.position;
            var unscaledPosition = transform.position + transform.rotation * transform.InverseTransformPoint(bonePosition);

            return handScale * unscaledPosition;
        }

        /// <summary>
        /// Updates bind poses of the bones in the hand skeleton based on current bone transforms. This method has effect only in 
        /// Unity editor while in edit mode, it will do nothing outside of it or in play mode
        /// </summary>
        protected void SetBoneBindPoses()
        {
#if !UNITY_EDITOR
            return;
#pragma warning disable CS0162 // Unreachable code detected
#endif
            if (!Application.IsPlaying(this))
            {
                var bones = Bones;
                
                for (var i = 0; i < bones.Count; i++)
                {
                    var bone = bones[i];

                    if (!bone.CaptureBindPose)
                    {
                        continue;
                    }

                    var parentBone = bone.ParentBone;

                    var bindPose = parentBone ? new Pose
                    (
                        position: parentBone.Transform.InverseTransformPoint(bone.Transform.position),
                        rotation: Quaternion.Inverse(parentBone.Transform.rotation) * bone.Transform.rotation
                    ) : new Pose
                    (
                        position: bone.Transform.position,
                        rotation: bone.Transform.rotation
                    );

                    bone.BindPose = bindPose;
                }
            }
#if !UNITY_EDITOR
#pragma warning restore CS0162 // Unreachable code detected
#endif
        }

        /// <summary>
        /// Updates hand skeleton pose based on pose provider poses and hand skeleton scale. Poses are updated by assigning 
        /// them to transform components directly. This method will also work while in edit mode, so any changes made in edit 
        /// mode that affect skeleton pose (e.g. changing its pose provider) will immediately be visible in the editor
        /// </summary>
        protected void UpdatePose()
        {
            if (!poseProvider)
            {
                SetBoneScalesAndScaledPositions(Scale);

                return;
            }

            var totalScale = scale;
            HandBoneKeyedReadOnlyCollection<Pose> bonePoses;

            if (applyPoseProviderScale)
            {
                totalScale *= poseProvider.Scale;
                bonePoses = poseProvider.GetBoneAbsoluteScaledPoses();
            }
            else
            {
                bonePoses = poseProvider.GetBoneAbsolutePoses();
            }

            var rootPose = bonePoses[0];

            transform.SetPositionAndRotation(rootPose.position, rootPose.rotation);
            transform.localScale = GetBoneScale(transform, totalScale);

            for (var i = 1; i < bones.Count; i++)
            {
                var bone = bones[i];
                var boneTransform = bone.Transform;

                bonePoses.GetItem(bone.BoneId, out var bonePose);

                boneTransform.SetPositionAndRotation(
                    position: rootPose.position + scale * (bonePose.position - rootPose.position),
                    rotation: bonePose.rotation);

                boneTransform.localScale = GetBoneScale(boneTransform, totalScale);
            }
        }

        /// <summary>
        /// Finalizes hand skeleton pose update by raising the  <see cref="OnPoseUpdated"/> event. Call this when hand skeleton is done 
        /// updating its poses and the poses are final for the current skeleton pose update cycle
        /// </summary>
        protected void FinalizePoseUpdate()
        {
            onPoseUpdated.Invoke();
        }

        /// <summary>
        /// Attaches the new hand bone component to the specified object
        /// </summary>
        /// <param name="boneObject"></param>
        /// <returns>Newly created bone component</returns>
        protected virtual HandBone CreateBone(GameObject boneObject)
        {
            if (!boneObject)
            {
                return null;
            }

            if (!boneObject.TryGetComponent<HandBone>(out var bone))
            {
                bone = boneObject.AddComponent<HandBone>();
            }

            return bone;
        }

        /// <summary>
        /// Called after the bone has been added to the hand skeleton, before the hand bone has been notified about it and before the bone added 
        /// event is sent. This method does not perform any logic by itself, it is used only as a callback for derived behaviours
        /// </summary>
        /// <param name="bone">The hand bone that was added</param>
        /// <param name="index">Index in the <see cref="Bones"/> collection at which the bone has been added</param>
        protected virtual void BoneAdded(HandBone bone, int index) { }

        /// <summary>
        /// Called after the bone has been removed from the hand skeleton, before the hand bone has been notified about it and before the bone removed 
        /// event is sent. This method does not perform any logic by itself, it is used only as a callback for derived behaviours
        /// </summary>
        /// <param name="bone">The removed hand bone</param>
        /// <param name="removedAtIndex">Index in the <see cref="Bones"/> collection from which the bone has been removed</param>
        protected virtual void BoneRemoved(HandBone bone, int removedAtIndex) { }

        public void AddBone(HandBoneId boneId, GameObject boneObject)
        {
            if (!boneObject)
            {
                throw new ArgumentNullException(nameof(boneObject));
            }

            if (isBeingDestroyed)
            {
                throw new ObjectDisposedException("Cannot add bone to the hand skeleton while it is being destroyed");
            }

            if (bones.Contains(boneId))
            {
                throw new ArgumentException("Hand skeleton already has a bone with the specified identity", nameof(boneId));
            }

            if (boneObject.scene != gameObject.scene)
            {
                throw new ArgumentException("Hand skeleton cannot have bones that are not in the same scene as the hand skeleton");
            }

            if (boneObject.TryGetComponent<HandBone>(out var bone))
            {
                if (bone.HandSkeleton)
                {
                    if (bone.HandSkeleton != this)
                    {
                        throw new ArgumentException("Specified bone is already contained in another hand skeleton", nameof(boneObject));
                    }
                    else
                    {
                        throw new ArgumentException("Specified bone is already contained in the hand skeleton", nameof(boneObject));
                    }
                }
            }
            else
            {
                bone = CreateBoneSafe(boneObject);
            }

            var boneIndex = (int)boneId;
#if UNITY_EDITOR
            editorBoneCollection.Bones[boneIndex] = boneObject;
#endif
            var insertIndex = bones.ModifiableCollection.Add(boneId, bone);
            var parentBoneId = FindParentBone(boneId);

            if (parentBoneId.HasValue)
            {
                parentBones[boneIndex] = (int)parentBoneId;
            }

            ResetParentBoneIndices(boneIndex + 1);

            try
            {
                bone.HandleAddedToHandSkeleton(this, boneId);
            }
            catch (ObjectDisposedException)
            {
#if UNITY_EDITOR
                editorBoneCollection.Bones[boneIndex] = null;
#endif
                bones.ModifiableCollection.RemoveAt(insertIndex);
                ResetParentBoneIndices(boneIndex);

                throw new ArgumentException("Cannot add bone which is being destroyed to the hand skeleton");
            }

            try
            {
                BoneAdded(bone, insertIndex);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (bone && bone.HandSkeleton == this)
            {
                bone.NotifyAddedToHandSkeleton();
            }

            if (!bone || bone.HandSkeleton != this)
            {
#if UNITY_EDITOR
                if (editorBoneCollection.Bones[boneIndex] == bone)
                {
                    editorBoneCollection.Bones[boneIndex] = null;
                }
#endif
                bones.ModifiableCollection.Remove(bone);
                ResetParentBoneIndices(boneIndex);
            }
            
            if (bones[boneId] == bone)
            {
                try
                {
                    onBoneAdded.Invoke(boneId);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            ObjectUtility.SetObjectDirty(this);
        }

        public bool RemoveBone(HandBoneId bone)
        {
            var removeAtIndex = bones.IndexOf(bone);

            if (removeAtIndex != -1)
            {
                RemoveBoneAt(removeAtIndex);

                return true;
            }

            return false;
        }

        public void RemoveBoneAt(int index)
        {
            if (index == 0)
            {
                throw new ArgumentException(
                    "Bone at index zero corresponds to root bone of the hand skeleton and it cannot be removed from it",
                    nameof(index));
            }

            var removeAtIndex = index;
            var bone = bones.ModifiableCollection.RemoveAt(removeAtIndex);
            var boneId = bone.BoneId;
            var boneIndex = (int)boneId;
#if UNITY_EDITOR
            editorBoneCollection.Bones[boneIndex] = null;
#endif
            var parentBone = bone.ParentBone;
            var parentBoneIndex = parentBone ? (int)parentBone.BoneId : -1;

            for (var id = boneId + 1; id <= HandSkeletonConfiguration.LastBoneId; id++)
            {
                ref var parentIndex = ref parentBones[(int)id];

                if (parentIndex == boneIndex)
                {
                    parentIndex = parentBoneIndex;
                }
            }

            InvokeAllBoneRemovedCallbacks(boneId, bone, index);

            ObjectUtility.SetObjectDirty(this);
        }

        /// <summary>
        /// Removes all bones from the hand skeleton, except the root bone
        /// </summary>
        public void ClearBones()
        {
            for (var i = bones.Count - 1; i > 0; i--)
            {
                RemoveBoneAt(i);
            }
        }

        /// <summary>
        /// Returns the bone that is currently in hand skeleton and also the closest ancestor in the hirerarchy for the specified bone
        /// </summary>
        /// <param name="bone">Bone to retrieve the closest ancestor contained in the skeleton</param>
        /// <returns></returns>
        public HandBoneId? GetParentBoneId(HandBoneId bone)
        {
            var parentBoneId = parentBones[(int)bone];

            return parentBoneId != -1 ? (HandBoneId)parentBoneId : default(HandBoneId?);
        }

        /// <summary>
        /// Returns the bone that is currently in hand skeleton and also the closest ancestor in the hirerarchy for the specified bone
        /// </summary>
        /// <param name="boneId">Bone to retrieve the closest ancestor contained in the skeleton</param>
        /// <param name="bone">The resulting parent bone</param>
        /// <returns></returns>
        public bool TryGetParentBone(HandBoneId boneId, out HandBone bone)
        {
            var parentBoneId = parentBones[(int)boneId];

            if (parentBoneId != -1)
            {
                bone = bones[(HandBoneId)parentBoneId];

                return true;
            }

            bone = null;

            return false;
        }

        /// <summary>
        /// Returns the bone that is currently in hand skeleton and also the closest ancestor in the hirerarchy for the specified bone
        /// </summary>
        /// <param name="bone">Bone to retrieve the closest ancestor contained in the skeleton</param>
        /// <returns></returns>
        public HandBone GetParentBone(HandBoneId bone)
        {
            var parentBoneId = parentBones[(int)bone];

            return parentBoneId != -1 ? bones[(HandBoneId)parentBoneId] : null;
        }
    }
}
