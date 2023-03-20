using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace OctoXR.Input
{
    [Serializable]
    public abstract class XRControllerButtonState
    {
        [SerializeField]
        [HideInInspector]
        private XRControllerButton button;
        /// <summary>
        /// Identifies the XR controller button
        /// </summary>
        public XRControllerButton Button => button;

        [SerializeField]
        [Tooltip("Is the button being touched?")]
        private bool isTouched;
        /// <summary>
        /// Is the button being touched?
        /// </summary>
        public bool IsTouched => isTouched;

        [SerializeField]
        [Tooltip("Is the button pressed?")]
        private bool isPressed;
        /// <summary>
        /// Is the button pressed?
        /// </summary>
        public bool IsPressed => isPressed;

        /// <summary>
        /// Value that specifies the amount of button press, or how much the button is being pressed, ranging from 0 to 1; 
        /// zero, if the button is not pressed at all; one, if the button is fully pressed
        /// </summary>
        public abstract float Value { get; }

        [SerializeField]
        [HideInInspector]
        private bool isOnTouchEventSent;
        [SerializeField]
        [HideInInspector]
        private bool isOnPressedEventSent;

        [SerializeField]
        private UnityEvent onTouch = new UnityEvent();
        public UnityEvent OnTouch => onTouch;

        [SerializeField]
        private UnityEvent onPressed = new UnityEvent();
        public UnityEvent OnPressed => onPressed;

        [SerializeField]
        private UnityEvent onDepressed = new UnityEvent();
        public UnityEvent OnDepressed => onDepressed;

        [SerializeField]
        private UnityEvent onUntouch = new UnityEvent();
        public UnityEvent OnUntouch => onUntouch;

        protected XRControllerButtonState(XRControllerButton button) => this.button = button;

        public abstract void Update(InputDevice inputDevice);

        public virtual void OnValidateWhenTracking()
        {
            if (isTouched)
            {
                NotifyTouched();
            }

            if (isPressed)
            {
                NotifyPressed();
            }
            else
            {
                NotifyDepressed();
            }

            if (!isTouched)
            {
                NotifyUntouched();
            }
        }

        protected void SetState(bool isTouched, bool isPressed)
        {
            if (this.isTouched != isTouched)
            {
                this.isTouched = isTouched;

                if (isTouched)
                {
                    NotifyTouched();
                }
                else
                {
                    NotifyUntouched();
                }
            }

            if (this.isPressed != isPressed)
            {
                this.isPressed = isPressed;

                if (isPressed)
                {
                    NotifyPressed();
                }
                else
                {
                    NotifyDepressed();
                }
            }
        }

        private void NotifyTouched()
        {
            if (!isOnTouchEventSent)
            {
                isOnTouchEventSent = true;

                try
                {
                    onTouch.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private void NotifyUntouched()
        {
            if (isOnTouchEventSent)
            {
                isOnTouchEventSent = false;

                try
                {
                    onUntouch.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private void NotifyPressed()
        {
            if (!isOnPressedEventSent)
            {
                isOnPressedEventSent = true;

                try
                {
                    onPressed.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private void NotifyDepressed()
        {
            if (isOnPressedEventSent)
            {
                isOnPressedEventSent = false;

                try
                {
                    onDepressed.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }

    [Serializable]
    public class XRControllerBinaryButton : XRControllerButtonState
    {
        public override float Value => IsPressed ? 1 : 0;

        public XRControllerBinaryButton(bool isSecondaryButton)
            : base(isSecondaryButton ? XRControllerButton.Secondary : XRControllerButton.Primary)
        {
        }

        public override void Update(InputDevice inputDevice)
        {
            var isTouched = false;
            var isPressed = false;

            if (inputDevice.isValid)
            {
                if (Button == XRControllerButton.Primary)
                {
                    inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out isTouched);
                    inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out isPressed);
                }
                else
                {
                    inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out isTouched);
                    inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out isPressed);
                }
            }

            SetState(isTouched, isPressed);
        }
    }

    [Serializable]
    public class XRControllerLerpButton : XRControllerButtonState
    {
        [SerializeField]
        [Tooltip("Value that specifies the amount of button press, or how much the button is being pressed, ranging from 0 to 1; " +
            "zero, if the button is not pressed at all; one, if the button is fully pressed")]
        [Range(0, 1)]
        private float value;
        public override float Value => value;

        [SerializeField]
        [Tooltip("Minimum value of button press required in order to consider the button as being touched, ranging from 0 to 1")]
        [Range(0, 1)]
        private float minValueToTouch = 0.01f;
        /// <summary>
        /// Minimum value of button press required in order to consider the button as being touched, ranging from 0 to 1
        /// </summary>
        public float MinValueToTouch
        {
            get => minValueToTouch;
            set
            {
                minValueToTouch = Mathf.Clamp01(value);
                SetState(minValueToTouch <= value, IsPressed);
            }
        }

        [SerializeField]
        [Tooltip("Minimum value of button press required in order to consider the button as being pressed, ranging from 0 to 1")]
        [Range(0, 1)]
        private float minValueToPress = 0.5f;
        /// <summary>
        /// Minimum value of button press required in order to consider the button as being pressed, ranging from 0 to 1
        /// </summary>
        public float MinValueToPress
        {
            get => minValueToPress;
            set
            {
                minValueToPress = Mathf.Clamp01(value);
                SetState(IsTouched, minValueToPress <= value);
            }
        }

        public XRControllerLerpButton(bool isGripButton)
            : base(isGripButton ? XRControllerButton.Grip : XRControllerButton.Trigger)
        {
        }

        public override void Update(InputDevice inputDevice)
        {
            value = 0;

            if (inputDevice.isValid)
            {
                inputDevice.TryGetFeatureValue(
                    Button == XRControllerButton.Trigger ? CommonUsages.trigger : CommonUsages.grip,
                    out value);
            }

            EvaluateAndSetBaseState();
        }

        public override void OnValidateWhenTracking()
        {
            EvaluateAndSetBaseState();
        }

        private void EvaluateAndSetBaseState()
        {
            var isTouched = minValueToTouch <= value;
            var isPressed = minValueToPress <= value;

            SetState(isTouched, isPressed);
        }
    }

    [Serializable]
    public class XRController2DAxis : XRControllerButtonState
    {
        [SerializeField]
        [Tooltip("The x-component of the axis")]
        [Range(-1, 1)]
        private float x;
        /// <summary>
        /// The x-component of the axis
        /// </summary>
        public float X => x;

        [SerializeField]
        [Tooltip("The y-component of the axis")]
        [Range(-1, 1)]
        private float y;
        /// <summary>
        /// The y-component of the axis
        /// </summary>
        public float Y => y;

        public override float Value => IsPressed ? 1 : 0;

        public XRController2DAxis(bool isSecondaryAxis)
            : base(isSecondaryAxis ? XRControllerButton.Secondary2DAxis : XRControllerButton.Primary2DAxis)
        {
        }

        public override void Update(InputDevice inputDevice)
        {
            var isTouched = false;
            var isPressed = false;
            var axis = Vector2.zero;

            if (inputDevice.isValid)
            {
                if (Button == XRControllerButton.Primary2DAxis)
                {
                    inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out isTouched);
                    inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out isPressed);
                    inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);
                }
                else
                {
                    inputDevice.TryGetFeatureValue(CommonUsages.secondary2DAxisTouch, out isTouched);
                    inputDevice.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out isPressed);
                    inputDevice.TryGetFeatureValue(CommonUsages.secondary2DAxis, out axis);
                }
            }

            x = axis.x;
            y = axis.y;

            SetState(isTouched, isPressed);
        }

        public override void OnValidateWhenTracking()
        {
            var axis = new Vector2(x, y);
            var magnitude = axis.magnitude;

            if (magnitude > 1)
            {
                axis /= magnitude;

                x = axis.x;
                y = axis.y;
            }

            base.OnValidateWhenTracking();
        }
    }
}
