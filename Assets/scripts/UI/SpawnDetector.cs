using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDetector : MonoBehaviour
{
    public GameObject PreviousSpawnPoint;
    public GameObject CurrentSpawnPoint;
    private bool didCollide = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true && didCollide == false)
        {
            PreviousSpawnPoint.SetActive(false);
            CurrentSpawnPoint.SetActive(true);
            didCollide = true;
        }
    }
}
