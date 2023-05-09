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
        int bridgeWidth = (int)(drawingLength / 1.5f);
        int bridgeThickness = drawingLength / 10;
        Texture2D Bridge = new Texture2D(textureSize, textureSize);

        for (int x = highestCoord.x; x < lowestCoord.x && x < textureSize; x += 10)
        {
            if (x >= highestCoord.x && x <= highestCoord.x + bridgeThickness || x >= lowestCoord.x - bridgeThickness)
            {
                for (int y = highestCoord.y - bridgeWidth; y < textureSize && y <= highestCoord.y + bridgeWidth; y += 15)
                {
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, drawCanvas);
                }
            }
            else
            {
                for (int y = highestCoord.y - bridgeWidth; y < textureSize && y <= highestCoord.y - bridgeWidth + bridgeThickness; y += 10)
                {
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, drawCanvas);
                }
                for (int y = highestCoord.y + bridgeWidth - bridgeThickness; y < textureSize && y <= highestCoord.y + bridgeWidth; y += 10)
                {
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, drawCanvas);
                }
            }
        }

        Bridge.Apply();
        return Bridge;
    }
}
