using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Abstract class which serves to give different functionality to a grabbable object.
    /// </summary>
    public abstract class GrabbableModifier : MonoBehaviour, IAttachable<Transform>
    {
        private Grabbable grabbable;
        public Grabbable Grabbable { get => grabbable; set => grabbable = value; }

        private void Awake()
        {
            grabbable = GetComponent<Grabbable>();
        }

        public virtual void Attach(Transform attachParent){}

        public virtual void Detach(){}
    }
}
