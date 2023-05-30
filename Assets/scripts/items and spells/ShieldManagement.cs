using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShieldManagement : ObjectManagement
{
    public void SpawnShield(int pixelHits)
    {
        int Durability;
        float accuracy = (float)pixelHits / (float)m_TotalPixelHitAttempt;
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

    public int CheckIfShield(DrawCanvas drawCanvas, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        Texture2D Shield;
        int pixelHits = 0;
        Shield = drawShield(drawCanvas.texture, ref pixelHits, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);
        Debug.Log("THERE WAS " + pixelHits + " SHIELD HITS");
        encodeDrawing2PNG("Circle.png", ref Shield);
        return pixelHits;
    }

    private Texture2D drawShield(Texture2D drawCanvas, ref int pixelHits, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        Texture2D shield = new Texture2D(textureSize, textureSize);
        float yRadius = (highestYCoord.y - lowestYCoord.y) / 2f;
        float xRadius = (lowestXCoord.x - highestXCoord.x) / 2f;
        float centerY = lowestYCoord.y + yRadius;
        float centerX = highestXCoord.x + xRadius;// added 30 to improve player's chances
        int circleThickess = (lowestXCoord.x - highestXCoord.x) / 9;

        for (int x = highestXCoord.x; x <= lowestXCoord.x + 30 && x < textureSize - 30; x += 10)
        {
            for (int y = 0; y < textureSize - 30; y += 15)
            {
                float distanceToCenter = Mathf.Sqrt(Mathf.Pow(y - centerY, 2) + Mathf.Pow(x - centerX, 2));
                if (distanceToCenter <= xRadius && distanceToCenter >= xRadius - circleThickess)//putting distanceToCenter == radius brings more accurate pixel hits but low pixel hits overall
                {
                    m_TotalPixelHitAttempt++;
                    shield.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, drawCanvas);
                }
                
            }
        }

        float FloatTotalPixelHitAttempt = m_TotalPixelHitAttempt / 1.5f;
        m_TotalPixelHitAttempt = (int)FloatTotalPixelHitAttempt;
        Debug.Log($"DA TOTAL HIT SHIELD ATTEMPTS IS {m_TotalPixelHitAttempt} HITS IS {pixelHits}");
        m_HitPercentage = (float)pixelHits / (float)m_TotalPixelHitAttempt;
        checkIfEnoughPixelHits(ref pixelHits, m_TotalPixelHitAttempt, 0.5f);
        shield.Apply();
        return shield;
    }
}
