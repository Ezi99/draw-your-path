using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SwordManagement : ItemManagement
{
    public void SpawnSword(int pixelHits, int numOfDrawnPixels)
    {
        int damage;
        int Durability;
        float accuracy = (float)pixelHits / (float)numOfDrawnPixels;
        GameObject cloneSword;

        checkNumOfItems();
        if (accuracy < 1)
        {
            Durability = (int)accuracy * 100;
            if (accuracy <= 0.75)
            {
                damage = 25;
                cloneSword = Instantiate(WeakItem, ItemSpawn.position, Quaternion.Euler(180, 0, 0));
            }
            else
            {
                damage = 35;
                cloneSword = Instantiate(RegularItem, ItemSpawn.position, Quaternion.Euler(180, 0, 0));
            }
        }
        else
        {
            Durability = 100;
            damage = 50;
            cloneSword = Instantiate(StrongItem, ItemSpawn.position, Quaternion.Euler(180, 0, 0));
        }

        cloneSword.GetComponent<Sword>().SetStats(damage, Durability);
        ItemList.Add(cloneSword);
    }
}
