using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeManagement : ObjectManagement
{
    private int lifeSpan;

    public void SpawnBridge(int pixelHits)
    {
        float accuracy = (float)pixelHits / (float)m_TotalPixelHitAttempt;
        GameObject bridgeClone;

        checkNumOfItems(1);

        if (accuracy <= 0.75)
        {
            bridgeClone = Instantiate(Weak, SpawnLocation.position, Quaternion.Euler(90, 90, 90));
            lifeSpan = 30;
        }
        else if (accuracy < 1)
        {
            bridgeClone = Instantiate(Regular, SpawnLocation.position, Quaternion.Euler(90, 90, 90));
            lifeSpan = 60;
        }
        else
        {
            bridgeClone = Instantiate(Strong, SpawnLocation.position, Quaternion.Euler(90, 90, 90));
            lifeSpan = 90;
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
        return pixelHits;
    }

    private Texture2D drawBridge(Texture2D drawCanvas, ref int pixelHits, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        int drawingLength = lowestXCoord.x - highestXCoord.x;
        Debug.Log("da bridge length is " + drawingLength);
        int bridgeThickness = drawingLength / 10;
        Texture2D Bridge = new Texture2D(textureSize, textureSize);
        bool[] importantPoints = { false, false, false, false };
        m_TotalPixelHitAttempt = 0;

        for (int x = highestXCoord.x; x < lowestXCoord.x && x < textureSize; x += 15)
        {
            if (x <= highestXCoord.x + bridgeThickness || x >= lowestXCoord.x - bridgeThickness)
            {
                for (int y = lowestYCoord.y; y < textureSize && y <= highestYCoord.y; y += 20)
                {
                    m_TotalPixelHitAttempt++;
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        if (x >= highestXCoord.x && x <= highestXCoord.x + bridgeThickness)
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
                for (int y = lowestYCoord.y; y < textureSize && y <= lowestYCoord.y + bridgeThickness; y += 15)
                {
                    m_TotalPixelHitAttempt++;
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        importantPoints[2] = true;
                    }
                }
                for (int y = highestYCoord.y - bridgeThickness; y < textureSize && y <= highestYCoord.y; y += 15)
                {
                    m_TotalPixelHitAttempt++;
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

        float FloatTotalPixelHitAttempt = m_TotalPixelHitAttempt / 1.5f;
        m_TotalPixelHitAttempt = (int)FloatTotalPixelHitAttempt;
        Debug.Log($"DA TOTAL HIT BRIDGE ATTEMPTS IS {m_TotalPixelHitAttempt} HITS IS {pixelHits}");
        checkIfEnoughPixelHits(ref pixelHits, m_TotalPixelHitAttempt, 0.4f);
        m_HitPercentage = (float)pixelHits / (float)m_TotalPixelHitAttempt;
        Bridge.Apply();
        return Bridge;
    }
}
