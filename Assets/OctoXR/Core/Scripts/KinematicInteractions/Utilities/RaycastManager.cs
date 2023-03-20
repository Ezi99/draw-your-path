using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    public abstract class RaycastManager
    {
        public static Grabbable target;
        public static bool isTargetHit;
        public static bool isRayHitting;

        private static Vector3 direction;
        private static RaycastHit raycastHit;

        public static void Raycast(Transform rayOrigin, float maxDistance, LayerMask layer)
        {
            direction = Vector3.Lerp(direction, rayOrigin.forward, 0.025f);
            isRayHitting = UnityEngine.Physics.Raycast(rayOrigin.position, direction, out raycastHit, maxDistance, layer);

            if (isRayHitting)
            {
                isTargetHit = true;
                target = raycastHit.collider.GetComponent<Grabbable>();
            }
            else
            {
                isTargetHit = false;
            }
        }

        public static void SphereCast(Transform sphereOrigin, float radius, float maxDistance, LayerMask layer)
        {
            direction = Vector3.Lerp(direction, sphereOrigin.forward, 1f);
            isRayHitting = UnityEngine.Physics.SphereCast(sphereOrigin.position, radius, direction, out raycastHit, maxDistance, layer);

            if (isRayHitting)
            {
                isTargetHit = true;
                target = raycastHit.collider.GetComponent<Grabbable>();
            }
            else
            {
                isTargetHit = false;
            }
        }
    }
}
