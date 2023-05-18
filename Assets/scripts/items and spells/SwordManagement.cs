using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SwordManagement : ObjectManagement
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
                cloneSword = Instantiate(Weak, SpawnLocation.position, SpawnLocation.rotation);
            }
            else
            {
                damage = 35;
                cloneSword = Instantiate(Regular, SpawnLocation.position, SpawnLocation.rotation);
            }
        }
        else
        {
            Durability = 100;
            damage = 50;
            cloneSword = Instantiate(Strong, SpawnLocation.position, SpawnLocation.rotation);
        }

        cloneSword.GetComponent<Sword>().SetStats(damage, Durability);
        ObjectList.Add(cloneSword);
    }

    public int CheckIfSword(DrawCanvas drawCanvas, Coordinates highestCoord, Coordinates lowestCoord, Color[] colors)
    {
        Texture2D Sword;
        int pixelHits = 0;
        Sword = drawSword(drawCanvas.texture, ref pixelHits, highestCoord, lowestCoord, colors);
        Debug.Log("THERE WAS " + pixelHits + " SWORD HITS");
        encodeDrawing2PNG("Sword.png", ref Sword);
        totalPixelHitAttempt = 0;
        return pixelHits;
    }

    private Texture2D drawSword(Texture2D drawCanvas, ref int pixelHits, Coordinates highestCoord, Coordinates lowestCoord, Color[] colors)
    {
        Texture2D Sword = new Texture2D(textureSize, textureSize);
        int swordWidth = (lowestCoord.x - highestCoord.x) / 9;
        int handleLocation = (lowestCoord.x - highestCoord.x) / 8;
        int handleSize = (lowestCoord.x - highestCoord.x) / 3;
        bool[] importantPoints = { false, false, false, false, false };

        for (int x = highestCoord.x; x <= lowestCoord.x && x < textureSize - 30; x += 15)
        {
            if (x <= lowestCoord.x - handleLocation && x >= lowestCoord.x - 2 * handleLocation)// check if it's time to draw handle
            {
                for (int y = highestCoord.y - handleSize; y < textureSize && y <= highestCoord.y - handleSize + handleSize / 2; y += 15)
                {
                    totalPixelHitAttempt++;
                    Sword.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[0] = true;
                    }
                }
                for (int y = highestCoord.y + handleSize - handleSize / 2; y < textureSize && y <= highestCoord.y + handleSize; y += 15)
                {
                    totalPixelHitAttempt++;
                    Sword.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[1] = true;
                    }
                }
            }
            else if (x >= highestCoord.x && x <= highestCoord.x + 30 || x >= lowestCoord.x - 30)// check if it's time to ends of the sword
            {
                for (int y = highestCoord.y - swordWidth; y < textureSize && y <= highestCoord.y + swordWidth; y += 15)
                {
                    totalPixelHitAttempt++;
                    Sword.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[2] = true;
                    }
                }
            }
            else
            {
                for (int y = highestCoord.y - swordWidth; y < textureSize && y <= highestCoord.y - swordWidth + swordWidth / 2; y += 15)
                {
                    totalPixelHitAttempt++;
                    Sword.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[3] = true;
                    }
                }
                for (int y = highestCoord.y + swordWidth - swordWidth / 2; y < textureSize && y <= highestCoord.y + swordWidth; y += 15)
                {
                    totalPixelHitAttempt++;
                    Sword.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[4] = true;
                    }
                }
            }

        }

        Debug.Log($"DA TOTAL HIT SWORD ATTEMPTS IS {totalPixelHitAttempt} HITS IS {pixelHits}");
        for (int i = 0; i < importantPoints.Length; i++)
        {
            if (importantPoints[i] == false)
            {
                pixelHits = 0;
                Debug.Log("can't finesse us with sword");
            }
        }
        checkIfEnoughPixelHits(ref pixelHits, totalPixelHitAttempt, 0.4f);
        Sword.Apply();
        return Sword;
    }

}
