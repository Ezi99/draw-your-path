using System.Collections;
using UnityEngine;

namespace OctoXR
{
    public class ResetTrackerOnLoad : MonoBehaviour
    {
        [SerializeField] private Transform _camera;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            var centerEyeAnchorForward = Vector3.ProjectOnPlane(_camera.forward, Vector3.up).normalized;
            var rot = Quaternion.FromToRotation(centerEyeAnchorForward, transform.forward);
            transform.rotation *= rot;

            var offset = new Vector3(_camera.localPosition.x, 0, _camera.localPosition.z);
            transform.Translate(-offset);
        }
    }
}
