using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    public string DestructorItemTag;
    public GameObject BrokenWall;
    public Transform BrokenWallPos;

    public void DestoryWall()
    {
        Instantiate(BrokenWall, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(DestructorItemTag) == true)
        {
            Instantiate(BrokenWall, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }*/
    /*private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider);
        if (collision.collider.CompareTag(DestructorItemTag) == true)
        {
            Instantiate(BrokenWall, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }*/

}
