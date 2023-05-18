using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeManagement : ObjectManagement // no need for Bridge class
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
                bridgeClone = Instantiate(Weak, SpawnLocation.position, Quaternion.Euler(90, 90, 90));
                lifeSpan = 20;
            }
            else
            {
                bridgeClone = Instantiate(Regular, SpawnLocation.position, Quaternion.Euler(90, 90, 90));
                lifeSpan = 30;
            }
        }
        else
        {
            bridgeClone = Instantiate(Strong, SpawnLocation.position, Quaternion.Euler(90, 90, 90));
            lifeSpan = 40;
        }

        bridgeClone.GetComponent<Bridge>().DespawnCountDown(lifeSpan);
        ObjectList.Add(bridgeClone);
    }

    public int CheckIfBridge(DrawCanvas drawCanvas, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        Texture2D Bridge;
        int pixelHits = 0;
        Bridge = drawBridge(drawCanvas.texture, ref pixelHits, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);
        Debug.Log("THERE WAS " + pixelHits + " BRIDGE HITS");
        encodeDrawing2PNG("Bridge.png", ref Bridge);
        totalPixelHitAttempt = 0;
        return pixelHits;
    }

    private Texture2D drawBridge(Texture2D drawCanvas, ref int pixelHits, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        int drawingLength = lowestXCoord.x - highestXCoord.x;
        int bottomY = lowestYCoord.y;
        int topY = (int)(highestXCoord.y + (highestXCoord.y - bottomY) * 1.1f);
        int bridgeWidth = (int)(drawingLength / 1.5);
        int bridgeThickness = drawingLength / 12;
        int startPoint = highestXCoord.x + 30;
        int endPoint = lowestXCoord.x - 30;
        Texture2D Bridge = new Texture2D(textureSize, textureSize);
        bool[] importantPoints = { false, false, false, false };

        for (int x = startPoint; x < endPoint && x < textureSize; x += 15)
        {
            if (x <= startPoint + bridgeThickness || x >= endPoint - bridgeThickness)
            {
                for (int y = bottomY; y < textureSize && y <= topY; y += 15)
                {
                    totalPixelHitAttempt++;
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        if (x >= startPoint && x <= startPoint + bridgeThickness)
                        {
                            importantPoints[0] = true;
                        }
                        else
                        {
                            importantPoints[1] = true;
                        }
                    }

                }
            }
            else
            {
                for (int y = bottomY; y < textureSize && y <= bottomY + bridgeThickness; y += 15)
                {
                    totalPixelHitAttempt++;
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[2] = true;
                    }
                }
                for (int y = topY - bridgeThickness; y < textureSize && y <= topY; y += 15)
                {
                    totalPixelHitAttempt++;
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[3] = true;
                    }
                }
            }
        }

        for (int i = 0; i < importantPoints.Length; i++)
        {
            if (importantPoints[i] == false)
            {
                pixelHits = 0;
                Debug.Log("can't finnese us in bridge");
            }
        }

        Debug.Log($"DA TOTAL HIT BRIDGE ATTEMPTS IS {totalPixelHitAttempt} HITS IS {pixelHits}");
        checkIfEnoughPixelHits(ref pixelHits, totalPixelHitAttempt, 0.4f);
        Bridge.Apply();
        return Bridge;
    }
}
