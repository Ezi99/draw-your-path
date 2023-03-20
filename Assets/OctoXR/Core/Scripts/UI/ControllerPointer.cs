using UnityEngine;
using OctoXR.Input;

namespace OctoXR.UI
{
    public class ControllerPointer : MonoBehaviour, IPointer
    {
        [SerializeField] private UnityXRControllerInputDataProvider controllerInputDataProvider;
        [SerializeField] private XRControllerButton selectButton = XRControllerButton.Trigger;
        private XRControllerButtonState button;
        private Transform palmCenter;
        public bool IsProviderTracking => controllerInputDataProvider.IsTracking;

        private void Awake() => button = controllerInputDataProvider.Buttons[selectButton];

        public float GetSelectActionStrength() => button.Value; 

        public Vector3 CalculateRayDirection() => palmCenter.up; 

        public void InjectPalmCenter(Transform palmCenter) => this.palmCenter = palmCenter;
    }
}
