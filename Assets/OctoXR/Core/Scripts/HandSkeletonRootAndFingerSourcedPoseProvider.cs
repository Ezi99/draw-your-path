using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR
{
    public class HandSkeletonRootAndFingerSourcedPoseProvider : HandSkeletonPoseProvider
    {
        [SerializeField]
        [Tooltip("Pose provider that is used as the source of hand root pose that will be provided by the pose provider")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private HandSkeletonPoseProvider rootPoseProvider;
        /// <summary>
        /// Pose provider that is used as the source of hand root pose that will be provided by the pose provider
        /// </summary>
        public HandSkeletonPoseProvider RootPoseProvider
        {
            get => rootPoseProvider;
            set => SetSourcePoseProvider(ref rootPoseProvider, value);
        }

        [SerializeField]
        [Tooltip("Pose provider that is used as the source of poses of hand fingers that will be provided by the pose provider")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private HandSkeletonPoseProvider fingerPoseProvider;
        /// <summary>
        /// Pose provider that is used as the source of poses of hand fingers that will be provided by the pose provider
        /// </summary>
        public HandSkeletonPoseProvider FingerPoseProvider
        {
            get => fingerPoseProvider;
            set => SetSourcePoseProvider(ref fingerPoseProvider, value);
        }

        [SerializeField]
        [Tooltip("Default handedness for which the pose provider provides poses. This is used only if the root pose provider is " +
            "not assigned, otherwise the hand for which the poses are provided is the same as the one from the assigned root pose " +
            "provider")]
        private HandType defaultHandType;
        /// <summary>
        /// Default handedness for which the pose provider provides poses. This is used only if the root pose provider is not
        /// assigned, otherwise the hand for which the poses are provided is the same as the one from the assigned root pose provider
        /// </summary>
        public HandType DefaultHandType
        {
            get => defaultHandType;
            set => defaultHandType = value;
        }

        /// <summary>
        /// Scale of the hand skeleton
        /// </summary>
        public new float Scale
        {
            get => base.Scale;
            set => base.Scale = value;
        }

        public override HandType GetHandType() => rootPoseProvider != null ? rootPoseProvider.GetHandType() : defaultHandType;

        private bool doNotUpdatePoseProviderState;
        private UnityAction sourcePoseDataUpdatedHandler;

        private static readonly Pose[] poses = new Pose[HandSkeletonConfiguration.BoneCount];

        protected virtual void Reset()
        {
            var scene = gameObject.scene;
            var gameObjects = ListPool<List<GameObject>>.Get();
            var poseProviders = ListPool<List<HandSkeletonPoseProvider>>.Get();

            scene.GetRootGameObjects(gameObjects);

            sourcePoseDataUpdatedHandler ??= OnSourcePoseDataUpdated;

            for (var i = 0; i < gameObjects.Count; i++)
            {
                var gameObject = gameObjects[i];

                gameObject.GetComponentsInChildren(poseProviders);

                for (var j = 0; j < poseProviders.Count; ++j)
                {
                    var poseProvider = poseProviders[j];

                    poseProvider.OnPoseDataUpdated.RemoveListener(sourcePoseDataUpdatedHandler);
                }
            }

            ListPool<List<HandSkeletonPoseProvider>>.Recycle(poseProviders);
            ListPool<List<GameObject>>.Recycle(gameObjects);
        }

        protected virtual void OnValidate()
        {
            if (rootPoseProvider != null)
            {
                SubscribeSourcePoseDataUpdatedHandler(rootPoseProvider);
                doNotUpdatePoseProviderState = false;
            }

            if (fingerPoseProvider != null)
            {
                SubscribeSourcePoseDataUpdatedHandler(fingerPoseProvider);
                doNotUpdatePoseProviderState = false;
            }
        }

        protected virtual void OnDestroy()
        {
            if (rootPoseProvider != null)
            {
                UnsubscribeSourcePoseDataUpdatedHandler(rootPoseProvider);
            }

            if (fingerPoseProvider != null)
            {
                UnsubscribeSourcePoseDataUpdatedHandler(fingerPoseProvider);
            }

            doNotUpdatePoseProviderState = true;
        }

        private void SetSourcePoseProvider(ref HandSkeletonPoseProvider sourcePoseProvider, HandSkeletonPoseProvider setValue)
        {
            if (sourcePoseProvider == setValue)
            {
                return;
            }

            if (setValue == this)
            {
                throw new ArgumentException("Hand skeleton root and finger pose provider cannot have itself as the source of " +
                    "hand skeleton root pose or hand skeleton finger poses");
            }

            if (setValue != null && setValue.gameObject.scene != gameObject.scene)
            {
                throw new ArgumentException("Pose provider that is used as the source of hand root pose or the source of hand " +
                    "finger poses provided by the hand skeleton root and finger pose provider must be in the same scene as the " +
                    "hand skeleton root and finger pose provider");
            }

            if (sourcePoseProvider != null)
            {
                UnsubscribeSourcePoseDataUpdatedHandler(sourcePoseProvider);
            }

            sourcePoseProvider = setValue;

            if (sourcePoseProvider != null)
            {
                SubscribeSourcePoseDataUpdatedHandler(sourcePoseProvider);
                doNotUpdatePoseProviderState = false;
            }

            ObjectUtility.SetObjectDirty(this);
        }

        private void SubscribeSourcePoseDataUpdatedHandler(HandSkeletonPoseProvider sourcePoseProvider)
        {
            UnsubscribeSourcePoseDataUpdatedHandler(sourcePoseProvider);

            sourcePoseProvider.OnPoseDataUpdated.AddListener(sourcePoseDataUpdatedHandler);
        }

        private void UnsubscribeSourcePoseDataUpdatedHandler(HandSkeletonPoseProvider sourcePoseProvider)
        {
            sourcePoseDataUpdatedHandler ??= OnSourcePoseDataUpdated;
            sourcePoseProvider.OnPoseDataUpdated.RemoveListener(sourcePoseDataUpdatedHandler);
        }

        private void OnSourcePoseDataUpdated() => doNotUpdatePoseProviderState = false;

        protected override void OnGetBonePoses()
        {
            if (gameObject.activeInHierarchy && 
                !doNotUpdatePoseProviderState)
            {
                UpdatePoseProviderState();
            }

            doNotUpdatePoseProviderState = true;
        }

        private void UpdatePoseProviderState()
        {
            if (rootPoseProvider != null)
            {
                poses[0] = rootPoseProvider.GetBoneRelativePoses()[0];
            }

            if (fingerPoseProvider != null)
            {
                var fingerPoses = fingerPoseProvider.GetBoneRelativePoses();

                for (var i = 1; i < poses.Length; i++)
                {
                    poses[i] = fingerPoses[i];
                }
            }

            SetBoneRelativePoses(poses);
        }
    }
}
