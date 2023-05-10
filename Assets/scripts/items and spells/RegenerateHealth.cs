using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RegenerateHealth : ObjectManagement
{
    public PlayerHealth m_PlayerHealth;

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

    public int CheckIfHealth(DrawCanvas drawCanvas, Coordinates highestCoord, Coordinates lowestCoord, Color[] colors)
    {
        Texture2D Health;
        int pixelHits = 0;
        Health = drawHealth(drawCanvas.texture, ref pixelHits, highestCoord, lowestCoord, colors);
        Debug.Log("THERE WAS " + pixelHits + " HEALTH HITS");
        encodeDrawing2PNG("Health.png", ref Health);
        return pixelHits;
    }

    private Texture2D drawHealth(Texture2D drawCanvas, ref int pixelHits, Coordinates highestCoord, Coordinates lowestCoord, Color[] colors)
    {
        Texture2D Health = new Texture2D(textureSize, textureSize);
        Color[] yellow = Enumerable.Repeat(Color.yellow, 30 * 30).ToArray();
        int healthLength = lowestCoord.x - highestCoord.x;
        int healthWidth = healthLength / 15;
        int firstLine = 0, secondLine = 0;
        bool middlePoint = false;
        bool enteredFirstLoop;

        if (healthLength % 2 == 1)
        {
            healthLength++;
        }

        for (int x = highestCoord.x; x <= lowestCoord.x && x < textureSize; x += 15)
        {
            enteredFirstLoop = false;
            for (int y = highestCoord.y - healthLength / 2; y < textureSize && x <= highestCoord.x + healthLength / 2 + healthWidth * 2 && x >= highestCoord.x + healthLength / 2 - healthWidth * 2 && y <= highestCoord.y + healthLength / 2; y += 15)
            {
                if (y >= highestCoord.y - healthLength / 8 && y <= highestCoord.y + healthLength / 8)
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
            for (int y = highestCoord.y - healthWidth; enteredFirstLoop == false && y < textureSize && y <= highestCoord.y + healthWidth; y += 15)
            {
                Health.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y, ref pixelHits, drawCanvas) == true)
                {
                    secondLine++;
                }
            }
        }

        int absDifference = Mathf.Abs(firstLine - secondLine);
        if (absDifference > firstLine || absDifference > secondLine || middlePoint == false)//make sure player drew a decent plus and not just 1 straight line
        {
            pixelHits = 0;
            Debug.Log($"can't finesse us {absDifference} absDifference and middlePoint = {middlePoint} with health");
        }

        Health.Apply();
        return Health;
    }
}
