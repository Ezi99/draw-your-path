using UnityEngine;

namespace OctoXR
{
    public static class ObjectUtility
    {
        public static void DestroyObject(Object obj)
        {
            DestroyObject(obj, false);
        }

        public static void DestroyObject(Object obj, bool destroyIfObjectAsset)
        {
            if (obj)
            {
#if UNITY_EDITOR
                if (Application.IsPlaying(obj))
                {
#endif
                    Object.Destroy(obj);
#if UNITY_EDITOR
                }
                else
                {
                    Object.DestroyImmediate(obj, destroyIfObjectAsset);
                }
#endif
            }
        }

        public static void SetObjectDirty(Object obj)
        {
#if UNITY_EDITOR
            if (obj && !Application.IsPlaying(obj))
            {
                UnityEditor.EditorUtility.SetDirty(obj);
            }
#endif
        }
    }
}
