using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerManagement : ObjectManagement
{
    public int CheckIfHammer(ref DrawCanvas drawCanvas, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        Texture2D hammer;
        int pixelHits = 0;
        hammer = drawHammer(ref drawCanvas.texture, ref pixelHits, ref highestCoord, ref lowestCoord, ref colors);
        Debug.Log("THERE WAS " + pixelHits + " HAMMER HITS");
        encodeDrawing2PNG("Hammer.png", ref hammer);
        return pixelHits;
    }

    private Texture2D drawHammer(ref Texture2D drawCanvas, ref int pixelHits, ref Coordinates highestCoord, ref Coordinates lowestCoord, ref Color[] colors)
    {
        int drawingLength = (lowestCoord.x - highestCoord.x);
        int HeadWidth = drawingLength / 5;
        int HeadLength = drawingLength / 6;
        //int handleLength = 0;
        int handleWidth = HeadWidth / 8;

        Debug.Log($"drawingLength {drawingLength} HeadWidth {HeadWidth} HeadLength {HeadLength} handleWidth {handleWidth}");

        Texture2D hammer = new Texture2D(textureSize, textureSize);

        for (int x = highestCoord.x; x < lowestCoord.x && x < textureSize; x += 10)
        {
            for (int y = highestCoord.y - HeadWidth; y < textureSize && y <= highestCoord.y + HeadWidth && x <= highestCoord.x + HeadLength && (x <= highestCoord.x + HeadLength / 5 || x >= highestCoord.x + HeadLength - HeadLength / 5); y += 15)
            {
                hammer.SetPixels(x, y, 30, 30, colors);
                isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                Debug.Log("entered loop 1");
            }
            if (x <= highestCoord.x + HeadLength && (x > highestCoord.x + HeadLength / 5 || x < highestCoord.x + HeadLength - HeadLength / 5))
            {
                for (int y = highestCoord.y - HeadWidth; y < textureSize && y <= highestCoord.y - HeadWidth + HeadWidth / 5; y += 15)
                {
                    hammer.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                    Debug.Log($"entered loop 2 y = {y} highestCoord.y = {highestCoord.y} HeadWidth / 5 = {HeadWidth / 5}");
                }
                for (int y = highestCoord.y + HeadWidth - HeadWidth / 5; y < textureSize && y <= highestCoord.y + HeadWidth; y += 15)
                {
                    hammer.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                    Debug.Log($"entered loop 2 y = {y} highestCoord.y = {highestCoord.y} HeadWidth / 5 = {HeadWidth / 5}");
                }
            }
            for (int y = highestCoord.y - handleWidth; y < textureSize && y <= highestCoord.y + handleWidth && x >= highestCoord.x + HeadLength && x <= lowestCoord.x; y += 15)
            {
                hammer.SetPixels(x, y, 30, 30, colors);
                isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                Debug.Log("entered loop 3");
            }
        }

        hammer.Apply();
        return hammer;
    }
}