using OctoXR.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR
{
    /// <summary>
    /// Component that acts as a filter for hand skeleton poses obtained from a source pose provider, but itself is also a 
    /// pose provider so it can provide filtered poses to a hand skeleton or to another filter
    /// </summary>
    [ExecuteAlways]
    public abstract class HandSkeletonPoseFilter : HandSkeletonPoseProvider
    {
        [SerializeField]
        [Tooltip("Pose provider that serves as the source of hand skeleton poses that need to be filtered")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private HandSkeletonPoseProvider sourcePoseProvider;
        /// <summary>
        /// Pose provider that serves as the source of hand skeleton poses that need to be filtered
        /// </summary>
        public HandSkeletonPoseProvider SourcePoseProvider
        {
            get => sourcePoseProvider;
            set
            {
                if (sourcePoseProvider == value)
                {
                    return;
                }

                if (value == this)
                {
                    throw new ArgumentException("Hand skeleton pose filter cannot have itself as the source of hand skeleton pose");
                }

                if (value && value.gameObject.scene != gameObject.scene)
                {
                    throw new ArgumentException("Source pose provider of the hand skeleton pose filter must be in the same scene as " +
                        "the hand skeleton pose filter");
                }

                if (sourcePoseProvider)
                {
                    UnsubscribeSourcePoseDataUpdatedHandler();
                    doNotUpdatePoseProviderState = true;
                }

                sourcePoseProvider = value;

                if (sourcePoseProvider)
                {
                    SubscribeSourcePoseDataUpdatedHandler();
                    doNotUpdatePoseProviderState = false;
                }

                UpdateHandType();

                ObjectUtility.SetObjectDirty(this);
            }
        }

        private void UpdateHandType()
        {
            handType = sourcePoseProvider ? sourcePoseProvider.GetHandType() : handType;
        }

        [SerializeField]
        [Tooltip("Is this the pose filter for a left or right hand. Note that this can be changed only if there is no source pose provider " +
            "assigned for the hand skeleton pose filter")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private HandType handType;
        /// <summary>
        /// Is this the pose filter for a left or right hand. Note that this can be changed only if there is no source pose provider
        /// assigned for the hand skeleton pose filter
        /// </summary>
        public HandType HandType
        {
            get => sourcePoseProvider ? sourcePoseProvider.GetHandType() : handType;
            set
            {
                if (handType == value)
                {
                    return;
                }

                if (sourcePoseProvider)
                {
                    const string message = "Hand type of a hand skeleton pose filter cannot be changed when the pose filter " +
                        "has source pose provider assigned";

                    throw new InvalidOperationException(message);
                }

                handType = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        private readonly HandBoneKeyedCollection<Pose> filteredBoneRelativePoses = new HandBoneKeyedCollection<Pose>();

        private bool doNotUpdatePoseProviderState;
        private UnityAction sourcePoseDataUpdatedHandler;

        public sealed override HandType GetHandType() => handType;

        protected virtual void Reset()
        {
            var scene = gameObject.scene;
            var gameObjects = ListPool<List<GameObject>>.Get();
            var poseProviders = ListPool<List<HandSkeletonPoseProvider>>.Get();

            scene.GetRootGameObjects(gameObjects);

            if (sourcePoseDataUpdatedHandler == null)
            {
                sourcePoseDataUpdatedHandler = OnSourcePoseDataUpdated;
            }

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
            if (sourcePoseProvider)
            {
                SubscribeSourcePoseDataUpdatedHandler();
                doNotUpdatePoseProviderState = false;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Just to have enable toggle box in editor, derived behaviours can just hide this
        /// </summary>
        private void OnEnable() { }
#endif
        protected virtual void OnDestroy()
        {
            if (sourcePoseProvider)
            {
                UnsubscribeSourcePoseDataUpdatedHandler();
                doNotUpdatePoseProviderState = true;
            }
        }

        private void SubscribeSourcePoseDataUpdatedHandler()
        {
            UnsubscribeSourcePoseDataUpdatedHandler();

            sourcePoseProvider.OnPoseDataUpdated.AddListener(sourcePoseDataUpdatedHandler);
        }

        private void UnsubscribeSourcePoseDataUpdatedHandler()
        {
            if (sourcePoseDataUpdatedHandler == null)
            {
                sourcePoseDataUpdatedHandler = OnSourcePoseDataUpdated;
            }

            sourcePoseProvider.OnPoseDataUpdated.RemoveListener(sourcePoseDataUpdatedHandler);
        }

        private void OnSourcePoseDataUpdated() => doNotUpdatePoseProviderState = false;

        protected override void OnGetBonePoses()
        {
            if (sourcePoseProvider && gameObject.activeInHierarchy)
            {
                if (doNotUpdatePoseProviderState)
                {
                    Scale = sourcePoseProvider.Scale;

                    return;
                }

                UpdatePoseProviderState();
            }

            doNotUpdatePoseProviderState = true;
        }

        private void UpdatePoseProviderState()
        {
            Scale = sourcePoseProvider.Scale;

            if (enabled)
            {
                GetFilteredBoneRelativePoses(sourcePoseProvider, filteredBoneRelativePoses);
                SetBoneRelativePoses(filteredBoneRelativePoses);
            }
            else
            {
                SetBoneRelativePoses(sourcePoseProvider.GetBoneRelativePoses());
            }
        }

        protected abstract void GetFilteredBoneRelativePoses(
            HandSkeletonPoseProvider sourcePoseProvider,
            HandBoneKeyedCollection<Pose> filteredBoneRelativePoses);
    }
}