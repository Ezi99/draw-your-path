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

    public int CheckIfHammer(DrawCanvas drawCanvas, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        Texture2D hammer;
        int pixelHits = 0;
        hammer = drawHammer(drawCanvas.texture, ref pixelHits, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);
        Debug.Log("THERE WAS " + pixelHits + " HAMMER HITS");
        encodeDrawing2PNG("Hammer.png", ref hammer);
        totalPixelHitAttempt = 0;
        return pixelHits;
    }

    private Texture2D drawHammer(Texture2D drawCanvas, ref int pixelHits, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        int drawingLength = (lowestXCoord.x - highestXCoord.x);
        int middlePoint = lowestYCoord.y + (highestYCoord.y - lowestYCoord.y) / 2;
        int HeadWidth = highestYCoord.y - lowestYCoord.y;//(int)(drawingLength / 2.2f);
        int HeadLength = (int)(drawingLength / 2.5f);
        int handleThickness = HeadWidth / 8;
        int headThickness = HeadLength / 4;
        int startPoint = highestXCoord.x;// + 30;
        bool[] importantPoints = { false, false, false, false };

        Texture2D hammer = new Texture2D(textureSize, textureSize);

        for (int x = startPoint; x < lowestXCoord.x && x < textureSize; x += 15)
        {
            if (x <= startPoint + HeadLength && (x <= startPoint + headThickness || x >= startPoint + HeadLength - headThickness))
            {
                for (int y = lowestYCoord.y; y < textureSize && y <= highestYCoord.y; y += 15)
                {
                    totalPixelHitAttempt++;
                    hammer.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[0] = true;
                    }
                }
            }
            else if (x <= startPoint + HeadLength && (x > startPoint + headThickness || x < startPoint + HeadLength - headThickness))
            {
                for (int y = lowestYCoord.y; y < textureSize && y <= lowestYCoord.y + headThickness; y += 15)
                {
                    totalPixelHitAttempt++;
                    hammer.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[1] = true;
                    }
                }
                for (int y = highestYCoord.y - headThickness; y < textureSize && y <= highestYCoord.y; y += 15)
                {
                    totalPixelHitAttempt++;
                    hammer.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[2] = true;
                    }
                }
            }
            for (int y = middlePoint - handleThickness; y < textureSize && y <= middlePoint + handleThickness && x >= startPoint + HeadLength && x <= lowestXCoord.x; y += 15)
            {
                totalPixelHitAttempt++;
                hammer.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                {
                    importantPoints[3] = true;
                }
            }
        }

        Debug.Log($"DA TOTAL HIT HAMMER ATTEMPTS IS {totalPixelHitAttempt} HITS IS {pixelHits}");
        totalPixelHitAttempt /= 2;
        checkIfEnoughPixelHits(ref pixelHits, totalPixelHitAttempt, 0.4f);
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