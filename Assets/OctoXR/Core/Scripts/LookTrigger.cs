using UnityEngine;
using UnityEngine.Events;

namespace OctoXR
{
    public class LookTrigger : MonoBehaviour
    {
        [SerializeField] private Transform centerEyeAnchor;
        [SerializeField] private float lookThreshold = 0.9f;
        [SerializeField] private UnityEvent onLook;
        [SerializeField] private UnityEvent onLookAway;
        
        private float lookingAmount = 0;

        private void Update()
        {
            lookingAmount = Vector3.Dot(centerEyeAnchor.forward, -transform.forward);
            
            if(lookingAmount >= lookThreshold)
            {
                onLook.Invoke();
            }
            else
            {
                onLookAway.Invoke();
            }
        }
    }
}
