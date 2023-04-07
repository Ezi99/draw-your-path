using UnityEngine;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SocialPlatforms.Impl;

public class SaveAndCompareDrawing : MonoBehaviour
{
    public GameObject RightMarkerDrawing;
    public GameObject LeftMarkerDrawing;
    public DrawCanvas LeftDrawCanvas;
    public DrawCanvas RightDrawCanvas;
    private string m_PlayersDrawing = "PlayersDrawing.png";
    private bool m_LeftDrew = false;
    private bool m_RightDrew = false;
    private int m_TextureSize = 2048;
    private Color[] m_Colors;

    private class coordinates
    {
        public int x;
        public int y;
    }

    coordinates highestCoord = new coordinates();
    coordinates lowestCoord = new coordinates();

    //private Texture2D test_image;

    void Start()
    {
        m_Colors = Enumerable.Repeat(Color.black, 30 * 30).ToArray();

        /*byte[] fileData = System.IO.File.ReadAllBytes(Application.dataPath + "/PlayersDrawing.png");
        // Create a new Texture2D object and load the PNG file data into it
        test_image = new Texture2D(2048, 2048);
        test_image.LoadImage(fileData);
        DrawCanvas test = new DrawCanvas();
        test.texture = test_image;
        compareDrawing(ref test);*/
    }

    void Update()
    {
        checkIfFinishDrawing();
    }

    private void checkIfFinishDrawing()
    {
        if (RightMarkerDrawing.activeSelf == true)
        {
            if (m_RightDrew == false)
            {
                m_RightDrew = true;
            }
        }
        else if (RightMarkerDrawing.activeSelf == false)
        {
            if (m_RightDrew == true)
            {
                saveDrawing(m_PlayersDrawing, ref LeftDrawCanvas);
                m_RightDrew = false;
            }
        }
        if (LeftMarkerDrawing.activeSelf == true)
        {
            if (m_LeftDrew == false)
            {
                m_LeftDrew = true;
            }
        }
        else if (LeftMarkerDrawing.activeSelf == false)
        {
            if (m_LeftDrew == true)
            {
                saveDrawing(m_PlayersDrawing, ref RightDrawCanvas);
                m_LeftDrew = false;
            }
        }
    }

    private void saveDrawing(string fileName, ref DrawCanvas drawCanvas)
    {
        if (drawCanvas == null) return;
        byte[] bytes = drawCanvas.texture.EncodeToPNG();
        compareDrawing(ref drawCanvas);
        resetCanvas(ref drawCanvas);
        string filePath = Application.dataPath + "/" + fileName;
        using (FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        Debug.Log("saved drawing to: " + filePath);
    }

    private void resetCanvas(ref DrawCanvas drawCanvas)
    {
        Color[] emptyColors = Enumerable.Repeat(new Color(0, 0, 0, 0), 2048 * 2048).ToArray();
        drawCanvas.texture.SetPixels(emptyColors);
        drawCanvas.texture.Apply();
    }

    private void compareDrawing(ref DrawCanvas drawCanvas)
    {
        getHighestAndLowestCoords(ref drawCanvas);
        //checkIfShield(ref drawCanvas);
        checkIfSword(ref drawCanvas);
    }

    private void getHighestAndLowestCoords(ref DrawCanvas drawCanvas)
    {
        lowestCoord.x = 0;
        lowestCoord.y = 0;
        bool lookingForHighestX = true;
        bool lookingForLowestY;

        for (int x = 0; x < (int)drawCanvas.textureSize.x; x++)
        {
            lookingForLowestY = true;
            for (int y = 0; y < (int)drawCanvas.textureSize.y && lookingForLowestY == true; y++)
            {
                Color pixelColor = drawCanvas.texture.GetPixel(x, y);

                if (pixelColor == Color.black)
                {
                    if (lookingForHighestX == true)
                    {
                        highestCoord.y = y;
                        highestCoord.x = x;
                        lookingForHighestX = false;
                    }
                    if (x > lowestCoord.x && lookingForLowestY == true)
                    {
                        lowestCoord.y = y;
                        lowestCoord.x = x;
                        lookingForLowestY = false;
                    }
                }
            }
        }
    }

    private int checkIfShield(ref DrawCanvas drawCanvas)
    {
        Texture2D Shield;
        int pixelHits;
        byte[] encodedImage;
        string filePath = Application.dataPath + "/" + "Circle.png";

        Shield = drawShield();
        encodedImage = Shield.EncodeToPNG();
        pixelHits = compareMyDrawing2PlayerDrawing(ref Shield, ref drawCanvas.texture);
        Debug.Log("THERE WAS " + pixelHits + " HITS");
        using (FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
        {
            stream.Write(encodedImage, 0, encodedImage.Length);
        }

        Debug.Log("saved the drawing to: " + filePath);
        return pixelHits;
    }

    private Texture2D drawShield()  // later we finna compare as we make the shield to see how many pixel hits we got instead of creating and then comparing
    {
        Texture2D shield = new Texture2D(m_TextureSize, m_TextureSize);
        float radius = (lowestCoord.x - highestCoord.x) / 2f;
        float centerY = highestCoord.y + radius - 200;
        float centerX = highestCoord.x + radius;

        Debug.Log("highest is " + highestCoord.x);
        Debug.Log("lowest is " + lowestCoord.x);
        for (int x = 0; x < m_TextureSize; x++)
        {
            for (int y = 0; y < m_TextureSize; y++)
            {
                float distanceToCenter = Mathf.Sqrt(Mathf.Pow(y - centerY, 2) + Mathf.Pow(x - centerX, 2));
                if (distanceToCenter <= radius && y <= shield.width && x <= shield.height)//putting distanceToCenter == radius brings more accurate pixel hits but low pixel hits overall
                {
                    shield.SetPixels(x, y, 30, 30, m_Colors);
                }
            }
        }

        shield.Apply();
        return shield;
    }

    private int compareMyDrawing2PlayerDrawing(ref Texture2D myDrawing, ref Texture2D playerDrawing)
    {
        int pixelHits = 0;
        for (int x = 0; x < m_TextureSize; x++)
        {
            for (int y = 0; y < m_TextureSize; y++)
            {
                Color myShieldPixelColor = myDrawing.GetPixel(x, y);
                Color playerShieldPixelColor = playerDrawing.GetPixel(x, y);

                if (myShieldPixelColor == Color.black && playerShieldPixelColor == Color.black)
                {
                    pixelHits++;
                }
            }
        }

        return pixelHits;
    }

    private int checkIfSword(ref DrawCanvas drawCanvas)
    {
        Texture2D Sword;
        int pixelHits = 0;
        byte[] encodedImage;
        string filePath = Application.dataPath + "/" + "Sword.png";
        Sword = drawSword();
        encodedImage = Sword.EncodeToPNG();
        //pixelHits = compareMyDrawing2PlayerDrawing(ref Sword, ref drawCanvas.texture);
        Debug.Log("THERE WAS " + pixelHits + " HITS");
        using (FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
        {
            stream.Write(encodedImage, 0, encodedImage.Length);
        }

        Debug.Log("saved the drawing to: " + filePath);
        return pixelHits;
    }

    private Texture2D drawSword() // later we finna compare as we make the sword to see how many pixel hits we got instead of creating and then comparing
    {
        Texture2D Sword = new Texture2D(m_TextureSize, m_TextureSize);
        int swordWidth = (lowestCoord.x - highestCoord.x) / 20;
        int handleLocation = (lowestCoord.x - highestCoord.x) / 7;
        int handleSize = (lowestCoord.x - highestCoord.x) / 10;
        Debug.Log("sword width is " + swordWidth);
        Debug.Log($"sword length is {lowestCoord.x - highestCoord.x}");
        Debug.Log("handleLocation is " + handleLocation);
        Debug.Log("handleSize is " + handleSize);
        for (int x = 0; x < m_TextureSize; x++)
        {
            for (int y = 0; y < m_TextureSize; y++)
            {
                if (x >= highestCoord.x && x <= lowestCoord.x)
                {
                    if(x <= lowestCoord.x - handleLocation && x >= lowestCoord.x - handleLocation - handleLocation / 3)// check if it's time to draw handle
                    {
                        if (y >= highestCoord.y - handleSize && y <= highestCoord.y + handleSize)
                        {
                            Sword.SetPixels(x, y, 30, 30, m_Colors);
                        }
                    }
                    else if (y >= highestCoord.y - swordWidth && y <= highestCoord.y + swordWidth)
                    {
                        Sword.SetPixels(x, y, 30, 30, m_Colors);
                    }
                }
                
            }
        }

        Sword.Apply();
        return Sword;
    }

    
}


