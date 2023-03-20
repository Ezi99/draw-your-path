using System;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.Input.ConfidenceTriggers
{
    [Serializable]
    public struct InputConfidenceBasedComponentTogglerTarget
    {
        /// <summary>
        /// Target component to toggle based on input confidence level. Note that this should be either <see cref="Behaviour"/> 
        /// or <see cref="Renderer"/> derived component
        /// </summary>
        [Tooltip("Target component to toggle based on input confidence level. Note that this should be either Behaviour or Renderer " +
            "derived component")]
        public Component Component;

        /// <summary>
        /// Should the target component be disabled on high input confidence and enabled on low confidence or vice versa
        /// </summary>
        [Tooltip("Should the target component be disabled on high input confidence and enabled on low confidence or vice versa")]
        public bool DisableOnHighConfidence;

        public InputConfidenceBasedComponentTogglerTarget(Component component, bool disableOnHighConfidence = false)
        {
            Component = component;
            DisableOnHighConfidence = disableOnHighConfidence;
        }
    }

    /// <summary>
    /// Toggles the enabled state of one or more target <see cref="Component"/>s based on a confidence level of source 
    /// <see cref="InputDataProvider"/>. Note that target components should be <see cref="Behaviour"/> or <see cref="Renderer"/>
    /// derived components, the component toggler will not be able to toggle other types of components
    /// </summary>
    public class InputConfidenceBasedComponentToggler : InputConfidenceTrigger
    {
        [SerializeField]
        [Tooltip("Target components to toggle based on input confidence level")]
        [PropertyDrawOptions(SkipInInspectorPropertyHierarchy = true)]
        private List<InputConfidenceBasedComponentTogglerTarget> toggleComponents;
        /// <summary>
        /// Target components to toggle based on input confidence level
        /// </summary>
        public List<InputConfidenceBasedComponentTogglerTarget> ToggleComponents => toggleComponents;

        protected override void UpdateConfidenceLow()
        {
            ConfidenceLow();
        }

        protected override void UpdateConfidenceHigh()
        {
            ConfidenceHigh();
        }

        private void ConfidenceLow()
        {
            for (var i = 0; i < toggleComponents.Count; i++)
            {
                var toggleComponent = toggleComponents[i];
                
                if (toggleComponent.Component)
                {
                    if (toggleComponent.Component is Behaviour behaviour)
                    {
                        behaviour.enabled = toggleComponent.DisableOnHighConfidence;
                    }
                    else if (toggleComponent.Component is Renderer renderer)
                    {
                        renderer.enabled = toggleComponent.DisableOnHighConfidence;
                    }
                }
            }
        }

        private void ConfidenceHigh()
        {
            for (var i = 0; i < toggleComponents.Count; i++)
            {
                var toggleComponent = toggleComponents[i];

                if (toggleComponent.Component)
                {
                    if (toggleComponent.Component is Behaviour behaviour)
                    {
                        behaviour.enabled = !toggleComponent.DisableOnHighConfidence;
                    }
                    else if (toggleComponent.Component is Renderer renderer)
                    {
                        renderer.enabled = !toggleComponent.DisableOnHighConfidence;
                    }
                }
            }
        }
    }
}