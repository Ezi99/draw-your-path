using UnityEngine;
using UnityEngine.UI;

namespace OctoXR.Samples.HandPoseDetection
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private Image leftImage;
        [SerializeField] private Image rightImage;
        [SerializeField] private Sprite[] sprites;

        public void LeftGestureDetected(int spriteIndex)
        {
            ChangeImageSprite(leftImage, spriteIndex);
        }

        public void RightGestureDetected(int spriteIndex)
        {
            ChangeImageSprite(rightImage, spriteIndex);
        }

        private void ChangeImageSprite(Image image, int spriteIndex)
        {
            image.sprite = sprites[spriteIndex];
        }
    }
}
