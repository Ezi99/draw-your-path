using UnityEngine;

namespace OctoXR
{
    /// <summary>
    /// Enumerates common ways to toggle a behaviour or logic active state
    /// </summary>
    public enum BehaviourToggleType
    {
        /// <summary>
        /// Toggle behaviour by changing the active state of the behaviour's GameObject (or possibly some other behaviour 
        /// related GameObject)
        /// </summary>
        [Tooltip("Toggle behaviour by changing the active state of the behaviour's GameObject (or possibly some other behaviour " +
            "related GameObject)")]
        ToggleGameObject,

        /// <summary>
        /// Toggle behaviour by changing the enabled state of the behaviour itself, assuming the behaviour is implemented as a
        /// GameObject component
        /// </summary>
        [Tooltip("Toggle behaviour by changing the enabled state of the behaviour itself, assuming the behaviour is implemented as a " +
            "GameObject component")]
        ToggleComponent
    }
}
