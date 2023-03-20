namespace OctoXR
{
    public class HandFingerPropertyDrawOptionsAttribute : PropertyDrawOptionsAttribute
    {
        public override bool TryGetCustomFormattedLabelText(int targetObjectIndex, out string labelText)
        {
            labelText = targetObjectIndex > -1 && targetObjectIndex <= (int)HandFinger.Pinky ? 
                StringUtility.GetSpaceSeparatedString(((HandFinger)targetObjectIndex).ToString(), true) : 
                string.Empty;

            return true;
        }
    }
}