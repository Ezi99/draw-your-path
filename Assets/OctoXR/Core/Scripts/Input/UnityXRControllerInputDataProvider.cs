using OctoXR.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace OctoXR.Input
{
    public enum XRControllerButton
    {
        Primary,
        Secondary,
        Trigger,
        Grip,
        Primary2DAxis,
        Secondary2DAxis
    }

    [Serializable]
    public class XRContollerButtonKeyedCollection<T> : Collections.SerializableByReference.ReadOnlyCollection<T>
        where T : class
    {
        private new const int count = (int)XRControllerButton.Secondary2DAxis + 1;

        public XRContollerButtonKeyedCollection(T[] items) : base(items) 
        {
            if (items.Length != count)
            {
                base.items = new T[count];

                throw new ArgumentException("Number of items in the specified array must be equal to the number of all the " +
                    $"values enumerated in {typeof(XRControllerButton)} enumeration ({count})", nameof(items));
            }
        }

        public T this[XRControllerButton button] => items[(int)button];
    }

    [Serializable]
    public class XRContollerButtonCollection : XRContollerButtonKeyedCollection<XRControllerButtonState>
    {
        [SerializeReference]
        [HideInInspector]
        private XRControllerBinaryButton primary;
        public XRControllerBinaryButton Primary => primary;

        [SerializeReference]
        [HideInInspector]
        private XRControllerBinaryButton secondary;
        public XRControllerBinaryButton Secondary => secondary;

        [SerializeReference]
        [HideInInspector]
        private XRControllerLerpButton trigger;
        public XRControllerLerpButton Trigger => trigger;

        [SerializeReference]
        [HideInInspector]
        private XRControllerLerpButton grip;
        public XRControllerLerpButton Grip => grip;

        [SerializeReference]
        [HideInInspector]
        private XRController2DAxis primary2DAxis;
        public XRController2DAxis Primary2DAxis => primary2DAxis;

        [SerializeReference]
        [HideInInspector]
        private XRController2DAxis secondary2DAxis;
        public XRController2DAxis Secondary2DAxis => secondary2DAxis;

        public XRContollerButtonCollection() : base(
            new XRControllerButtonState[]
            {
                new XRControllerBinaryButton(false),
                new XRControllerBinaryButton(true),
                new XRControllerLerpButton(false),
                new XRControllerLerpButton(true),
                new XRController2DAxis(false),
                new XRController2DAxis(true),
            })
        {
            primary = (XRControllerBinaryButton)items[0];
            secondary = (XRControllerBinaryButton)items[1];
            trigger = (XRControllerLerpButton)items[2];
            grip = (XRControllerLerpButton)items[3];
            primary2DAxis = (XRController2DAxis)items[4];
            secondary2DAxis = (XRController2DAxis)items[5];
        }
    }

    [DefaultExecutionOrder(-90)]
    public class UnityXRControllerInputDataProvider : InputDataProvider
    {
        [SerializeField]
        [Tooltip("Specifies for which hand is input provided by the input data provider")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true)]
        private HandType handType;
        /// <summary>
        /// Specifies for which hand is input provided by the input data provider
        /// </summary>
        public HandType HandType
        {
            get => handType;
            set
            {
                if (handType == value)
                {
                    return;
                }
                
                handType = value;
                SetIsTracking(false);

                ObjectUtility.SetObjectDirty(this);
            }
        }

        public sealed override HandType GetHandType() => handType;

        [SerializeField]
        [Tooltip("Specifies whether the input provider logic should be executed in FixedUpdate instead of Update by default")]
        private bool runInFixedUpdate;
        /// <summary>
        /// Specifies whether the input provider logic should be executed in FixedUpdate instead of Update by default
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
        [Tooltip("Button state for every XR controller button")]
        [XRControllerButtonPropertyDrawOptions(
            ReadOnly = ReadOnlyPropertyDrawOptions.ReadOnlyAlways,
            IsCustomCollection = true,
            CustomCollectionBackingArrayOrListFieldPath = "items",
            CustomCollectionItemsReadOnly = ReadOnlyPropertyDrawOptions.NotReadOnly)]
        private XRContollerButtonCollection buttons = new XRContollerButtonCollection();
        /// <summary>
        /// Button state for every XR controller button
        /// </summary>
        public XRContollerButtonCollection Buttons => buttons;

        /// <summary>
        /// Scale of the hand skeleton
        /// </summary>
        public new float Scale
        {
            get => base.Scale; 
            set => base.Scale = value;
        }

        [SerializeField]
        [Tooltip("Bind pose of the hand skeleton. These are default poses of the hand bones, relative to parent bones. This is also " +
            "the target pose that will be provided by the input data provider when there is no suitable input source being tracked. " +
            "This property is optional; if nothing is assigned, then target pose when the input provider is not tracking is not going " +
            "to be set")]
        private HandSkeletonPose bindPose;
        /// <summary>
        /// Bind pose of the hand skeleton. These are default poses of the hand bones, relative to parent bones. This is also the 
        /// target pose that will be provided by the input data provider when there is no suitable input source being tracked. 
        /// This property is optional; if nothing is assigned, then target pose when the input provider is not tracking is not 
        /// going to be set
        /// </summary>
        public HandSkeletonPose BindPose
        {
            get => bindPose;
            set
            {
#if UNITY_EDITOR
                if (bindPose == value)
                {
                    return;
                }
#endif
                bindPose = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Specifies whether the assigned bind pose is going to be provided as target pose by the pose provider when not tracking. " +
            "This has effect only if there is a bind pose assigned to the input data provider")]
        private bool provideBindPoseWhenNotTracking = true;
        /// <summary>
        /// Specifies whether the assigned bind pose is going to be provided as target pose by the pose provider when not tracking. This
        /// has effect only if there is a bind pose assigned to the input data provider
        /// </summary>
        public bool ProvideBindPoseWhenNotTracking
        {
            get => provideBindPoseWhenNotTracking;
            set
            {
#if UNITY_EDITOR
                if (provideBindPoseWhenNotTracking == value)
                {
                    return;
                }
#endif
                provideBindPoseWhenNotTracking = value;
                ObjectUtility.SetObjectDirty(this);
            }
        }

        [SerializeField]
        [Tooltip("Maximum amount of velocity with which the bone positions relative to their parent bones can change when updating target " +
            "bone poses that are provided by the pose provider")]
        [Min(0)]
        private float maxBoneVelocity = 10;
        /// <summary>
        /// Maximum amount of velocity with which the bone positions relative to their parent bones can change when updating target bone 
        /// poses that are provided by the pose provider
        /// </summary>
        public float MaxBoneVelocity
        {
            get => maxBoneVelocity;
            set => maxBoneVelocity = Mathf.Max(0, value);
        }

        [SerializeField]
        [Tooltip("Maximum amount of angular velocity with which the bone rotations relative to their parent bones can change when updating " +
            "target bone poses that are provided by the pose provider, specified in degrees per second")]
        [Min(0)]
        private float maxBoneAngularVelocity = 720;
        /// <summary>
        /// Maximum amount of angular velocity with which the bone rotations relative to their parent bones can change when updating target
        /// bone poses that are provided by the pose provider, specified in degrees per second
        /// </summary>
        public float MaxBoneAngularVelocity
        {
            get => maxBoneAngularVelocity;
            set => maxBoneAngularVelocity = Mathf.Max(0, value);
        }

        [SerializeField]
        [Tooltip("Triggers that trigger the target poses that should be provided by the pose provider based on different input based " +
            "conditions. These are evaluated in the order as they appear in this list, the first trigger that has its pose condition " +
            "fulfilled is the active one. If no conditions are triggered, then the target pose is set to the bind pose if one is assigned, " +
            "otherwise it is not set at all")]
        [PropertyDrawOptions(LabelText = "", ArrayItemLabelIndexFormat = " #{0}", ArrayItemLabelBaseIndex = 1)]
        private List<XRControllerPoseTrigger> poseTriggers = new List<XRControllerPoseTrigger>();
        /// <summary>
        /// Triggers that trigger the target poses that should be provided by the pose provider based on different input based conditions. 
        /// These are evaluated in the order as they appear in this list, the first trigger that has its pose condition fulfilled is the 
        /// active one. If no conditions are triggered, then the target pose is set to the bind pose if one is assigned, otherwise it is 
        /// not set at all
        /// </summary>
        public List<XRControllerPoseTrigger> PoseTriggers => poseTriggers;

        private XRControllerPoseTrigger lastTrigger;
        private HandSkeletonPose lastTriggeredPose;

        private static readonly List<InputDevice> inputDevices = new List<InputDevice>();

        private InputDevice inputDevice;

        private readonly Pose[] bonePoses = new Pose[HandSkeletonConfiguration.BoneCount];

#if UNITY_EDITOR
        private bool wasInputDeviceValid = true;
#endif
        protected override void OnValidate()
        {
            base.OnValidate();

            for (var j = 0; j < poseTriggers.Count; j++)
            {
                var poseTrigger = poseTriggers[j];

                if (poseTrigger == null)
                {
                    poseTriggers.RemoveAt(j--);

                    continue;
                }

                if (poseTrigger.Condition == null)
                {
                    poseTriggers[j] = new XRControllerPoseTrigger();
                }
            }

            if (isActiveAndEnabled && IsTracking)
            {
                for (var i = 0; i < buttons.Count; i++)
                {
                    buttons[i].OnValidateWhenTracking();

                    if (!IsTracking)
                    {
                        break;
                    }
                }
            }
            else
            {
                inputDevice = default;

                for (var i = 0; i < buttons.Count; i++)
                {
                    buttons[i].Update(inputDevice);
                }
            }
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (!Application.IsPlaying(this))
            {
                var maxBoneDistanceChange = float.MaxValue;
                var maxBoneAngleChange = 180;

                if (IsTracking)
                {
                    UpdatePoseState(Pose.identity, maxBoneDistanceChange, maxBoneAngleChange);
                }
                else
                {
                    UpdatePoseStateWhenNotTracking(maxBoneDistanceChange, maxBoneAngleChange);
                }

                return;
            }
#endif
            if (!runInFixedUpdate)
            {
                UpdateInputAndPoseState(Time.deltaTime);
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
                UpdateInputAndPoseState(Time.fixedDeltaTime);
            }
        }

        protected override void TrackingLost()
        {
            base.TrackingLost();

            inputDevice = default;

            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[i].Update(inputDevice);
            }
        }

        private void UpdateInputAndPoseState(float deltaTime)
        {
            if (!inputDevice.isValid)
            {
                if (!TryResetInputDevice())
                {
                    SetIsTracking(false);

                    UpdatePoseStateWhenNotTracking(maxBoneVelocity * deltaTime, maxBoneAngularVelocity * deltaTime);

                    return;
                }
            }

            // We are here only if there is valid device to use
#if UNITY_EDITOR
            wasInputDeviceValid = true;
#endif
            try
            {
                SetIsTracking(true);
            }
            finally
            {
                if (IsTracking)
                {
                    Confidence = 1;

                    for (var i = 0; i < buttons.Count; i++)
                    {
                        buttons[i].Update(inputDevice);

                        if (!IsTracking)
                        {
                            break;
                        }
                    }
                }

                if (IsTracking)
                {
                    Pose devicePose;

                    inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out devicePose.position);
                    inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out devicePose.rotation);

                    UpdatePoseState(devicePose, maxBoneVelocity * deltaTime, maxBoneAngularVelocity * deltaTime);
                }
                else
                {
                    UpdatePoseStateWhenNotTracking(maxBoneVelocity * deltaTime, maxBoneAngularVelocity * deltaTime);
                }
            }
        }

        private bool TryResetInputDevice()
        {
            var deviceCharacteristics = handType == HandType.Left ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right;

            deviceCharacteristics |= InputDeviceCharacteristics.HeldInHand;

            InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, inputDevices);
#if UNITY_EDITOR
            if (inputDevices.Count > 1)
            {
                var log = LogUtility.FormatLogMessageFromComponent(this, $"Multiple devices detected for the {handType} side:");

                for (var i = 0; i < inputDevices.Count; i++)
                {
                    var device = inputDevices[i];

                    log += $"{Environment.NewLine}[{i + 1}] ";

                    if (device.isValid)
                    {
                        log += $"{device.name} (Manufacturer: {device.manufacturer ?? "N/A"})";
                    }
                    else
                    {
                        log += "(Not valid)";
                    }
                }

                Debug.LogWarning(log);
            }
#endif
            inputDevice = default;

            for (var i = 0; i < inputDevices.Count; i++)
            {
                var device = inputDevices[i];

                if (device.isValid)
                {
                    inputDevice = device;

                    break;
                }
            }

            if (!inputDevice.isValid)
            {
#if UNITY_EDITOR
                if (wasInputDeviceValid)
                {
                    wasInputDeviceValid = false;

                    var log = LogUtility.FormatLogMessageFromComponent(this, $"No valid input device to use for the {handType} side");

                    Debug.LogWarning(log);
                }
#endif
                return false;
            }
#if UNITY_EDITOR
            var message = LogUtility.FormatLogMessageFromComponent(this, $"Using input device for the {handType} side: " +
                $"{inputDevice.name} (Manufacturer: {inputDevice.manufacturer ?? "N/A"})");

            Debug.Log(message);
#endif
            return true;
        }

        private void UpdatePoseStateWhenNotTracking(float maxBoneDistanceChange, float maxBoneAngleChange)
        {
            lastTrigger = null;

            if (!bindPose || !provideBindPoseWhenNotTracking)
            {
                return;
            }

            var currentPoses = GetBoneRelativePoses();

            bonePoses[0] = currentPoses[0];

            for (var i = (int)HandSkeletonConfiguration.FirstFingerBoneId; i < HandSkeletonConfiguration.BoneCount; ++i)
            {
                var startPose = currentPoses[i];
                var targetPose = bindPose[i];

                ref var bonePose = ref bonePoses[i];

                GetDistanceFromStartAndEndPosition(
                    in startPose.position,
                    in targetPose.position,
                    out var moveDirection,
                    out var boneDistance);

                if (maxBoneDistanceChange < boneDistance)
                {
                    bonePose.position = startPose.position + moveDirection * maxBoneDistanceChange;
                }
                else
                {
                    bonePose.position = targetPose.position;
                }

                GetAxisAngleFromStartAndEndRotation(
                    in startPose.rotation,
                    in targetPose.rotation,
                    out var axis,
                    out var boneAngle);

                if (maxBoneAngleChange < boneAngle)
                {
                    bonePose.rotation = startPose.rotation * Quaternion.AngleAxis(maxBoneAngleChange, axis);
                }
                else
                {
                    bonePose.rotation = targetPose.rotation;
                }
            }

            SetBoneRelativePoses(bonePoses);
        }

        private void UpdatePoseState(Pose devicePose, float maxBoneDistanceChange, float maxBoneAngleChange)
        {
            GetTargetPose(
                out var targetPose,
                out var targetBones,
                out var targetPoseComponents,
                out var interpolateFromPose,
                out var toTargetPoseWeight);

            var rootPose = GetRootPose(devicePose);

            if (targetPose)
            {
                SetPose(
                    in rootPose,
                    targetPose,
                    targetBones,
                    targetPoseComponents,
                    interpolateFromPose,
                    toTargetPoseWeight,
                    maxBoneDistanceChange,
                    maxBoneAngleChange);
            }
            else
            {
                var currentPoses = GetBoneRelativePoses();

                bonePoses[0] = rootPose;

                for (var i = (int)HandSkeletonConfiguration.FirstFingerBoneId; i < HandSkeletonConfiguration.BoneCount; i++)
                {
                    bonePoses[i] = currentPoses[i];
                }

                SetBoneRelativePoses(bonePoses);
            }
        }

        private void GetTargetPose(
            out HandSkeletonPose targetPose,
            out HandBones targetBones,
            out PoseComponents targetPoseComponents,
            out HandSkeletonPose interpolateFromPose,
            out float toTargetPoseWeight)
        {
            for (var i = 0; i < poseTriggers.Count; i++)
            {
                var trigger = poseTriggers[i];

                if (trigger == null)
                {
                    poseTriggers.RemoveAt(i--);
                    ObjectUtility.SetObjectDirty(this);

                    continue;
                }

                if (!trigger.Enabled)
                {
                    continue;
                }

                if (trigger.Condition.Evaluate(buttons, out var buttonValue))
                {
                    targetPose = trigger.TargetPose;
                    targetBones = trigger.TargetBones;
                    targetPoseComponents = trigger.TargetPoseComponents;

                    if (lastTrigger != trigger)
                    {
                        lastTriggeredPose = lastTrigger?.TargetPose;
                        lastTrigger = trigger;
                    }

                    if (trigger.InterpolateTargetPose)
                    {
                        if (lastTriggeredPose)
                        {
                            interpolateFromPose = lastTriggeredPose;
                            toTargetPoseWeight = Mathf.Clamp01(buttonValue);
                        }
                        else if (bindPose)
                        {
                            interpolateFromPose = bindPose;
                            toTargetPoseWeight = Mathf.Clamp01(buttonValue);
                        }
                        else
                        {
                            interpolateFromPose = targetPose;
                            toTargetPoseWeight = 0;
                        }
                    }
                    else
                    {
                        interpolateFromPose = targetPose;
                        toTargetPoseWeight = 0;
                    }

                    return;
                }
            }

            if (bindPose)
            {
                targetPose = bindPose;
                targetBones = HandBones.FingerBones;
                targetPoseComponents = PoseComponents.PositionAndRotation;
                interpolateFromPose = bindPose;
                toTargetPoseWeight = 0;
                lastTrigger = null;

                return;
            }

            targetPose = default;
            targetBones = default;
            targetPoseComponents = default;
            interpolateFromPose = default;
            toTargetPoseWeight = default;
            lastTrigger = default;
        }

        private void SetPose(
            in Pose rootPose,
            HandSkeletonPose targetPose,
            HandBones targetBones,
            PoseComponents targetPoseComponents,
            HandSkeletonPose interpolateFromPose,
            float toTargetPoseWeight,
            float maxBoneDistanceChange,
            float maxBoneAngleChange)
        {
            var currentPoses = GetBoneRelativePoses();

            bonePoses[0] = rootPose;

            var boneFlag = 1;

            if (targetPoseComponents == PoseComponents.Position)
            {
                for (var i = (int)HandSkeletonConfiguration.FirstFingerBoneId; i < HandSkeletonConfiguration.BoneCount; i++)
                {
                    ref var bonePose = ref bonePoses[i];
                    var start = currentPoses[i];

                    if ((targetBones & (HandBones)(boneFlag << i)) == HandBones.None)
                    {
                        bonePose = start;
                        continue;
                    }

                    var targetPosition = Vector3.LerpUnclamped(interpolateFromPose[i].position, targetPose[i].position, toTargetPoseWeight);

                    GetDistanceFromStartAndEndPosition(
                        in start.position,
                        in targetPosition,
                        out var moveDirection,
                        out var boneDistance);

                    if (maxBoneDistanceChange < boneDistance)
                    {
                        bonePose.position = start.position + moveDirection * maxBoneDistanceChange;
                    }
                    else
                    {
                        bonePose.position = targetPosition;
                    }

                    bonePose.rotation = start.rotation;
                }
            }
            else if (targetPoseComponents == PoseComponents.Rotation)
            {
                for (var i = (int)HandSkeletonConfiguration.FirstFingerBoneId; i < HandSkeletonConfiguration.BoneCount; i++)
                {
                    ref var bonePose = ref bonePoses[i];
                    var start = currentPoses[i];

                    if ((targetBones & (HandBones)(boneFlag << i)) == HandBones.None)
                    {
                        bonePose = start;
                        continue;
                    }

                    bonePose.position = start.position;

                    var targetRotation = Quaternion.SlerpUnclamped(interpolateFromPose[i].rotation, targetPose[i].rotation, toTargetPoseWeight);

                    GetAxisAngleFromStartAndEndRotation(
                        in start.rotation,
                        in targetRotation,
                        out var axis,
                        out var boneAngle);

                    if (maxBoneAngleChange < boneAngle)
                    {
                        bonePose.rotation = start.rotation * Quaternion.AngleAxis(maxBoneAngleChange, axis);
                    }
                    else
                    {
                        bonePose.rotation = targetRotation;
                    }
                }
            }
            else
            {
                for (var i = (int)HandSkeletonConfiguration.FirstFingerBoneId; i < HandSkeletonConfiguration.BoneCount; i++)
                {
                    ref var bonePose = ref bonePoses[i];
                    var start = currentPoses[i];

                    if ((targetBones & (HandBones)(boneFlag << i)) == HandBones.None)
                    {
                        bonePose = start;
                        continue;
                    }

                    var interpolateFrom = interpolateFromPose[i];
                    var target = targetPose[i];

                    target.position = Vector3.LerpUnclamped(interpolateFrom.position, target.position, toTargetPoseWeight);
                    target.rotation = Quaternion.SlerpUnclamped(interpolateFrom.rotation, target.rotation, toTargetPoseWeight);

                    GetDistanceFromStartAndEndPosition(
                        in start.position,
                        in target.position,
                        out var moveDirection,
                        out var boneDistance);

                    if (maxBoneDistanceChange < boneDistance)
                    {
                        bonePose.position = start.position + moveDirection * maxBoneDistanceChange;
                    }
                    else
                    {
                        bonePose.position = target.position;
                    }

                    GetAxisAngleFromStartAndEndRotation(
                        in start.rotation,
                        in target.rotation,
                        out var axis,
                        out var boneAngle);

                    if (maxBoneAngleChange < boneAngle)
                    {
                        bonePose.rotation = start.rotation * Quaternion.AngleAxis(maxBoneAngleChange, axis);
                    }
                    else
                    {
                        bonePose.rotation = target.rotation;
                    }
                }
            }

            SetBoneRelativePoses(bonePoses);
        }

        private static void GetDistanceFromStartAndEndPosition(
            in Vector3 startPosition, in Vector3 endPosition, out Vector3 direction, out float distance)
        {
            var positionDelta = endPosition - startPosition;
            var dist = positionDelta.magnitude;
            var dir = positionDelta / dist;

            if (float.IsNaN(dir.x))
            {
                direction = Vector3.zero;
                distance = 0;

                return;
            }

            direction = dir;
            distance = dist;
        }

        private static void GetAxisAngleFromStartAndEndRotation(
            in Quaternion startRotation, in Quaternion endRotation, out Vector3 axis, out float degrees)
        {
            var rotationDelta = Quaternion.Inverse(startRotation) * endRotation;

            rotationDelta.ToAngleAxis(out var rotationAngle, out var rotationAxis);
            rotationAngle = Mathf.DeltaAngle(0, rotationAngle);

            if (float.IsNaN(rotationAngle * rotationAxis.x))
            {
                axis = Vector3.right;
                degrees = 0;

                return;
            }

            if (rotationAngle < 0f)
            {
                rotationAngle = -rotationAngle;
                rotationAxis = -rotationAxis;
            }

            axis = rotationAxis;
            degrees = rotationAngle;
        }
    }
}