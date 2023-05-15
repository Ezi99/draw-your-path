using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    public GameObject BrokenWall;
    public Transform BrokenWallPos;
    private int health = 100;

    public void TakeDamage(int damage, int velocity)
    {
        if (velocity >= 5)
        {
            health -= damage;
            if (health <= 0)
            {
                Debug.Log("WALL OUT");
                Instantiate(BrokenWall, BrokenWallPos.position, BrokenWallPos.rotation);
                Destroy(gameObject);
            }
        }
    }
}
