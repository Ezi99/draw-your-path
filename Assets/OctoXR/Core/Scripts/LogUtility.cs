using System;
using UnityEngine;

namespace OctoXR
{
    public static class LogUtility
    {
        public static string FormatLogMessageFromComponent(Component component, string message)
        {
            return $"[{component.gameObject.name}][{component.GetType()}]{Environment.NewLine}{message}";
        }
    }
}
