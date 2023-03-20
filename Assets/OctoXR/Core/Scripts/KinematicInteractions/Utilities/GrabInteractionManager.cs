using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    enum InteractionType
    {
        None,
        Hide,
        Material
    }

    public class GrabInteractionManager : MonoBehaviour
    {
        [SerializeField] private Material onGrabMaterial;
        [SerializeField] private InteractionType interactionType;
        private Material originalMaterial;
        private ProximityGrabController proximityGrabController;

        private void Awake()
        {
            proximityGrabController = GetComponent<ProximityGrabController>();
        }

        public void HideMeshRenderer(SkinnedMeshRenderer handMesh)
        {
            handMesh.enabled = false;
        }

        public void ShowMeshRenderer(SkinnedMeshRenderer handMesh)
        {
            handMesh.enabled = true;
        }
    }
}
