namespace OctoXR.Input
{
    public class XRControllerButtonPropertyDrawOptionsAttribute : PropertyDrawOptionsAttribute
    {
        public override bool TryGetCustomFormattedLabelText(int targetObjectIndex, out string labelText)
        {
            labelText = targetObjectIndex > -1 && targetObjectIndex < (int)XRControllerButton.Primary2DAxis ? 
                StringUtility.GetSpaceSeparatedString(((XRControllerButton)targetObjectIndex).ToString(), true) :
                targetObjectIndex == (int)XRControllerButton.Primary2DAxis ? "Primary 2D Axis" :
                targetObjectIndex == (int)XRControllerButton.Secondary2DAxis ? "Secondary 2D Axis" :
                string.Empty;

            return true;
        }
    }
}