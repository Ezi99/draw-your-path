using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    public GameObject BrokenWall;
    public Transform BrokenWallPos;

    public void DestoryWall()
    {
        Instantiate(BrokenWall, BrokenWallPos.position, BrokenWallPos.rotation);
        Destroy(gameObject);
    }
}
