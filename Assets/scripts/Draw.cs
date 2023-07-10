using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Draw : MonoBehaviour
{
    [SerializeField] private Transform tip;
    [SerializeField] private int penSize;
    private Color[] colors;
    private RaycastHit touchedSurface;
    private DrawCanvas canvas = null;
    private Vector2 touchedSurfacePos, lastTouchedSurfacePos;
    private bool touchedSurfaceLastFrame;
    private Coordinates highestXCoord = new Coordinates();
    private Coordinates lowestXCoord = new Coordinates();
    private Coordinates highestYCoord = new Coordinates();
    private Coordinates lowestYCoord = new Coordinates();
    private int numOfPixels = 0;

    void Start()
    {
        colors = Enumerable.Repeat(Color.red, penSize * penSize).ToArray();
        highestYCoord.y = 0;
        lowestYCoord.y = 1024;
        lowestXCoord.x = 0;
        lowestXCoord.y = 0;
        highestXCoord.x = 1024;
        highestXCoord.y = 1024;
    }

    void Update()
    {
        Drawing();
    }

    private void Drawing()
    {
        if (Physics.Raycast(tip.position, transform.forward, out touchedSurface, 0.035f))
        {
            if (touchedSurface.transform.CompareTag("DrawCanvas") == true)
            {
                if (canvas == null)
                {
                    canvas = touchedSurface.transform.GetComponent<DrawCanvas>();
                }

                touchedSurfacePos = new Vector2(touchedSurface.textureCoord.x, touchedSurface.textureCoord.y);
                var x = (int)(touchedSurfacePos.x * canvas.textureSize.x - (penSize / 2));
                var y = (int)(touchedSurfacePos.y * canvas.textureSize.y - (penSize / 2));

                if (y - penSize < 0 || y + penSize > canvas.textureSize.y || x - penSize < 0 || x + penSize > canvas.textureSize.x)
                {
                    return;
                }

                updateHighLowCoords(x, y);
                if (touchedSurfaceLastFrame == true)
                {
                    canvas.texture.SetPixels(x, y, penSize, penSize, colors);
                    numOfPixels++;
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(lastTouchedSurfacePos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(lastTouchedSurfacePos.y, y, f);

                        if (lerpY - penSize < 0 || lerpY + penSize > canvas.textureSize.y || lerpX - penSize < 0 || lerpX + penSize > canvas.textureSize.x)
                        {
                            return;
                        }

                        canvas.texture.SetPixels(lerpX, lerpY, penSize, penSize, colors);
                        updateHighLowCoords(lerpX, lerpY);
                    }

                    canvas.texture.Apply();
                }

                lastTouchedSurfacePos = new Vector2(x, y);
                touchedSurfaceLastFrame = true;
                return;
            }
        }

        canvas = null;
        touchedSurfaceLastFrame = false;
    }

    public void GetCoordinates(Coordinates highXCoord, Coordinates lowXCoord, Coordinates highYCoord, Coordinates lowYCoord)
    {
        highXCoord.x = highestXCoord.x;
        highXCoord.y = highestXCoord.y;
        lowXCoord.x = lowestXCoord.x;
        lowXCoord.y = lowestXCoord.y;
        lowYCoord.x = lowestYCoord.x;
        lowYCoord.y = lowestYCoord.y;
        highYCoord.x = highestYCoord.x;
        highYCoord.y = highestYCoord.y;
    }

    private void updateHighLowCoords(int x, int y)
    {
        if (x < highestXCoord.x)
        {
            highestXCoord.y = y;
            highestXCoord.x = x;
        }
        if (x > lowestXCoord.x)
        {
            lowestXCoord.y = y;
            lowestXCoord.x = x;
        }
        if(y > highestYCoord.y)
        {
            highestYCoord.x = x;
            highestYCoord.y = y;
        }
        if(y < lowestYCoord.y)
        {
            lowestYCoord.x = x;
            lowestYCoord.y = y;
        }
    }

    public int GetNumOfDrawnPixels()
    {
        return numOfPixels;
    }

    public void ResetCoords( Coordinates highXCoord,  Coordinates lowXCoord, Coordinates highYCoord, Coordinates lowYCoord)
    {
        lowXCoord.x = lowestXCoord.x = 0;
        lowXCoord.y = lowestXCoord.y = 0;
        highXCoord.x = highestXCoord.x = 1024;
        highXCoord.y = highestXCoord.y = 1024;
        lowYCoord.x = lowestYCoord.x = 1024;
        lowYCoord.y = lowestYCoord.y = 1024;
        highYCoord.x = highestYCoord.x = 0;
        highYCoord.y = highestYCoord.y = 0;
    }

    public void ResetNumOfPixels()
    {
        numOfPixels = 0;
    }
}
