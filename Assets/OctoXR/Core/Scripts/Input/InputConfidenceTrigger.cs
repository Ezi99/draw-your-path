using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.Input
{
    /// <summary>
    /// Component that serves as trigger for logic that needs to be executed based on confidence level being low or high
    /// based on a certain threshold value
    /// </summary>
    public class InputConfidenceTrigger : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Input data provider to read the confidence level from")]
        private InputDataProvider inputDataProvider;
        /// <summary>
        /// Input data provider to read the confidence level from
        /// </summary>
        public InputDataProvider InputDataProvider
        {
            get => inputDataProvider;
            set
            {
#if UNITY_EDITOR
                if (inputDataProvider == value)
                {
                    return;
                }
#endif
                inputDataProvider = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        private float bufferedConfidenceThreshold;

        [SerializeField]
        [Tooltip("The value of input confidence level for triggering confidence low/high logic. Confidence level is considered low " +
            "when it is less than this value")]
        [Range(0, 1)]
        private float confidenceThreshold = 0.5f;
        /// <summary>
        /// The value of input confidence level for triggering confidence low/high logic. Confidence level is considered low
        /// when it is less than this value
        /// </summary>
        public float ConfidenceThreshold
        {
            get => confidenceThreshold;
            set
            {
                if (isNotifyingConfidenceLevelChanged)
                {
                    bufferedConfidenceThreshold = value;
                }
                else
                {
#if UNITY_EDITOR
                    if (confidenceThreshold == value)
                    {
                        return;
                    }
#endif
                    confidenceThreshold = value;

                    ObjectUtility.SetObjectDirty(this);
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        private bool isConfidenceLowTriggered;
        /// <summary>
        /// Has low input confidence been triggered by the trigger since the last time the input confidence was evaluated. 
        /// This value is not the opposite of <see cref="IsConfidenceHighTriggered"/>; they can both be false in some circumstances,
        /// but never both be true
        /// </summary>
        public bool IsConfidenceLowTriggered => isConfidenceLowTriggered;

        [SerializeField]
        [HideInInspector]
        private bool isConfidenceHighTriggered;
        /// <summary>
        /// Has high input confidence been triggered by the trigger since the last time the input confidence was evaluated. 
        /// This value is not the opposite of <see cref="IsConfidenceLowTriggered"/>; they can both be false in some circumstances,
        /// but never both be true
        /// </summary>
        public bool IsConfidenceHighTriggered => isConfidenceHighTriggered;

        private bool isNotifyingConfidenceLevelChanged;

        [SerializeField]
        private UnityEvent onConfidenceLow = new UnityEvent();
        [SerializeField]
        private UnityEvent onConfidenceHigh = new UnityEvent();

        public UnityEvent OnConfidenceLow => onConfidenceLow;
        public UnityEvent OnConfidenceHigh => onConfidenceHigh;

        [SerializeField]
        [Tooltip("When should the trigger re-evaluate input confidence level and trigger the appropriate state")]
        private InputConfidenceTriggerEvaluationRun triggerEvaluationRun = InputConfidenceTriggerEvaluationRun.Update;
        /// <summary>
        /// When should the trigger re-evaluate input confidence level and trigger the appropriate state
        /// </summary>
        public InputConfidenceTriggerEvaluationRun TriggerEvaluationRun
        {
            get => triggerEvaluationRun;
            set
            {
#if UNITY_EDITOR
                if (triggerEvaluationRun == value)
                {
                    return;
                }
#endif
                triggerEvaluationRun = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        /// <summary>
        /// Callback method for handling input confidence low trigger before the <see cref="OnConfidenceLow"/> event is raised
        /// </summary>
        protected virtual void HandleConfidenceLow() { }

        /// <summary>
        /// Callback method for handling input confidence high trigger before the <see cref="OnConfidenceHigh"/> event is raised.
        /// </summary>
        protected virtual void HandleConfidenceHigh() { }

        /// <summary>
        /// Gets called every frame while input confidence is low. Depending on the <see cref="TriggerEvaluationRun"/> this may
        /// get called from <see cref="FixedUpdate"/> or <see cref="Update"/> or both. Used as callback only, does not perform any logic
        /// </summary>
        protected virtual void UpdateConfidenceLow() { }

        /// <summary>
        /// Gets called every frame while input confidence is high. Depending on the <see cref="TriggerEvaluationRun"/> this may
        /// get called from <see cref="FixedUpdate"/> or <see cref="Update"/> or both. Used as callback only, does not perform any logic
        /// </summary>
        protected virtual void UpdateConfidenceHigh() { }

        private void FixedUpdate()
        {
            if (triggerEvaluationRun == InputConfidenceTriggerEvaluationRun.Update)
            {
                return;
            }

            UpdateConfidenceTrigger();
        }

        private void Update()
        {
            if (triggerEvaluationRun == InputConfidenceTriggerEvaluationRun.FixedUpdate)
            {
                return;
            }

            UpdateConfidenceTrigger();
        }

        /// <summary>
        /// Checks the confidence level of current input data provider if there is one assigned and raises the appropriate callbacks
        /// if confidence level has crossed the threshold value
        /// </summary>
        private void UpdateConfidenceTrigger()
        {
            var confidence = inputDataProvider ? inputDataProvider.Confidence : 0f;

            if (confidence < confidenceThreshold)
            {
                if (!isConfidenceLowTriggered)
                {
                    isConfidenceLowTriggered = true;
                    isConfidenceHighTriggered = false;

                    isNotifyingConfidenceLevelChanged = true;
                    bufferedConfidenceThreshold = confidenceThreshold;

                    try
                    {
                        HandleConfidenceLow();
                        onConfidenceLow.Invoke();
                    }
                    finally
                    {
                        isNotifyingConfidenceLevelChanged = false;
                        confidenceThreshold = bufferedConfidenceThreshold;

                        ObjectUtility.SetObjectDirty(this);
                    }
                }

                UpdateConfidenceLow();
            }
            else 
            {
                if (!isConfidenceHighTriggered)
                {
                    isConfidenceLowTriggered = false;
                    isConfidenceHighTriggered = true;

                    isNotifyingConfidenceLevelChanged = true;
                    bufferedConfidenceThreshold = confidenceThreshold;

                    try
                    {
                        HandleConfidenceHigh();
                        onConfidenceHigh.Invoke();
                    }
                    finally
                    {
                        isNotifyingConfidenceLevelChanged = false;
                        confidenceThreshold = bufferedConfidenceThreshold;

                        ObjectUtility.SetObjectDirty(this);
                    }
                }

                UpdateConfidenceHigh();
            }
        }
    }
}
