namespace OctoXR
{
    public class HandBonePropertyDrawOptionsAttribute : PropertyDrawOptionsAttribute
    {
        public override bool TryGetCustomFormattedLabelText(int targetObjectIndex, out string labelText)
        {
            labelText = targetObjectIndex > -1 && targetObjectIndex <= (int)HandBoneId.PinkyFingerTip ? 
                StringUtility.GetSpaceSeparatedString(((HandBoneId)targetObjectIndex).ToString(), true) : 
                string.Empty;

            return true;
        }
    }
}