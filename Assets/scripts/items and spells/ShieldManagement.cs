using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShieldManagement : ObjectManagement
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
                cloneShield = Instantiate(Weak, SpawnLocation.position, SpawnLocation.rotation);
            }
            else
            {
                cloneShield = Instantiate(Regular, SpawnLocation.position, SpawnLocation.rotation);
            }
        }
        else
        {
            Durability = 100;
            cloneShield = Instantiate(Strong, SpawnLocation.position, SpawnLocation.rotation);
        }

        cloneShield.GetComponent<Shield>().SetStats(Durability);
        ObjectList.Add(cloneShield);
    }

    public int CheckIfShield(ref DrawCanvas drawCanvas, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D Shield;
        int pixelHits = 0;
        Shield = drawShield(ref drawCanvas.texture, ref pixelHits, ref highestCoord, ref lowestCoord, ref colors);
        Debug.Log("THERE WAS " + pixelHits + " SHIELD HITS");
        encodeDrawing2PNG("Circle.png", ref Shield);
        return pixelHits;
    }

    private Texture2D drawShield(ref Texture2D drawCanvas, ref int pixelHits, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D shield = new Texture2D(textureSize, textureSize);
        float radius = (lowestCoord.x - highestCoord.x) / 2f;
        float centerY = highestCoord.y;
        float centerX = highestCoord.x + radius + 30;// added 30 to improve player's chances
        int circleThickess = (lowestCoord.x - highestCoord.x) / 12;

        for (int x = highestCoord.x; x <= lowestCoord.x + 30 && x < textureSize - 30; x += 15)
        {
            for (int y = 0; y < textureSize - 30; y += 15)
            {
                float distanceToCenter = Mathf.Sqrt(Mathf.Pow(y - centerY, 2) + Mathf.Pow(x - centerX, 2));
                if (distanceToCenter <= radius && distanceToCenter >= radius - circleThickess)//putting distanceToCenter == radius brings more accurate pixel hits but low pixel hits overall
                {
                    shield.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                }
            }
        }

        shield.Apply();
        return shield;
    }
}
