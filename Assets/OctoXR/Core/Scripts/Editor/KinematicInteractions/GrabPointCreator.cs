using OctoXR.KinematicInteractions;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    public abstract class GrabPointCreator
    {
        public static GameObject CreateGrabPoint(string name, Transform parent, HandType handType)
        {
            var grabPoint = new GameObject();
            grabPoint.transform.SetParent(parent);
            grabPoint.transform.localPosition = Vector3.zero;
            grabPoint.name = name;
            grabPoint.AddComponent<GrabPoint>().HandType = handType;

            return grabPoint;
        }
    }
}
