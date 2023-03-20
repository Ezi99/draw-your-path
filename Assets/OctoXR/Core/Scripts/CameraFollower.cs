using UnityEngine;

namespace OctoXR
{
    public class CameraFollower : MonoBehaviour
    {
        [HideInInspector] public Vector3 Offset;
        private Transform _transform;
        private Transform _camera;

        private void Start()
        {
            _transform = transform;
            _camera = Camera.main.transform;
        }

        private void LateUpdate() => 
            _transform.SetPositionAndRotation(_camera.position + Offset, Quaternion.Euler(0, _camera.rotation.eulerAngles.y, 0));
    }
}
