using UnityEngine;

namespace OctoXR
{
    public class LabelAttribute : PropertyAttribute
    {
        public string LabelText { get; set; }
        public string ArrayItemLabelIndexFormat { get; set; }
        public int ArrayItemLabelBaseIndex { get; set; }

        public LabelAttribute() { }
        public LabelAttribute(string labelText) => LabelText = labelText;

        public virtual bool TryGetCustomFormattedLabelText(int targetObjectIndex, out string labelText)
        {
            labelText = null;

            return false;
        }
    }
}