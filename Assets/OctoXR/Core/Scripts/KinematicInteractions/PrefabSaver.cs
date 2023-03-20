using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace OctoXR.KinematicInteractions
{
#if UNITY_EDITOR
    public class PrefabSaver
    {
        public static void CreatePrefab(GameObject gameObject, string path, bool useDefaultPath)
        {
            string localPath = "";

            if (useDefaultPath)
            {
                if (!Directory.Exists($"Assets/{SceneManager.GetActiveScene().name}_Prefabs"))
                    AssetDatabase.CreateFolder("Assets", $"{SceneManager.GetActiveScene().name}_Prefabs");
                localPath = $"Assets/{SceneManager.GetActiveScene().name}_Prefabs/" + gameObject.name + ".prefab";
            }
            else
            {
                localPath = path + gameObject.name + ".prefab";
            }
            bool prefabSuccess;

            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction, out prefabSuccess);
            if (prefabSuccess)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save" + prefabSuccess);

            AssetDatabase.GetAssetPath(gameObject);

        }

        [MenuItem("Examples/Create Prefab", true)]
        static bool ValidateCreatePrefab()
        {
            return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
        }
    }
#endif
}
