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

    public int CheckIfBridge(DrawCanvas drawCanvas, Coordinates highestCoord, Coordinates lowestCoord, Color[] colors)
    {
        Texture2D Bridge;
        int pixelHits = 0;
        Bridge = drawBridge(drawCanvas.texture, ref pixelHits, highestCoord, lowestCoord, colors);
        Debug.Log("THERE WAS " + pixelHits + " BRIDGE HITS");
        encodeDrawing2PNG("Bridge.png", ref Bridge);
        return pixelHits;
    }

    private Texture2D drawBridge(Texture2D drawCanvas, ref int pixelHits, Coordinates highestCoord, Coordinates lowestCoord, Color[] colors)
    {
        int drawingLength = lowestCoord.x - highestCoord.x;
        int bridgeWidth = (int)(drawingLength / 1.5);
        int bridgeThickness = drawingLength / 12;
        int startPoint = highestCoord.x + 30;
        int endPoint = lowestCoord.x - 30;
        Texture2D Bridge = new Texture2D(textureSize, textureSize);
        bool[] importantPoints = { false, false, false, false };

        for (int x = startPoint; x < endPoint && x < textureSize; x += 15)
        {
            if (x <= startPoint + bridgeThickness || x >= endPoint - bridgeThickness)
            {
                for (int y = highestCoord.y - bridgeWidth; y < textureSize && y <= highestCoord.y + bridgeWidth; y += 15)
                {
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
                for (int y = highestCoord.y - bridgeWidth; y < textureSize && y <= highestCoord.y - bridgeWidth + bridgeThickness; y += 15)
                {
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[2] = true;
                    }
                }
                for (int y = highestCoord.y + bridgeWidth - bridgeThickness; y < textureSize && y <= highestCoord.y + bridgeWidth; y += 15)
                {
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[3] = true;
                    }
                }
            }
        }

        for(int i = 0; i < importantPoints.Length;i++)
        {
            if (importantPoints[i] == false)
            {
                pixelHits = 0;
                Debug.Log("can't finnese us in bridge");
            }
        }

        Bridge.Apply();
        return Bridge;
    }
}
