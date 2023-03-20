using UnityEngine;

namespace OctoXR.KinematicInteractions.Utilities
{
    public class CircleCreator : MonoBehaviour
    {
        [Range(0, 10)][SerializeField] private int slotAmount;
        [Range(0f, 1f)] public float radius = 1f;
        public GameObject spawnablePrefab;
        public string prefabName = "Default";

        private void Start()
        {
            SpawnPrefabs(spawnablePrefab);
        }

        private void SpawnPrefabs(GameObject prefabToSpawn)
        {
            for (var i = 0; i < slotAmount; i++)
            {
                var theta = i * 2 * Mathf.PI / slotAmount;
                var x = Mathf.Sin(theta) * radius;
                var z = Mathf.Cos(theta) * radius;
                const float y = 0;

                SpawnPrefab(x, y, z, i, prefabToSpawn);
            }
        }

        private void SpawnPrefab(float x, float y, float z, int i, GameObject newPrefab)
        {
            newPrefab = Instantiate(newPrefab, transform, true);
            newPrefab.transform.localPosition = new Vector3(x, y, z);
            newPrefab.name = prefabName + "_" + (i + 1);
        }
    }
}
