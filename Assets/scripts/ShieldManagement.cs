using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManagement : ItemManagement
{
    public void SpawnShield(int pixelHits, int numOfDrawnPixels)
    {
        int Durability;
        float accuracy = (float)pixelHits / (float)numOfDrawnPixels;
        GameObject cloneShield;

        checkNumOfItems();
        if (accuracy < 1)
        {
            Durability = (int)accuracy * 100;
            if (accuracy <= 0.75)
            {
                cloneShield = Instantiate(WeakItem, ItemSpawn.position, Quaternion.Euler(90, 90, 90));
            }
            else
            {
                cloneShield = Instantiate(RegularItem, ItemSpawn.position, Quaternion.Euler(90, 90, 90));
            }
        }
        else
        {
            Durability = 100;
            cloneShield = Instantiate(StrongItem, ItemSpawn.position, Quaternion.Euler(90, 90, 90));
        }

        cloneShield.GetComponent<Shield>().SetStats(Durability);
        ItemList.Add(cloneShield);
    }

}
