using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int CheckIfHealth(ref DrawCanvas drawCanvas, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D Health;
        int pixelHits = 0;
        Health = drawHealth(ref drawCanvas.texture, ref pixelHits, ref highestCoord, ref lowestCoord, ref colors);
        Debug.Log("THERE WAS " + pixelHits + " HEALTH HITS");
        encodeDrawing2PNG("Health.png", ref Health);
        return pixelHits;
    }

    private Texture2D drawHealth(ref Texture2D drawCanvas, ref int pixelHits, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D Health = new Texture2D(textureSize, textureSize);
        int healthLength = lowestCoord.x - highestCoord.x;
        int healthWidth = healthLength / 15;
        int firstLine = 0, secondLine = 0;
        Debug.Log("da width is " + healthWidth);
        bool enteredFirstLoop = false;

        if (healthLength % 2 == 1)
        {
            healthLength++;
        }

        for (int x = highestCoord.x; x <= lowestCoord.x && x < textureSize; x += 15)
        {
            enteredFirstLoop = false;
            for (int y = highestCoord.y - healthLength / 2; y < textureSize && x <= highestCoord.x + healthLength / 2 + healthWidth * 2 && x >= highestCoord.x + healthLength / 2 - healthWidth * 2 && y <= highestCoord.y + healthLength / 2; y += 15)
            {
                Health.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y, ref pixelHits, ref drawCanvas))
                {
                    firstLine++;
                }
                enteredFirstLoop = true;
            }
            for (int y = highestCoord.y - healthWidth; enteredFirstLoop == false && y < textureSize && y <= highestCoord.y + healthWidth; y += 15)
            {
                Health.SetPixels(x, y, 30, 30, colors);
                if (isPixelSet(x, y, ref pixelHits, ref drawCanvas))
                {
                    secondLine++;
                }
            }
            /*
            for (int y = 0; y < textureSize; y += 15)
            {
                if (y >= highestCoord.y - 30 && y <= highestCoord.y + 30)
                {
                    Health.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                }
                else if (x <= highestCoord.x + healthLength / 2 + 20 && x >= highestCoord.x + healthLength / 2 - 20 && y >= highestCoord.y - healthLength / 2 && y <= highestCoord.y + healthLength / 2)
                {
                    Health.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                }
            }*/
        }
        Debug.Log($"first is {firstLine} second is {secondLine}");
        int absDifference = Mathf.Abs(firstLine - secondLine);
        if (absDifference > firstLine  || absDifference > secondLine)//make sure player drew a decent plus and not just 1 straight line
        {
            pixelHits = 0;
            Debug.Log($"can't finesse us with {absDifference}");
        }

        Health.Apply();
        return Health;
    }
}
