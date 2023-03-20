using UnityEngine;

namespace OctoXR.HandPoseDetection
{
    [System.Serializable]
    public class HandShapeData
    {
        [SerializeField] private string name;
        [SerializeField] private HandBoneId handBoneId;
        [SerializeField] private Vector3 relativeBonePosition;

        public HandBoneId HandBoneId => handBoneId;
        public Vector3 RelativeBonePosition { get => relativeBonePosition; set => relativeBonePosition = value; }

        public HandShapeData(HandBoneId handBoneId, Vector3 relativeBonePosition)
        {
            this.handBoneId = handBoneId;
            this.relativeBonePosition = relativeBonePosition;
            name = handBoneId.ToString();
        }
    }
}
