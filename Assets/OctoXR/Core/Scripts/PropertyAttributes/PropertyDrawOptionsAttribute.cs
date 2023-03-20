using System;

namespace OctoXR
{
    [Flags]
    public enum ReadOnlyPropertyDrawOptions
    {
        InheritedFromParent = 0,
        ReadOnlyInPlayMode = 1,
        ReadOnlyAlways = 3,
        NotReadOnly = 4
    }

    public class PropertyDrawOptionsAttribute : LabelAttribute
    {
        public ReadOnlyPropertyDrawOptions ReadOnly { get; set; }
        public bool IsCustomCollection { get; set; }
        public string CustomCollectionBackingArrayOrListFieldPath { get; set; }
        public ReadOnlyPropertyDrawOptions CustomCollectionItemsReadOnly { get; set; }
        public bool SkipInInspectorPropertyHierarchy { get; set; }
        public bool SetValueViaPropertyOrMethod { get; set; }
        public string SetValueViaPropertyOrMethodName { get; set; }

        public virtual PropertyDrawOptionsAttribute Clone() => (PropertyDrawOptionsAttribute)MemberwiseClone();
    }
}