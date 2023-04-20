using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenerateHealth : ObjectManagement
{
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
        if (healthLength % 2 == 1) 
        {
            healthLength++;
        }

        for (int x = highestCoord.x; x <= lowestCoord.x && x < textureSize; x += 15)
        {
            for (int y = 0; y < textureSize; y += 15)
            {
                if (y >= highestCoord.y - 20 && y <= highestCoord.y + 20)
                {
                    Health.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                }
                else if (x <= highestCoord.x + healthLength / 2 + 20 && x >= highestCoord.x + healthLength / 2 - 20 && y >= highestCoord.y - healthLength / 2 && y <= highestCoord.y + healthLength / 2)
                {
                    Health.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                }
            }
        }

        Health.Apply();
        return Health;
    }
}
