using System;
using UnityEngine;

namespace OctoXR
{
    [Serializable]
    [CreateAssetMenu(fileName = "NewOctoSettings", menuName = "OctoXR/Setup/OctoSettings")]
    public class OctoSettings : ScriptableObject
    {
        public int defSolverIterations = 25;
        public int defSolverVelocityIterations = 15;
        public float fixedTimeStep = 0.011111111f;
        public bool ignoreSetup;
    }
}
