using System;
using System.Collections.Generic;
using UnityEngine;

namespace OctoXR.Input.ConfidenceTriggers
{
    /// <summary>
    /// Enumerates parts of the common structure of a hand skeleton
    /// </summary>
    public enum HandSkeletonStructurePart
    {
        /// <summary>
        /// Specifies part of the hand skeleton structure that contains hand skeleton component - this could be viewed as the central 
        /// or root part of that structure
        /// </summary>
        [Tooltip("Specifies part of the hand skeleton structure that contains hand skeleton component - this could be viewed as the central " +
            "or root part of that structure")]
        HandSkeleton,

        /// <summary>
        /// Specifies part of the hand skeleton structure that comprises the bones of the hand skeleton - these are the objects that
        /// have a hand bone component attached
        /// </summary>
        [Tooltip("Specifies part of the hand skeleton structure that comprises the bones of the hand skeleton - these are the objects that " +
            "have a hand bone component attached")]
        HandBones
    }

    [Serializable]
    public struct InputConfidenceBasedHandSkeletonTogglerTarget
    {
        /// <summary>
        /// Target hand skeleton to toggle based on input confidence level
        /// </summary>
        [Tooltip("Target hand skeleton to toggle based on input confidence level")]
        public HandSkeleton HandSkeleton;

        /// <summary>
        /// Which part of the target hand skeleton to toggle
        /// </summary>
        [Tooltip("Which part of the target hand skeleton to toggle")]
        public HandSkeletonStructurePart TogglePart;


        /// <summary>
        /// In what way should the target hand skeleton part be toggled
        /// </summary>
        [Tooltip("In what way should the target hand skeleton part be toggled")]
        public BehaviourToggleType ToggleType;

        /// <summary>
        /// Should the target hand skeleton be toggled off on high input confidence and toggled on on low confidence or vice versa
        /// </summary>
        [Tooltip("Should the target hand skeleton be toggled off on high input confidence and toggled on on low confidence or vice versa")]
        public bool ToggleOffOnHighConfidence;

        public InputConfidenceBasedHandSkeletonTogglerTarget(
            HandSkeleton handSkeleton, 
            HandSkeletonStructurePart togglePart = HandSkeletonStructurePart.HandSkeleton,
            BehaviourToggleType toggleType = BehaviourToggleType.ToggleGameObject,
            bool toggleOffOnHighConfidence = false)
        {
            HandSkeleton = handSkeleton;
            TogglePart = togglePart;
            ToggleType = toggleType;
            ToggleOffOnHighConfidence = toggleOffOnHighConfidence;
        }
    }

    /// <summary>
    /// Toggles the active state of one or more target <see cref="HandSkeleton"/>s based on a confidence level of source 
    /// <see cref="InputDataProvider"/>
    /// </summary>
    [DefaultExecutionOrder(-85)]
    public class InputConfidenceBasedHandSkeletonToggler : InputConfidenceTrigger
    {
        [SerializeField]
        [Tooltip("Target hand skeletons to toggle based on input confidence level")]
        [PropertyDrawOptions(SkipInInspectorPropertyHierarchy = true)]
        private List<InputConfidenceBasedHandSkeletonTogglerTarget> toggleHandSkeletons;
        /// <summary>
        /// Target hand skeletons to toggle based on input confidence level
        /// </summary>
        public List<InputConfidenceBasedHandSkeletonTogglerTarget> ToggleHandSkeletons => toggleHandSkeletons;

        protected virtual void Reset()
        {
            ConfidenceThreshold = 0.75f;
            TriggerEvaluationRun = InputConfidenceTriggerEvaluationRun.UpdateAndFixedUpdate;
        }

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
            for (var i = 0; i < toggleHandSkeletons.Count; i++)
            {
                var toggleHandSkeleton = toggleHandSkeletons[i];
                var toggle = toggleHandSkeleton.ToggleOffOnHighConfidence;

                ToggleHandSkeleton(toggleHandSkeleton.HandSkeleton, toggleHandSkeleton.TogglePart, toggleHandSkeleton.ToggleType, toggle);
            }
        }

        private void ConfidenceHigh()
        {
            for (var i = 0; i < toggleHandSkeletons.Count; i++)
            {
                var toggleHandSkeleton = toggleHandSkeletons[i];
                var toggle = !toggleHandSkeleton.ToggleOffOnHighConfidence;

                ToggleHandSkeleton(toggleHandSkeleton.HandSkeleton, toggleHandSkeleton.TogglePart, toggleHandSkeleton.ToggleType, toggle);
            }
        }

        private static void ToggleHandSkeleton(
            HandSkeleton handSkeleton, 
            HandSkeletonStructurePart part, 
            BehaviourToggleType toggleType, 
            bool toggle)
        {
            if (!handSkeleton)
            {
                return;
            }

            if (toggleType == BehaviourToggleType.ToggleGameObject)
            {
                if (part == HandSkeletonStructurePart.HandSkeleton)
                {
                    ToggleGameObject(handSkeleton.gameObject, toggle);
                }
                else
                {
                    var bones = handSkeleton.Bones;

                    for (var i = 0; i < bones.Count; i++)
                    {
                        ToggleGameObject(bones[i].gameObject, toggle);
                    }
                }
            }
            else
            {
                if (part == HandSkeletonStructurePart.HandSkeleton)
                {
                    handSkeleton.enabled = toggle;
                }
                else
                {
                    var bones = handSkeleton.Bones;

                    for (var i = 0; i < bones.Count; i++)
                    {
                        bones[i].enabled = toggle;
                    }
                }
            }
        }

        private static void ToggleGameObject(GameObject gameObject, bool toggle)
        {
            if (gameObject.activeSelf != toggle)
            {
                gameObject.SetActive(toggle);
            }
        }
    }
}