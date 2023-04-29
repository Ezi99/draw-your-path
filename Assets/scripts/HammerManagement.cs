using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerManagement : ObjectManagement
{
    public void SpawnHammer(int pixelHits, int numOfDrawnPixels)
    {
        int damage;
        int Durability;
        float accuracy = (float)pixelHits / (float)numOfDrawnPixels;
        GameObject cloneHammer;

        checkNumOfItems();
        if (accuracy < 1)
        {
            if (accuracy <= 0.75)
            {
                Durability = 30;
                damage = 30;
                cloneHammer = Instantiate(Weak, SpawnLocation.position, SpawnLocation.rotation);
            }
            else
            {
                Durability = 50;
                damage = 50;
                cloneHammer = Instantiate(Regular, SpawnLocation.position, SpawnLocation.rotation);
            }
        }
        else
        {
            Durability = 100;
            damage = 100;
            cloneHammer = Instantiate(Strong, SpawnLocation.position, SpawnLocation.rotation);
        }

        cloneHammer.GetComponent<Hammer>().SetStats(damage, Durability);
        ObjectList.Add(cloneHammer);
    }

    public int CheckIfHammer(ref DrawCanvas drawCanvas, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D hammer;
        int pixelHits = 0;
        hammer = drawHammer(ref drawCanvas.texture, ref pixelHits, ref highestCoord, ref lowestCoord, ref colors);
        Debug.Log("THERE WAS " + pixelHits + " HAMMER HITS");
        encodeDrawing2PNG("Hammer.png", ref hammer);
        return pixelHits;
    }

    private Texture2D drawHammer(ref Texture2D drawCanvas, ref int pixelHits, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        int drawingLength = (lowestCoord.x - highestCoord.x);
        int HeadWidth = drawingLength / 4;
        int HeadLength = drawingLength / 5;
        int handleWidth = HeadWidth / 8;
        bool importantPoint1 = false;
        bool importantPoint2 = false;
        bool importantPoint3 = false;
        bool importantPoint4 = false;

        Texture2D hammer = new Texture2D(textureSize, textureSize);

        for (int x = highestCoord.x; x < lowestCoord.x && x < textureSize; x += 10)
        {
            for (int y = highestCoord.y - HeadWidth; y < textureSize && y <= highestCoord.y + HeadWidth && x <= highestCoord.x + HeadLength && (x <= highestCoord.x + HeadLength / 5 || x >= highestCoord.x + HeadLength - HeadLength / 5); y += 15)
            {
                hammer.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y, ref pixelHits, ref drawCanvas) == true)
                {
                    importantPoint1 = true;
                }
            }
            if (x <= highestCoord.x + HeadLength && (x > highestCoord.x + HeadLength / 5 || x < highestCoord.x + HeadLength - HeadLength / 5))
            {
                for (int y = highestCoord.y - HeadWidth; y < textureSize && y <= highestCoord.y - HeadWidth + HeadWidth / 5; y += 15)
                {
                    hammer.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, ref drawCanvas) == true)
                    {
                        importantPoint2 = true;
                    }
                }
                for (int y = highestCoord.y + HeadWidth - HeadWidth / 5; y < textureSize && y <= highestCoord.y + HeadWidth; y += 15)
                {
                    hammer.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, ref drawCanvas) == true)
                    {
                        importantPoint3 = true;
                    }
                }
            }
            for (int y = highestCoord.y - handleWidth; y < textureSize && y <= highestCoord.y + handleWidth && x >= highestCoord.x + HeadLength && x <= lowestCoord.x; y += 15)
            {
                hammer.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y, ref pixelHits, ref drawCanvas) == true)
                {
                    importantPoint4 = true;
                }
            }
        }

        if (importantPoint1 == false || importantPoint2 == false || importantPoint3 == false || importantPoint4 == false)
        {
            pixelHits = 0;
            Debug.Log("can't finesse us with hammer");
        }

        hammer.Apply();
        return hammer;
    }
}