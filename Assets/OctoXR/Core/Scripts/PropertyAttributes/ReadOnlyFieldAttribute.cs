using UnityEngine;

namespace OctoXR
{
    public enum ReadOnlyFieldOption
    {
        ReadOnlyAlways,
        ReadOnlyInPlayMode,
        NotReadOnly
    }

    public class ReadOnlyFieldAttribute : PropertyAttribute
    {
        public ReadOnlyFieldOption Option { get; set; } = ReadOnlyFieldOption.ReadOnlyAlways;
    }
}
