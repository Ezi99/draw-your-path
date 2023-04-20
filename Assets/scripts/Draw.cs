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
    [SerializeField] private int penSize = 30;
    private Color[] colors;
    private RaycastHit touchedSurface;
    private DrawCanvas canvas = null;
    private Vector2 touchedSurfacePos, lastTouchedSurfacePos;
    private bool touchedSurfaceLastFrame;
    private Coordinates highestCoord = new Coordinates();
    private Coordinates lowestCoord = new Coordinates();
    private int numOfPixels = 0;
    void Start()
    {
        colors = Enumerable.Repeat(Color.red, penSize * penSize).ToArray();
        lowestCoord.x = 0;
        lowestCoord.y = 0;
        highestCoord.x = 1024;
        highestCoord.y = 1024;
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

    public void GetCoordinates(ref Coordinates highCoord, ref Coordinates lowCoord)
    {
        highCoord = highestCoord;
        lowCoord = lowestCoord;
    }

    private void updateHighLowCoords(int x, int y)
    {
        if (x < highestCoord.x)
        {
            highestCoord.y = y;
            highestCoord.x = x;
        }
        if (x > lowestCoord.x)
        {
            lowestCoord.y = y;
            lowestCoord.x = x;
        }
    }

    public int GetNumOfDrawnPixels()
    {
        return numOfPixels;
    }

    public void ResetCoords(ref Coordinates highCoord, ref Coordinates lowCoord)
    {
        lowCoord.x = lowestCoord.x = 0;
        lowCoord.y = lowestCoord.y = 0;
        highCoord.x = highestCoord.x = 1024;
        highCoord.y = highestCoord.y = 1024;
    }

    public void ResetNumOfPixels()
    {
        numOfPixels = 0;
    }
}
