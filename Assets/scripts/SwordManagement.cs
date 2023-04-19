using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SwordManagement : ItemManagement
{
    private int textureSize = 1024;

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

    public int CheckIfSword(ref DrawCanvas drawCanvas, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D Sword;
        int pixelHits = 0;
        Sword = drawSword(ref drawCanvas.texture, ref pixelHits, ref highestCoord, ref lowestCoord, ref colors);
        Debug.Log("THERE WAS " + pixelHits + " SWORD HITS");
        encodeDrawing2PNG("Sword.png", ref Sword);
        return pixelHits;
    }

    private Texture2D drawSword(ref Texture2D drawCanvas, ref int pixelHits, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D Sword = new Texture2D(textureSize, textureSize);
        int swordWidth = (lowestCoord.x - highestCoord.x) / 9;
        int handleLocation = (lowestCoord.x - highestCoord.x) / 8;
        int handleSize = (lowestCoord.x - highestCoord.x) / 3;

        for (int x = highestCoord.x; x <= lowestCoord.x && x < textureSize - 30; x += 15)
        {
            for (int y = 0; y < textureSize - 30; y += 15)
            {
                if (x <= lowestCoord.x - handleLocation && x >= lowestCoord.x - 2 * handleLocation)// check if it's time to draw handle
                {
                    if (y >= highestCoord.y - handleSize && y <= highestCoord.y + handleSize)
                    {
                        if (y <= highestCoord.y - handleSize + handleSize / 2 || y >= highestCoord.y + handleSize - handleSize / 2)
                        {
                            Sword.SetPixels(x, y, 30, 30, colors);
                            isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                        }
                    }
                }
                else if (y >= highestCoord.y - swordWidth && y <= highestCoord.y + swordWidth)
                {
                    if (x >= highestCoord.x && x <= highestCoord.x + 30 || x >= lowestCoord.x - 30)
                    {
                        Sword.SetPixels(x, y, 30, 30, colors);
                        isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                    }
                    else if (y <= highestCoord.y - swordWidth + swordWidth / 2 || y >= highestCoord.y + swordWidth - swordWidth / 2)
                    {
                        Sword.SetPixels(x, y, 30, 30, colors);
                        isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                    }
                }
            }
        }

        Sword.Apply();
        return Sword;
    }

}
