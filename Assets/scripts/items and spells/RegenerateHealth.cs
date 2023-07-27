using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RegenerateHealth : ObjectManagement
{
    public PlayerHealth m_PlayerHealth;

    public void SpawnHealth(int pixelHits)
    {
        int gain;
        float accuracy = (float)pixelHits / (float)m_TotalPixelHitAttempt;

        if (accuracy < 1)
        {
            if (accuracy <= 0.75)
            {
                gain = 25;
                Weak.SetActive(true);
            }
            else
            {
                gain = 35;
                Regular.SetActive(true);
            }
        }
        else
        {
            gain = 50;
            Strong.SetActive(true);
        }

        m_PlayerHealth.GainHealth(gain);
    }

    public int CheckIfHealth(DrawCanvas drawCanvas, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        Texture2D Health;
        int pixelHits = 0;
        Health = drawHealth(drawCanvas.texture, ref pixelHits, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);
        Debug.Log("THERE WAS " + pixelHits + " HEALTH HITS");
        encodeDrawing2PNG("Health.png", ref Health);
        return pixelHits;
    }

    private Texture2D drawHealth(Texture2D drawCanvas, ref int pixelHits, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        Texture2D Health = new Texture2D(textureSize, textureSize);
        Color[] yellow = Enumerable.Repeat(Color.yellow, 30 * 30).ToArray();

        int topY = lowestYCoord.y + (highestYCoord.y - lowestYCoord.y) / 2;
        int healthLength = lowestXCoord.x - highestXCoord.x;
        int healthThickness = healthLength / 17;
        int firstLine = 0, secondLine = 0;
        bool middlePoint = false;
        bool enteredFirstLoop;
        m_TotalPixelHitAttempt = 0;

        if (healthLength % 2 == 1)
        {
            healthLength++;
        }

        for (int x = highestXCoord.x; x <= lowestXCoord.x && x < textureSize; x += 15)
        {
            enteredFirstLoop = false;
            for (int y = lowestYCoord.y; y < textureSize && x <= highestXCoord.x + healthLength / 2 + healthThickness * 2 && x >= highestXCoord.x + healthLength / 2 - healthThickness * 2 && y <= highestYCoord.y; y += 15)
            {
                m_TotalPixelHitAttempt++;
                if (y >= topY - healthLength / 10 && y <= topY + healthLength / 10)
                {
                    Health.SetPixels(x, y, 30, 30, yellow);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        firstLine++;
                        middlePoint = true;
                    }
                }
                else
                {
                    Health.SetPixels(x, y, 30, 30, colors);
                    if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                    {
                        firstLine++;
                    }
                }

                enteredFirstLoop = true;
            }
            for (int y = topY - healthThickness; enteredFirstLoop == false && y < textureSize && y <= topY + healthThickness; y += 15)
            {
                m_TotalPixelHitAttempt++;
                Health.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                {
                    secondLine++;
                }
            }
        }

        float FloattotalPixelHitAttempt = m_TotalPixelHitAttempt / 1.5f;
        m_TotalPixelHitAttempt = (int)FloattotalPixelHitAttempt;
        int absDifference = Mathf.Abs(firstLine - secondLine);
        Debug.Log($"DA TOTAL HIT HEALTH ATTEMPTS IS {m_TotalPixelHitAttempt} HITS IS {pixelHits}");
        checkIfEnoughPixelHits(ref pixelHits, m_TotalPixelHitAttempt, 0.4f);
        if (absDifference > firstLine || absDifference > secondLine || middlePoint == false)//make sure player drew a decent plus and not just 1 straight line
        {
            pixelHits = 0;
            Debug.Log($"can't finesse us {absDifference} absDifference and middlePoint = {middlePoint} with health");
        }

        Health.Apply();
        return Health;
    }
}
