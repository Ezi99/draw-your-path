using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallManagement : ObjectManagement
{
    public GameObject leftFireBallLauncher;
    public GameObject rightFireBallLauncher;
    public ShieldManagement circle;
    public void SpawnFireBall(int pixelHits, int numOfDrawnPixels, ref Draw marker, bool didRightDraw)
    {
        int damage;
        float accuracy = (float)pixelHits / (float)numOfDrawnPixels;
        GameObject cloneFireBall;
        Transform markerPos = marker.gameObject.transform;
        Rigidbody rigidBody = null;

        if (didRightDraw == false)
        {
            SpawnLocation = leftFireBallLauncher.transform;

        }
        else
        {
            SpawnLocation = rightFireBallLauncher.transform;

        }

        checkNumOfItems();
        if (accuracy < 1)
        {
            if (accuracy <= 0.75)
            {
                damage = 30;
                cloneFireBall = Instantiate(Weak, markerPos.position, markerPos.rotation);
            }
            else
            {
                damage = 40;
                cloneFireBall = Instantiate(Regular, markerPos.position, markerPos.rotation);
            }
        }
        else
        {
            damage = 60;
            cloneFireBall = Instantiate(Strong, markerPos.position, markerPos.rotation);
        }

        rigidBody = cloneFireBall.GetComponent<Rigidbody>();
        cloneFireBall.GetComponent<FireBall>().SetStats(damage);
        rigidBody.AddForce(SpawnLocation.forward * 5000);
        ObjectList.Add(cloneFireBall);
    }

    public int CheckIfFireBall(ref DrawCanvas drawCanvas, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        int pixelHits = 0;
        pixelHits = circle.CheckIfShield(ref drawCanvas, ref highestCoord, ref lowestCoord, ref colors);
        Debug.Log($"THERE WAS {pixelHits} FIREBALL HITS");
        return pixelHits;
    }
}
