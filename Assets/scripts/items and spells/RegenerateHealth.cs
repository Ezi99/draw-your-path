using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RegenerateHealth : ObjectManagement
{
    public PlayerHealth m_PlayerHealth;

    private void Start()
    {
        totalPixelHitAttempt = 0;
    }

    public void SpawnHealth(int pixelHits, int numOfDrawnPixels)
    {
        int gain;
        float accuracy = (float)pixelHits / (float)numOfDrawnPixels;

        if (accuracy < 1)
        {
            if (accuracy <= 0.75)
            {
                gain = 25;
            }
            else
            {
                gain = 35;
            }
        }
        else
        {
            gain = 50;
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
        totalPixelHitAttempt = 0;
        return pixelHits;
    }

    private Texture2D drawHealth(Texture2D drawCanvas, ref int pixelHits, Coordinates highestXCoord, Coordinates lowestXCoord, Coordinates highestYCoord, Coordinates lowestYCoord, Color[] colors)
    {
        Texture2D Health = new Texture2D(textureSize, textureSize);
        Color[] yellow = Enumerable.Repeat(Color.yellow, 30 * 30).ToArray();

        int topY = lowestYCoord.y + (highestYCoord.y - lowestYCoord.y) / 2;
        int healthLength = lowestXCoord.x - highestXCoord.x;
        int healthThickness = healthLength / 15;
        int firstLine = 0, secondLine = 0;
        bool middlePoint = false;
        bool enteredFirstLoop;

        if (healthLength % 2 == 1)
        {
            healthLength++;
        }

        for (int x = highestXCoord.x; x <= lowestXCoord.x && x < textureSize; x += 15)
        {
            enteredFirstLoop = false;
            for (int y = lowestYCoord.y; y < textureSize && x <= highestXCoord.x + healthLength / 2 + healthThickness * 2 && x >= highestXCoord.x + healthLength / 2 - healthThickness * 2 && y <= highestYCoord.y; y += 15)
            {
                totalPixelHitAttempt++;
                if (y >= topY - healthLength / 8 && y <= topY + healthLength / 8)
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
                totalPixelHitAttempt++;
                Health.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                {
                    secondLine++;
                }
            }
        }

        int absDifference = Mathf.Abs(firstLine - secondLine);
        Debug.Log($"DA TOTAL HIT HEALTH ATTEMPTS IS {totalPixelHitAttempt} HITS IS {pixelHits}");
        checkIfEnoughPixelHits(ref pixelHits, totalPixelHitAttempt, 0.4f);
        if (absDifference > firstLine || absDifference > secondLine || middlePoint == false)//make sure player drew a decent plus and not just 1 straight line
        {
            pixelHits = 0;
            Debug.Log($"can't finesse us {absDifference} absDifference and middlePoint = {middlePoint} with health");
        }

        Health.Apply();
        return Health;
    }
}
