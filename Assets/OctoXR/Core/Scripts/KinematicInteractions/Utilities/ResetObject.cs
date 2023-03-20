using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OctoXR.KinematicInteractions.Utilities
{
    public class ResetObject : MonoBehaviour
    {
        private bool hasCollided;
        [SerializeField] private PrefabSpawner prefabSpawner;
        public PrefabSpawner PrefabSpawner { get => prefabSpawner; set => prefabSpawner = value; }

        [SerializeField] private float respawnTime;
        public float RespawnTime { get => respawnTime; set => respawnTime = value; }

        [SerializeField] private GameObject particle;
        public GameObject Particle { get => particle; set => particle = value; }

        public UnityEvent OnReset;
    }
}
