using UnityEngine;

namespace OctoXR
{
    public class RayInteractor
    {
        protected float maxRayDistance;
        protected LayerMask layerMask;

        public RayInteractor(float maxRayDistance, LayerMask layerMask)
        {
            this.maxRayDistance = maxRayDistance;
            this.layerMask = layerMask;
        }

        protected bool Raycast(Vector3 raySource, Vector3 rayDirection, out RaycastHit raycastHit)
        {
            var ray = new Ray(raySource, rayDirection);

            return UnityEngine.Physics.Raycast(ray, out raycastHit, maxRayDistance, layerMask);
        }
    }
}
