using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeManagement : ItemManagement // no need for Bridge class
{
    private int lifeSpan;
    public void SpawnBridge(int pixelHits, int numOfDrawnPixels)
    {
        float accuracy = (float)pixelHits / (float)numOfDrawnPixels;
        GameObject bridgeClone;
        
        checkNumOfItems(1);
        if (accuracy < 1)
        {
            if (accuracy <= 0.75)
            {
                bridgeClone = Instantiate(WeakItem, ItemSpawn.position, Quaternion.Euler(90, 90, 90));
                lifeSpan = 10;
            }
            else
            {
                bridgeClone = Instantiate(RegularItem, ItemSpawn.position, Quaternion.Euler(90, 90, 90));
                lifeSpan = 15;
            }
        }
        else
        {
            bridgeClone = Instantiate(StrongItem, ItemSpawn.position, Quaternion.Euler(90, 90, 90));
            lifeSpan = 20;
        }

        Invoke("Despawn", lifeSpan);
        ItemList.Add(bridgeClone);
    }

    private void Despawn()
    {
        checkNumOfItems(1);
    }
}
