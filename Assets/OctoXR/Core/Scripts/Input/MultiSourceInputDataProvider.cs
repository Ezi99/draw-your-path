using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.Input
{
    /// <summary>
    /// Input data provider that is able to provide input data from multiple input sources. It keeps a list of 
    /// <see cref="InputDataProvider"/>s and switches between them based on their order in the list and whether
    /// they are tracking their input sources at any given time. When the first input data provider from that list
    /// is tracking its input source, then input data from that input data provider will be provided by the
    /// <see cref="MultiSourceInputDataProvider"/> so long as the tracking is not lost by the active input data
    /// provider. If the tracking is lost by the active input data provider, new input data provider is chosen to
    /// be the source of input data with the same criteria applied again
    /// </summary>
    [DefaultExecutionOrder(-85)]
    public class MultiSourceInputDataProvider : InputDataProvider
    {
        [SerializeField]
        [Tooltip("Input data provider that is current source of input data provided by the multi-source input data provider")]
        [PropertyDrawOptions(ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways)]
        private InputDataProvider current;
        /// <summary>
        /// Input data provider that is current source of input data provided by the multi-source input data provider
        /// </summary>
        public InputDataProvider Current => current;

        [SerializeField]
        [Tooltip("Sidedness of the hand for which the multi-source input data provider is providing input data by default")]
        private HandType defaultHandType;
        /// <summary>
        /// Sidedness of the hand for which the multi-source input data provider is providing input data by default
        /// </summary>
        public HandType DefaultHandType
        {
            get => defaultHandType;
            set
            {
#if UNITY_EDITOR
                if (defaultHandType == value)
                {
                    return;
                }
#endif
                defaultHandType = value;
                ObjectUtility.SetObjectDirty(this);
            }
        }

        public override HandType GetHandType() => current ? current.GetHandType() : defaultHandType;

        [SerializeField]
        [Tooltip("Specifies whether the multi-source input provider logic should be executed in FixedUpdate instead of Update by default")]
        private bool runInFixedUpdate;
        /// <summary>
        /// Specifies whether the multi-source input provider logic should be executed in FixedUpdate instead of Update by default
        /// </summary>
        public bool RunInFixedUpdate
        {
            get => runInFixedUpdate;
            set
            {
#if UNITY_EDITOR
                if (runInFixedUpdate == value)
                {
                    return;
                }
#endif
                runInFixedUpdate = value;
                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("List of input data providers out of which the multi-source input data provider selects one to use as source of input " +
            "data to provide based on their order in the list and whether they are tracking their input sources at any given time")]
        private List<InputDataProvider> sourceInputDataProviders = new List<InputDataProvider>();
        /// <summary>
        /// List of input data providers out of which the multi-source input data provider selects one to use as source of input data 
        /// to provide based on their order in the list and whether they are tracking their input sources at any given time
        /// </summary>
        public List<InputDataProvider> SourceInputDataProviders => sourceInputDataProviders;

        private readonly Pose[] bonePoses = new Pose[HandSkeletonConfiguration.BoneCount];

        protected override void OnValidate()
        {
            base.OnValidate();

            for (var i = 0; i < sourceInputDataProviders.Count; ++i)
            {
                var inputDataProvider = sourceInputDataProviders[i];
#if !UNITY_EDITOR
                if (!inputDataProvider)
                {
                    sourceInputDataProviders.RemoveAt(i--);

                    continue;
                }
#endif
                if (inputDataProvider == this)
                {
                    sourceInputDataProviders.RemoveAt(i--);
                }
            }

            if (IsTracking && current)
            {
                Confidence = current.Confidence;
                Scale = current.Scale;
            }
            else
            {
                Confidence = 0;
            }
        }

        protected override void TrackingStart()
        {
            base.TrackingStart();

            if (!current)
            {
                SetIsTracking(false);
            }
        }

        protected override void TrackingLost()
        {
            base.TrackingLost();

            if (!isActiveAndEnabled)
            {
                return;
            }

            if (current)
            {
                SetIsTracking(true);
                Confidence = current.Confidence;
            }
        }

        protected override void OnDisable()
        {
            current = null;

            base.OnDisable();
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (!Application.IsPlaying(this))
            {
                UpdateInputAndPoseState();

                return;
            }
#endif
            if (!runInFixedUpdate)
            {
                UpdateInputAndPoseState();
            }
        }

        protected virtual void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.IsPlaying(this))
            {
                return;
            }
#endif
            if (runInFixedUpdate)
            {
                UpdateInputAndPoseState();
            }
        }

        private void UpdateInputAndPoseState()
        {
            var current = this.current;
#if UNITY_EDITOR
            var setDirty = false;
#endif
            if (!current || !current.IsTracking)
            {
                this.current = null;

                for (var i = 0; i < sourceInputDataProviders.Count; ++i)
                {
                    var inputDataProvider = sourceInputDataProviders[i];

                    if (!inputDataProvider)
                    {
                        continue;
                    }

                    if (inputDataProvider == this)
                    {
                        sourceInputDataProviders.RemoveAt(i--);
#if UNITY_EDITOR
                        setDirty = true;
#endif
                        continue;
                    }

                    if (inputDataProvider.IsTracking)
                    {
                        this.current = inputDataProvider;
                        break;
                    }
                }
            }
#if UNITY_EDITOR
            if (setDirty || this.current != current)
            {
                ObjectUtility.SetObjectDirty(this);
            }
#endif
            if (this.current)
            {
                try
                {
                    SetIsTracking(true);
                }
                finally
                {
                    if (this.current)
                    {
                        Confidence = this.current.Confidence;

                        this.current.GetBoneRelativePoses().CopyTo(bonePoses, 0);

                        ref var rootPose = ref bonePoses[0];

                        rootPose = GetRootPose(rootPose);

                        Scale = this.current.Scale;
                        SetBoneRelativePoses(bonePoses);
                    }
                    else
                    {
                        SetIsTracking(false);
                    }
                }
            }
            else
            {
                SetIsTracking(false);
            }
        }
    }
}