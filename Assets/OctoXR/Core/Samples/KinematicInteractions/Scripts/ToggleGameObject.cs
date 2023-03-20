using UnityEngine;

namespace OctoXR.Samples.KinematicInteractions
{
    public class ToggleGameObject : MonoBehaviour
    {
        public void ToggleObject(GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
