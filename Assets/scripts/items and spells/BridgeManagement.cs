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
                lifeSpan = 10;
            }
            else
            {
                bridgeClone = Instantiate(Regular, SpawnLocation.position, Quaternion.Euler(90, 90, 90));
                lifeSpan = 15;
            }
        }
        else
        {
            bridgeClone = Instantiate(Strong, SpawnLocation.position, Quaternion.Euler(90, 90, 90));
            lifeSpan = 20;
        }

        bridgeClone.GetComponent<Bridge>().DespawnCountDown(lifeSpan);
        ObjectList.Add(bridgeClone);
    }

    public int CheckIfBridge(ref DrawCanvas drawCanvas, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D Bridge;
        int pixelHits = 0;
        Bridge = drawBridge(ref drawCanvas.texture,ref pixelHits, ref highestCoord, ref lowestCoord, ref colors);
        Debug.Log("THERE WAS " + pixelHits + " BRIDGE HITS");
        encodeDrawing2PNG("Bridge.png", ref Bridge);
        return pixelHits;
    }

    private Texture2D drawBridge(ref Texture2D drawCanvas,ref int pixelHits, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        float bridgeWidth = (float)(lowestCoord.x - highestCoord.x) / 1.5f;
        Texture2D Bridge = new Texture2D(textureSize, textureSize);

        for (int x = highestCoord.x; x < lowestCoord.x && x < textureSize - 30; x += 30)
        {
            for (int y = 0; y < textureSize - 30; y += 10)
            {
                if (x >= highestCoord.x && x <= highestCoord.x + 30 || x >= lowestCoord.x - 30)
                {
                    if (y >= highestCoord.y - bridgeWidth && y <= highestCoord.y + bridgeWidth)
                    {
                        Bridge.SetPixels(x, y, 30, 30, colors);
                        isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                    }
                }
                else if (y >= highestCoord.y - bridgeWidth && y <= highestCoord.y - bridgeWidth + 30 || y <= highestCoord.y + bridgeWidth && y >= highestCoord.y + bridgeWidth - 30)
                {
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                }
            }
        }

        Bridge.Apply();
        return Bridge;
    }

}
