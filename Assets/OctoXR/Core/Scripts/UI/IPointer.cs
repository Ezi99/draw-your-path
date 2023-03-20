using UnityEngine;

namespace OctoXR.UI
{
    public interface IPointer
    {
        public bool IsProviderTracking { get; }
        public float GetSelectActionStrength();
        public Vector3 CalculateRayDirection();
        public void InjectPalmCenter(Transform palmCenter);
    }
}
