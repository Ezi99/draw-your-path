using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallManagement : ObjectManagement
{
    public GameObject leftFireBallLauncher;
    public GameObject rightFireBallLauncher;
    public ShieldManagement circle;

    public void SpawnFireBall(int pixelHits, Draw marker, bool didHandRightDraw)
    {
        int damage;
        float accuracy = (float)pixelHits / (float)m_TotalPixelHitAttempt;
        GameObject cloneFireBall;
        Transform markerPos = marker.gameObject.transform;
        Rigidbody rigidBody;

        if (didHandRightDraw == false)
        {
            SpawnLocation = leftFireBallLauncher.transform;

        }
        else
        {
            SpawnLocation = rightFireBallLauncher.transform;

        }

        checkNumOfItems();
        if (accuracy <= 0.75)
        {
            damage = 30;
            cloneFireBall = Instantiate(Weak, markerPos.position, markerPos.rotation);
        }
        else if (accuracy < 1)
        {
            damage = 50;
            cloneFireBall = Instantiate(Regular, markerPos.position, markerPos.rotation);
        }
        else
        {
            damage = 100;
            cloneFireBall = Instantiate(Strong, markerPos.position, markerPos.rotation);
        }

        rigidBody = cloneFireBall.GetComponent<Rigidbody>();
        cloneFireBall.GetComponent<FireBall>().SetStats(damage);
        rigidBody.AddForce(SpawnLocation.forward * 3000);
        ObjectList.Add(cloneFireBall);
    }

    public int CheckIfFireBall(DrawCanvas drawCanvas, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        int pixelHits;
        pixelHits = circle.CheckIfShield(drawCanvas, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);
        m_TotalPixelHitAttempt = circle.TotalPixelHitAttempt;
        m_HitPercentage = circle.HitPercentage;
        Debug.Log($"THERE WAS {pixelHits} FIREBALL HITS");
        return pixelHits;
    }
}