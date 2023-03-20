using OctoXR.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.Input
{
    [Serializable]
    public struct HandFingerState
    {
        [SerializeField]
        private bool isPinching;
        public bool IsPinching { get => isPinching; set => isPinching = value; }

        [SerializeField]
        [Range(0, 1)]
        private float pinchStrength;
        public float PinchStrength { get => pinchStrength; set => pinchStrength = Mathf.Clamp01(value); }

        [SerializeField]
        [Range(0, 1)]
        private float confidence;
        public float Confidence { get => confidence; set => confidence = Mathf.Clamp01(value); }   

        public HandFingerState(bool isPinching, float pinchStrength, float confidence)
        {
            this.isPinching = isPinching;
            this.pinchStrength = Mathf.Clamp01(pinchStrength);
            this.confidence = Mathf.Clamp01(confidence);
        }
    }

    [Serializable]
    public class HandFingerStateCollection : HandFingerKeyedReadOnlyCollection<HandFingerState>
    {
        public HandFingerStateCollection() { }
        public HandFingerStateCollection(HandFingerState[] fingerStates) : base(fingerStates) { }

        public void GetFingerState(HandFinger finger, out HandFingerState fingerState) => fingerState = items[(int)finger];
        public void GetFingerState(int index, out HandFingerState fingerState) => fingerState = items[index];
    }

    public abstract class HandInputDataProvider : InputDataProvider
    {
        [Serializable]
        private class InternalHandFingerStateCollection : HandFingerStateCollection
        {
            public new ref HandFingerState this[HandFinger finger] => ref items[(int)finger];
            public new ref HandFingerState this[int index] => ref items[index];
        }

        [SerializeField]
        [Tooltip("Input state of hand fingers")]
        [HandFingerPropertyDrawOptions(
            ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways,
            IsCustomCollection = true,
            CustomCollectionBackingArrayOrListFieldPath = "items",
            CustomCollectionItemsReadOnly = ReadOnlyPropertyDrawOptions.NotReadOnly)]
        private InternalHandFingerStateCollection fingers = new InternalHandFingerStateCollection();
        /// <summary>
        /// Input state of hand fingers
        /// </summary>
        public HandFingerStateCollection Fingers => fingers;

        [SerializeField]
        [Tooltip("Is some uderlying system defined gesture currently in progress")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true, SetValueViaPropertyOrMethodName = nameof(SetIsSystemGestureInProgress))]
        private bool isSystemGestureInProgress;
        /// <summary>
        /// Is some uderlying system defined gesture currently in progress
        /// </summary>
        public bool IsSystemGestureInProgress => isSystemGestureInProgress;

        [SerializeField]
        private UnityEvent onSystemGestureStart = new UnityEvent();
        [SerializeField]
        private UnityEvent onSystemGestureEnd = new UnityEvent();

        public UnityEvent OnSystemGestureStart => onSystemGestureStart;
        public UnityEvent OnSystemGestureEnd => onSystemGestureEnd;

        protected void SetFingerState(HandFinger finger, HandFingerState fingerState)
        {
            SetFingerState((int)finger, fingerState);
        }

        protected void SetFingerState(int index, HandFingerState fingerState)
        {
            if (IsTracking)
            {
                ref var thisFingerState = ref fingers[index];
#if UNITY_EDITOR
                var setDirty = thisFingerState.IsPinching != fingerState.IsPinching ||
                    thisFingerState.PinchStrength != fingerState.PinchStrength ||
                    thisFingerState.Confidence != fingerState.Confidence;
#endif
                thisFingerState = fingerState;
#if UNITY_EDITOR
                if (setDirty)
                {
                    ObjectUtility.SetObjectDirty(this);
                }
#endif
            }
        }

        protected void SetIsSystemGestureInProgress(bool isSystemGestureInProgress)
        {
            if (isSystemGestureInProgress != this.isSystemGestureInProgress)
            {
                if (isSystemGestureInProgress)
                {
                    if (!IsTracking)
                    {
                        return;
                    }

                    this.isSystemGestureInProgress = true;

                    onSystemGestureStart.Invoke();
                }
                else
                {
                    this.isSystemGestureInProgress = false;

                    onSystemGestureEnd.Invoke();
                }

                ObjectUtility.SetObjectDirty(this);
            }
        }

        protected override void TrackingLost()
        {
            base.TrackingLost();

            for (var i = 0; i < fingers.Count; i++)
            {
                fingers[i] = default;
            }

            SetIsSystemGestureInProgress(false);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsTracking)
            {
                for (var i = 0; i < fingers.Count; i++)
                {
                    fingers[i] = default;
                }
            }
        }
    }
}
