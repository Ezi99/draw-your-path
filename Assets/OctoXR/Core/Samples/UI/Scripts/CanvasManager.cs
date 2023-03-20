using UnityEngine;
using UnityEngine.UI;

namespace OctoXR.Samples.UI
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Slider redSlider;
        [SerializeField] private Slider greenSlider;
        [SerializeField] private Slider blueSlider;
        [SerializeField] private Slider alphaSlider;

        public void RandomColor()
        {
            redSlider.value = Random.Range(0f, 1f);
            greenSlider.value = Random.Range(0f, 1f);
            blueSlider.value = Random.Range(0f, 1f);
            alphaSlider.value = toggle.isOn ? Random.Range(0f, 1f) : 1f;
        }

        public void DropdownValueChange()
        {
            if (dropdown.value == 0)
            {
                redSlider.value = 1;
                greenSlider.value = 0;
                blueSlider.value = 0;
                alphaSlider.value = 1;
            }
            else if (dropdown.value == 1)
            {
                redSlider.value = 0;
                greenSlider.value = 1;
                blueSlider.value = 0;
                alphaSlider.value = 1;
            } 
            else
            {
                redSlider.value = 0;
                greenSlider.value = 0;
                blueSlider.value = 1;
                alphaSlider.value = 1;
            }
        }

        public void ToggleValueChanged()
        {
            if (toggle.isOn)
            {
                alphaSlider.gameObject.SetActive(true);
            }
            else
            {
                alphaSlider.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (toggle.isOn)
            {
                image.color = new Color(redSlider.value, greenSlider.value, blueSlider.value, alphaSlider.value);
            }
            else
            {
                image.color = new Color(redSlider.value, greenSlider.value, blueSlider.value, 1);
            }
        }
    }
}
