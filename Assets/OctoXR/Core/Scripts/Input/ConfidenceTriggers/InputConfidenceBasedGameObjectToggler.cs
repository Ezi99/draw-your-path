using System;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.Input.ConfidenceTriggers
{
    [Serializable]
    public struct InputConfidenceBasedGameObjectTogglerTarget
    {
        /// <summary>
        /// Target GameObject to toggle based on input confidence level
        /// </summary>
        [Tooltip("Target GameObject to toggle based on input confidence level")]
        public GameObject Object;

        /// <summary>
        /// Should the target object be deactived on high input confidence and actived on low confidence or vice versa
        /// </summary>
        [Tooltip("Should the target object be deactived on high input confidence and actived on low confidence or vice versa")]
        public bool DeactivateOnHighConfidence;

        public InputConfidenceBasedGameObjectTogglerTarget(GameObject @object, bool deactivateOnHighConfidence = false)
        {
            Object = @object;
            DeactivateOnHighConfidence = deactivateOnHighConfidence;
        }
    }

    /// <summary>
    /// Toggles the active state of one or more target <see cref="GameObject"/>s based on a confidence level of source 
    /// <see cref="InputDataProvider"/>
    /// </summary>
    public class InputConfidenceBasedGameObjectToggler : InputConfidenceTrigger
    {
        [SerializeField]
        [Tooltip("Target GameObjects to toggle based on input confidence level")]
        [PropertyDrawOptions(SkipInInspectorPropertyHierarchy = true)]
        private List<InputConfidenceBasedGameObjectTogglerTarget> toggleObjects;
        /// <summary>
        /// Target GameObjects to toggle based on input confidence level
        /// </summary>
        public List<InputConfidenceBasedGameObjectTogglerTarget> ToggleObjects => toggleObjects;

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
            for (var i = 0; i < toggleObjects.Count; i++)
            {
                var toggleObject = toggleObjects[i];
                var active = toggleObject.DeactivateOnHighConfidence;

                if (toggleObject.Object && toggleObject.Object.activeSelf != active)
                {
                    toggleObject.Object.SetActive(active);
                }
            }
        }

        private void ConfidenceHigh()
        {
            for (var i = 0; i < toggleObjects.Count; i++)
            {
                var toggleObject = toggleObjects[i];
                var active = !toggleObject.DeactivateOnHighConfidence;

                if (toggleObject.Object && toggleObject.Object.activeSelf != active)
                {
                    toggleObject.Object.SetActive(active);
                }
            }
        }
    }
}