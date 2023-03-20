using System;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    [Serializable]
    public class ManipulationBase
    {
        [SerializeField] private PositionManipulator positionManipulator;
        public PositionManipulator PositionManipulator { get => positionManipulator; set => positionManipulator = value; }

        [SerializeField] private ScaleManipulator scaleManipulator;
        public ScaleManipulator ScaleManipulator { get => scaleManipulator; set => scaleManipulator = value; }

        [SerializeField] private RotationManipulator rotationManipulator;
        public RotationManipulator RotationManipulator { get => rotationManipulator; set => rotationManipulator = value; }
    }
}
