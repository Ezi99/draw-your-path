using System;
using UnityEngine;

namespace OctoXR.Input
{
    public enum XRControllerConditionType
    {
        ButtonAction,
        Binary,
        Constant
    }

    public enum XRControllerButtonAction
    {
        Press,
        Touch
    }

    public enum BinaryXRControllerConditionButtonValueCombine
    {
        ValueFromCondition1,
        ValueFromCondition2,
        Avg,
        Max,
        Min
    }

    public static class XRControllerPoseConditionUtility
    {
        public static void SetConditionType(
            ref XRControllerConditionType conditionType,
            ref XRControllerCondition condition,
            XRControllerConditionType valueToSet)
        {
            if (conditionType == valueToSet)
            {
                return;
            }

            conditionType = valueToSet;

            if (conditionType == XRControllerConditionType.ButtonAction)
            {
                condition = new ButtonActionXRControllerCondition();
            }
            else if (conditionType == XRControllerConditionType.Binary)
            {
                condition = new BinaryXRControllerCondition();
            }
            else
            {
                condition = new ConstantXRControllerCondition();
            }
        }
    }

    [Serializable]
    public abstract class XRControllerCondition
    {
        [SerializeField]
        [Tooltip("Should the condition's evaluation results be inverted (true to false, false to true)")]
        private bool invert;
        /// <summary>
        /// Should the condition's evaluation results be inverted (true to false, false to true)
        /// </summary>
        public bool Invert
        {
            get => invert;
            set => invert = value;
        }

        /// <summary>
        /// Type of the condition
        /// </summary>
        public abstract XRControllerConditionType ConditionType { get; }

        /// <summary>
        /// Evaluates the condition based on input controller button state
        /// </summary>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public bool Evaluate(XRContollerButtonCollection buttons, out float buttonValue) => invert ^ EvaluateCore(buttons, out buttonValue);

        /// <summary>
        /// Evaluates the condition based on input controller button state
        /// </summary>
        /// <param name="buttons"></param>
        /// <returns></returns>
        protected abstract bool EvaluateCore(XRContollerButtonCollection buttons, out float buttonValue);
    }

    [Serializable]
    public class ConstantXRControllerCondition : XRControllerCondition
    {
        public sealed override XRControllerConditionType ConditionType => XRControllerConditionType.Constant;

        protected override bool EvaluateCore(XRContollerButtonCollection buttons, out float buttonValue)
        {
            buttonValue = 1;
            return true;
        }
    }

    [Serializable]
    public class ButtonActionXRControllerCondition : XRControllerCondition
    {
        [SerializeField]
        [Tooltip("Button whose state is being evaluated by the condition")]
        private XRControllerButton button;
        /// <summary>
        /// Button whose state is being evaluated by the condition
        /// </summary>
        public XRControllerButton Button
        {
            get => button;
            set => button = value;
        }

        [SerializeField]
        [Tooltip("Button action that is evaluated by the condition")]
        private XRControllerButtonAction action;
        /// <summary>
        /// Button action that is evaluated by the condition
        /// </summary>
        public XRControllerButtonAction Action
        {
            get => action;
            set => action = value;
        }

        public sealed override XRControllerConditionType ConditionType => XRControllerConditionType.ButtonAction;

        public ButtonActionXRControllerCondition() { }
        public ButtonActionXRControllerCondition(XRControllerButton button, XRControllerButtonAction action)
        { 
            this.button = button;
            this.action = action;
        }

        protected override bool EvaluateCore(XRContollerButtonCollection buttons, out float buttonValue)
        {
            if (buttons == null)
            {
                throw new ArgumentNullException(nameof(buttons));
            }

            var button = buttons[this.button];

            buttonValue = button.Value;

            return action == XRControllerButtonAction.Press ? button.IsPressed : button.IsTouched;
        }
    }

    [Serializable]
    public class BinaryXRControllerCondition : XRControllerCondition
    {
        [SerializeField]
        [Tooltip("Type of nested condition that appears on the left-hand side of the binary condition's evaluation")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true, SetValueViaPropertyOrMethodName = nameof(SetConditionType1))]
        private XRControllerConditionType conditionType1;
        private void SetConditionType1(XRControllerConditionType value)
        {
            XRControllerPoseConditionUtility.SetConditionType(ref conditionType1, ref condition1, value);
        }

        [SerializeReference]
        [Tooltip("Nested condition that appears on the left-hand side of the binary condition's evaluation")]
        private XRControllerCondition condition1;
        /// <summary>
        /// Nested condition that appears on the left-hand side of the binary condition's evaluation
        /// </summary>
        public XRControllerCondition Condition1
        {
            get => condition1;
            set
            {
                if (condition1 == value)
                {
                    return;
                }

                condition1 = value ?? throw new ArgumentNullException(nameof(value));
                conditionType1 = condition1.ConditionType;
            }
        }

        [SerializeField]
        [Tooltip("Boolean logic operation to perform with nested conditions when evaluating the binary condition")]
        private BooleanBinaryOp op;
        /// <summary>
        /// Boolean logic operation to perform with nested conditions when evaluating the binary condition
        /// </summary>
        public BooleanBinaryOp Op
        {
            get => op;
            set => op = value;
        }

        [SerializeField]
        [Tooltip("Type of nested condition that appears on the right-hand side of the binary condition's evaluation")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true, SetValueViaPropertyOrMethodName = nameof(SetConditionType2))]
        private XRControllerConditionType conditionType2;
        private void SetConditionType2(XRControllerConditionType value)
        {
            XRControllerPoseConditionUtility.SetConditionType(ref conditionType2, ref condition2, value);
        }

        [SerializeReference]
        [Tooltip("Nested condition that appears on the right-hand side of the binary condition's evaluation")]
        private XRControllerCondition condition2;
        /// <summary>
        /// Nested condition that appears on the right-hand side of the binary condition's evaluation
        /// </summary>
        public XRControllerCondition Condition2
        {
            get => condition2;
            set
            {
                if (condition2 == value)
                {
                    return;
                }

                condition2 = value ?? throw new ArgumentNullException(nameof(value));
                conditionType2 = condition2.ConditionType;
            }
        }

        [SerializeField]
        [Tooltip("Specifies how should the binary condition calculate total combined button value which is presumably based on " +
            "the state of controller buttons involved in the binary condition's nested conditions")]
        private BinaryXRControllerConditionButtonValueCombine buttonValueCombine;
        /// <summary>
        /// Specifies how should the binary condition calculate total combined button value which is presumably based on the state
        /// of controller buttons involved in the binary condition's nested conditions
        /// </summary>
        public BinaryXRControllerConditionButtonValueCombine ButtonValueCombine
        {
            get => buttonValueCombine;
            set => buttonValueCombine = value;
        }

        public sealed override XRControllerConditionType ConditionType => XRControllerConditionType.Binary;

        public BinaryXRControllerCondition()
        {
            InitializeDefault();
        }

        public BinaryXRControllerCondition(
            XRControllerCondition condition1,
            XRControllerCondition condition2,
            BinaryXRControllerConditionButtonValueCombine buttonValueCombine = 
            BinaryXRControllerConditionButtonValueCombine.ValueFromCondition1)
        {
            if (condition1 == null)
            {
                InitializeDefault();

                throw new ArgumentNullException(nameof(condition1));
            }

            if (condition2 == null)
            {
                InitializeDefault();

                throw new ArgumentNullException(nameof(condition2));
            }

            conditionType1 = condition1.ConditionType;
            this.condition1 = condition1;
            conditionType2 = condition2.ConditionType;
            this.condition2 = condition2;
            this.buttonValueCombine = buttonValueCombine;
        }

        private void InitializeDefault()
        {
            conditionType1 = XRControllerConditionType.ButtonAction;
            condition1 = new ButtonActionXRControllerCondition();
            conditionType2 = XRControllerConditionType.ButtonAction;
            condition2 = new ButtonActionXRControllerCondition();
        }

        protected override bool EvaluateCore(XRContollerButtonCollection buttons, out float buttonValue)
        {
            float buttonValue1;
            float buttonValue2;

            var result = op switch
            {
                BooleanBinaryOp.And => condition1.Evaluate(buttons, out buttonValue1) & condition2.Evaluate(buttons, out buttonValue2),
                BooleanBinaryOp.Or => condition1.Evaluate(buttons, out buttonValue1) | condition2.Evaluate(buttons, out buttonValue2),
                BooleanBinaryOp.ExOr => condition1.Evaluate(buttons, out buttonValue1) ^ condition2.Evaluate(buttons, out buttonValue2),
                _ => throw new NotSupportedException("Operation not supported")
            };

            buttonValue = buttonValueCombine switch
            {
                BinaryXRControllerConditionButtonValueCombine.ValueFromCondition1 => buttonValue1,
                BinaryXRControllerConditionButtonValueCombine.ValueFromCondition2 => buttonValue2,
                BinaryXRControllerConditionButtonValueCombine.Avg => (buttonValue1 + buttonValue2) / 2,
                BinaryXRControllerConditionButtonValueCombine.Max => Mathf.Max(buttonValue1, buttonValue2),
                BinaryXRControllerConditionButtonValueCombine.Min => Mathf.Min(buttonValue1, buttonValue2),
                _ => 0
            };

            return result;
        }
    }

    [Serializable]
    public class XRControllerPoseTrigger
    {
        [SerializeField]
        [Tooltip("Is the trigger enabled? Disabled triggers will not be evaluated and will be ignored")]
        private bool enabled;
        /// <summary>
        /// Is the trigger enabled? Disabled triggers will not be evaluated and will be ignored
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        [SerializeField]
        [Tooltip("Type of condition that needs to be fulfilled in order to trigger the target pose")]
        [PropertyDrawOptions(SetValueViaPropertyOrMethod = true, SetValueViaPropertyOrMethodName = nameof(SetConditionType))]
        private XRControllerConditionType conditionType;
        private void SetConditionType(XRControllerConditionType value)
        {
            XRControllerPoseConditionUtility.SetConditionType(ref conditionType, ref condition, value);
        }

        [SerializeReference]
        [Tooltip("Condition that needs to be fulfilled in order to trigger the target pose")]
        private XRControllerCondition condition;
        /// <summary>
        /// Condition that needs to be fulfilled in order to trigger the target pose
        /// </summary>
        public XRControllerCondition Condition
        {
            get => condition;
            set => condition = value ?? throw new ArgumentNullException(nameof(value));
        }

        [SerializeField]
        [Tooltip("Hand skeleton pose that should be provided when the pose condition is fulfilled")]
        private HandSkeletonPose targetPose;
        /// <summary>
        /// Hand skeleton pose that should be provided when the pose condition is fulfilled
        /// </summary>
        public HandSkeletonPose TargetPose
        {
            get => targetPose;
            set => targetPose = value;
        }

        [SerializeField]
        [Tooltip("Hand bones affected by the target pose. These are key bones, only these should be affected when the target pose " +
            "is triggered")]
        private HandBones targetBones = HandBones.RotatingFingerBones;
        /// <summary>
        /// Hand bones affected by the target pose. These are key bones, only these should be affected when the target pose is triggered
        /// </summary>
        public HandBones TargetBones
        {
            get => targetBones;
            set => targetBones = value;
        }

        [SerializeField]
        [Tooltip("Specifies which components of the target hand bone poses should be affected when the target pose is triggered")]
        private PoseComponents targetPoseComponents = PoseComponents.Rotation;
        /// <summary>
        /// Specifies which components of the target hand bone poses should be affected when the target pose is triggered
        /// </summary>
        public PoseComponents TargetPoseComponents
        {
            get => targetPoseComponents;
            set => targetPoseComponents = value;
        }

        [SerializeField]
        [Tooltip("Specifies whether the target pose should be obtained by interpolating between current pose and the assigned " +
            "target pose with the interpolation factor that is obtained as a combined button value of all the buttons involved " +
            "in the pose trigger's condition")]
        private bool interpolateTargetPose;
        /// <summary>
        /// Specifies whether the target pose should be obtained by interpolating between current pose and the assigned target
        /// pose with the interpolation factor that is obtained as a combined button value of all the buttons involved in the
        /// pose trigger's condition
        /// </summary>
        public bool InterpolateTargetPose
        {
            get => interpolateTargetPose;
            set => interpolateTargetPose = value;
        }

        public XRControllerPoseTrigger()
        {
            enabled = true;
            InitializeDefault();
        }

        public XRControllerPoseTrigger(
            XRControllerCondition condition,
            HandSkeletonPose targetPose = null,
            HandBones targetBones = HandBones.RotatingFingerBones,
            PoseComponents targetPoseComponents = PoseComponents.Rotation,
            bool interpolateTargetPose = false)
        {
            if (condition == null)
            {
                InitializeDefault();

                throw new ArgumentNullException(nameof(condition));
            }

            enabled = true;
            conditionType = condition.ConditionType;
            this.condition = condition;
            this.targetPose = targetPose;
            this.targetBones = targetBones;
            this.targetPoseComponents = targetPoseComponents;
            this.interpolateTargetPose = interpolateTargetPose;
        }

        private void InitializeDefault()
        {
            conditionType = XRControllerConditionType.ButtonAction;
            condition = new ButtonActionXRControllerCondition();
        }
    }
}
