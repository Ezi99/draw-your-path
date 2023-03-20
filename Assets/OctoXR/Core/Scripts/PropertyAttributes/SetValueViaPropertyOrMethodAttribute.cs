using UnityEngine;

namespace OctoXR
{
    public class SetValueViaPropertyOrMethodAttribute : PropertyAttribute
    {
        public string PropertyOrMethodName { get; set; }
    }
}