using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    public class PrefabSpawner : MonoBehaviour
    {
        public void SpawnPrefabAtTransform(GameObject gameObject, Transform spawnPosition, Transform parent, float destroyTime)
        {
            var gameObjectInstance = Instantiate(gameObject,
                new Vector3(spawnPosition.position.x, spawnPosition.position.y, spawnPosition.position.z), gameObject.transform.rotation);
            Destroy(gameObjectInstance, destroyTime);

            if (parent != null) gameObjectInstance.transform.SetParent(parent);
        }

        public void SpawnPrefabAtVector(GameObject gameObject, Vector3 spawnPosition, Transform parent, float destroyTime)
        {
            gameObject.transform.localPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
            var gameObjectInstance = Instantiate(gameObject,
                gameObject.transform.localPosition, gameObject.transform.rotation);
            Destroy(gameObjectInstance, destroyTime);

            if (parent != null) gameObjectInstance.transform.SetParent(parent);
        }
    }
}
