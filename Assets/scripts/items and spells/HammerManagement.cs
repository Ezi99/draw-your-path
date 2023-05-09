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

    public int CheckIfHammer(DrawCanvas drawCanvas, Coordinates highestCoord, Coordinates lowestCoord, Color[] colors)
    {
        Texture2D hammer;
        int pixelHits = 0;
        hammer = drawHammer(drawCanvas.texture, ref pixelHits, highestCoord, lowestCoord, colors);
        Debug.Log("THERE WAS " + pixelHits + " HAMMER HITS");
        encodeDrawing2PNG("Hammer.png", ref hammer);
        return pixelHits;
    }

    private Texture2D drawHammer(Texture2D drawCanvas, ref int pixelHits, Coordinates highestCoord, Coordinates lowestCoord, Color[] colors)
    {
        int drawingLength = (lowestCoord.x - highestCoord.x);
        int HeadWidth = drawingLength / 3;
        int HeadLength = drawingLength / 3;
        int handleThickness = HeadWidth / 8;
        int headThickness = HeadLength / 4;
        bool[] importantPoints = { false, false, false, false };

        Texture2D hammer = new Texture2D(textureSize, textureSize);

        for (int x = highestCoord.x; x < lowestCoord.x && x < textureSize; x += 15)
        {
            if (x <= highestCoord.x + HeadLength && (x <= highestCoord.x + headThickness || x >= highestCoord.x + HeadLength - headThickness))
            {
                for (int y = highestCoord.y - HeadWidth; y < textureSize && y <= highestCoord.y + HeadWidth; y += 15)
                {
                    hammer.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[0] = true;
                    }
                }
            }
            else if (x <= highestCoord.x + HeadLength && (x > highestCoord.x + headThickness || x < highestCoord.x + HeadLength - headThickness))
            {
                for (int y = highestCoord.y - HeadWidth; y < textureSize && y <= highestCoord.y - HeadWidth + headThickness; y += 15)
                {
                    hammer.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[1] = true;
                    }
                }
                for (int y = highestCoord.y + HeadWidth - headThickness; y < textureSize && y <= highestCoord.y + HeadWidth; y += 15)
                {
                    hammer.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[2] = true;
                    }
                }
            }
            for (int y = highestCoord.y - handleThickness; y < textureSize && y <= highestCoord.y + handleThickness && x >= highestCoord.x + HeadLength && x <= lowestCoord.x; y += 15)
            {
                hammer.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y,ref pixelHits, drawCanvas) == true)
                {
                    importantPoints[3] = true;
                }
            }
        }

        for (int i = 0; i < importantPoints.Length; i++)
        {
            if (importantPoints[i] == false)
            {
                pixelHits = 0;
                Debug.Log("can't finesse us with hammer");
            }
        }

        hammer.Apply();
        return hammer;
    }
}