using System;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.Input
{
    [ExecuteAlways]
    public abstract class InputDataProvider : HandSkeletonPoseProvider
    {
        [SerializeField]
        [Tooltip("Transform to use as input data root transform. It affects all world space pose data")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private Transform inputRoot;
        /// <summary>
        /// Transform to use as input data root transform. It affects all world space pose data
        /// </summary>
        public Transform InputRoot 
        { 
            get => inputRoot;
            set
            {
                if (inputRoot != value)
                {
                    var previousRoot = inputRoot;

                    inputRoot = value;

                    ObjectUtility.SetObjectDirty(this);

                    InputRootChanged(previousRoot);
                }
            }
        }

        [SerializeField]
        [Tooltip("Transform whose local position and local rotation specify the offset to apply to the final root pose that will " +
            "be provided by the pose provider")]
        private Transform rootPoseOffset;
        /// <summary>
        /// Transform whose local position and local rotation specify the offset to apply to the final root pose that will be provided
        /// by the pose provider
        /// </summary>
        public Transform RootPoseOffset
        {
            get => rootPoseOffset;
            set
            {
#if UNITY_EDITOR
                if (rootPoseOffset == value)
                {
                    return;
                }
#endif
                rootPoseOffset = value;
                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Is input source being tracked currently?")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true, SetValueViaPropertyOrMethodName = nameof(SetIsTracking))]
        private bool isTracking;
        /// <summary>
        /// Is input source being tracked currently?
        /// </summary>
        public bool IsTracking => isTracking;

        [SerializeField]
        private UnityEvent onTrackingStart = new UnityEvent();
        [SerializeField]
        private UnityEvent onTrackingLost = new UnityEvent();

        public UnityEvent OnTrackingStart => onTrackingStart;
        public UnityEvent OnTrackingLost => onTrackingLost;

        [SerializeField]
        [HideInInspector]
        private bool isOnTrackingStartSent;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Confidence level of input data (0 to 1)")]
        private float confidence;
        /// <summary>
        /// Confidence level of input data (0 to 1)
        /// </summary>
        public float Confidence
        {
            get => confidence;
            protected set
            {
#if UNITY_EDITOR
                var previousValue = confidence;
#endif
                if (isTracking)
                {
                    confidence = Mathf.Clamp01(value);
                }
                else
                {
                    confidence = 0f;
                }
#if UNITY_EDITOR
                if (confidence != previousValue)
                {
                    ObjectUtility.SetObjectDirty(this);
                }
#endif
            }
        }

        protected Pose GetRootPose(Pose baseRootPose)
        {
            if (inputRoot)
            {
                baseRootPose = baseRootPose.TransformedBy(inputRoot);
            }

            if (rootPoseOffset)
            {
                var offsetPose = new Pose(rootPoseOffset.localPosition, rootPoseOffset.localRotation);

                baseRootPose = offsetPose.TransformedBy(in baseRootPose);
            }

            return baseRootPose;
        }

        protected void SetIsTracking(bool isTracking)
        {
            if (isTracking != this.isTracking)
            {
                if (isTracking)
                {
                    if (!isActiveAndEnabled)
                    {
                        return;
                    }

                    this.isTracking = true;

                    TrackingStart();

                    if (!this.isTracking)
                    {
                        return;
                    }

                    if (!isOnTrackingStartSent)
                    {
                        isOnTrackingStartSent = true;
                        onTrackingStart.Invoke();
                    }
                }
                else
                {
                    this.isTracking = false;
                    confidence = 0f;

                    TrackingLost();

                    if (this.isTracking)
                    {
                        return;
                    }

                    if (isOnTrackingStartSent)
                    {
                        isOnTrackingStartSent = false;
                        onTrackingLost.Invoke();
                    }
                }

                ObjectUtility.SetObjectDirty(this);
            }
        }

        protected virtual void InputRootChanged(Transform previousRoot) { }

        protected virtual void TrackingStart() { }

        protected virtual void TrackingLost() { }

        protected virtual void OnValidate()
        {
            if (!isTracking)
            {
                confidence = 0f;
            }
        }

        protected virtual void OnDisable() => SetIsTracking(false);
    }
}
