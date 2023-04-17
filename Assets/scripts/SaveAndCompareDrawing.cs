using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Timeline;
using System.Linq.Expressions;

public class SaveAndCompareDrawing : MonoBehaviour
{
    public SwordManagement SwordManage;
    public ShieldManagement ShieldManage;
    public BridgeManagement BridgeManage;
    public DrawCanvas LeftDrawCanvas;
    public DrawCanvas RightDrawCanvas;
    public Draw RightMarker;
    public Draw LeftMarker;
    private string playersDrawingName = "PlayersDrawing.png";
    private bool leftDrew = false;
    private bool rightDrew = false;
    private int textureSize = 1024;
    private Color[] colors;
    private Coordinates highestCoord = new Coordinates();
    private Coordinates lowestCoord = new Coordinates();
    private int numOfDrawnPixels;
    private int maxItemPixelHits = 0;
    private int accuracyLimit;

    void Start()
    {
        colors = Enumerable.Repeat(Color.red, 30 * 30).ToArray();
    }

    void Update()
    {
        checkIfFinishDrawing();
    }

    private void checkIfFinishDrawing()
    {
        if (RightMarker.isActiveAndEnabled == true)
        {
            numOfDrawnPixels = RightMarker.GetNumOfDrawnPixels();
            RightMarker.GetCoordinates(ref highestCoord, ref lowestCoord);
            if (rightDrew == false)
            {
                rightDrew = true;
            }
        }
        else if (RightMarker.isActiveAndEnabled == false)
        {
            if (rightDrew == true)
            {
                Debug.Log($"number of total pixels - {numOfDrawnPixels}");
                saveDrawing(playersDrawingName, ref LeftDrawCanvas);
                resetStats(ref RightMarker);
                rightDrew = false;
            }
        }
        if (LeftMarker.isActiveAndEnabled == true)
        {
            numOfDrawnPixels = LeftMarker.GetNumOfDrawnPixels();
            LeftMarker.GetCoordinates(ref highestCoord, ref lowestCoord);
            if (leftDrew == false)
            {
                leftDrew = true;
            }
        }
        else if (LeftMarker.isActiveAndEnabled == false)
        {
            if (leftDrew == true)
            {
                Debug.Log($"number of total pixels - {numOfDrawnPixels}");
                saveDrawing(playersDrawingName, ref RightDrawCanvas);
                resetStats(ref LeftMarker);
                leftDrew = false;
            }
        }
    }

    private void saveDrawing(string fileName, ref DrawCanvas drawCanvas)
    {
        if (numOfDrawnPixels > 30)
        {
            compareDrawing(ref drawCanvas);
        }
        encodeDrawing2PNG(fileName, ref drawCanvas.texture);
        if (numOfDrawnPixels != 0)
        {
            resetCanvas(ref drawCanvas);
        }
    }

    private void resetStats(ref Draw marker)
    {
        marker.ResetCoords(ref highestCoord, ref lowestCoord);
        marker.ResetNumOfPixels();
        maxItemPixelHits = 0;
    }

    private void compareDrawing(ref DrawCanvas drawCanvas)
    {
        accuracyLimit = (int)(numOfDrawnPixels * 0.5);
        string result = "nothing";
        int shieldPixelHits = checkIfShield(ref drawCanvas);
        int swordPixelHits = checkIfSword(ref drawCanvas);
        int bridgePixelHits = checkIfBridge(ref drawCanvas);
        comparePixelHits(shieldPixelHits, "shield", ref result);
        comparePixelHits(swordPixelHits, "sword", ref result);
        comparePixelHits(bridgePixelHits, "bridge", ref result);
        if (result == "sword")
        {
            SwordManage.SpawnSword(swordPixelHits, numOfDrawnPixels);
        }
        else if (result == "shield")
        {
            ShieldManage.SpawnShield(shieldPixelHits, numOfDrawnPixels);
        }
        else if (result == "bridge")
        {
            BridgeManage.SpawnBridge(bridgePixelHits, numOfDrawnPixels);
        }

        Debug.Log($"CONGRATS YOU GOT {result} with accuracy above {accuracyLimit} pixels !!!");
    }

    private void resetCanvas(ref DrawCanvas drawCanvas)
    {
        Color[] emptyColors = Enumerable.Repeat(new Color(0, 0, 0, 0), textureSize * textureSize).ToArray();
        drawCanvas.texture.SetPixels(emptyColors);
        drawCanvas.texture.Apply();
    }

    private void comparePixelHits(int itemPixelHits, string itemName, ref string result)
    {
        if (itemPixelHits > accuracyLimit && itemPixelHits > maxItemPixelHits)
        {
            maxItemPixelHits = itemPixelHits;
            result = itemName;
        }
    }

    private int checkIfShield(ref DrawCanvas drawCanvas)
    {
        Texture2D Shield;
        int pixelHits = 0;
        Shield = drawShield(ref drawCanvas.texture, ref pixelHits);
        Debug.Log("THERE WAS " + pixelHits + " SHIELD HITS");
        encodeDrawing2PNG("Circle.png", ref Shield);
        return pixelHits;
    }

    private int checkIfSword(ref DrawCanvas drawCanvas)
    {
        Texture2D Sword;
        int pixelHits = 0;
        Sword = drawSword(ref drawCanvas.texture, ref pixelHits);
        Debug.Log("THERE WAS " + pixelHits + " SWORD HITS");
        encodeDrawing2PNG("Sword.png", ref Sword);
        return pixelHits;
    }

    private int checkIfBridge(ref DrawCanvas drawCanvas)
    {
        Texture2D Bridge;
        int pixelHits = 0;
        Bridge = drawBridge(ref drawCanvas.texture, ref pixelHits);
        Debug.Log("THERE WAS " + pixelHits + " BRIDGE HITS");
        encodeDrawing2PNG("Bridge.png", ref Bridge);
        return pixelHits;
    }

    private Texture2D drawBridge(ref Texture2D drawCanvas, ref int pixelHits)
    {
        int bridgeWidth = lowestCoord.x - highestCoord.x;
        Texture2D Bridge = new Texture2D(textureSize, textureSize);

        for (int x = highestCoord.x; x < lowestCoord.x && x < textureSize - 30; x += 15)
        {
            for (int y = 0; y < textureSize - 30; y += 15)
            {
                if (x >= highestCoord.x && x <= highestCoord.x + 30 || x >= lowestCoord.x - 30)
                {
                    if (y >= highestCoord.y - bridgeWidth && y <= highestCoord.y + bridgeWidth)
                    {
                        Bridge.SetPixels(x, y, 30, 30, colors);
                        isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                    }
                }
                else if(y >= highestCoord.y - bridgeWidth  && y <= highestCoord.y - bridgeWidth + 30 || y <= highestCoord.y + bridgeWidth && y >= highestCoord.y + bridgeWidth - 30)
                {
                    Bridge.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                }
            }
        }

        Bridge.Apply();
        return Bridge;
    }

    private Texture2D drawShield(ref Texture2D drawCanvas, ref int pixelHits)
    {
        Texture2D shield = new Texture2D(textureSize, textureSize);
        float radius = (lowestCoord.x - highestCoord.x) / 2f;
        float centerY = highestCoord.y;
        float centerX = highestCoord.x + radius + 30;// added 30 to improve player's chances 

        for (int x = highestCoord.x; x <= lowestCoord.x + 30 && x < textureSize - 30; x += 15)
        {
            for (int y = 0; y < textureSize - 30; y += 15)
            {
                float distanceToCenter = Mathf.Sqrt(Mathf.Pow(y - centerY, 2) + Mathf.Pow(x - centerX, 2));
                if (distanceToCenter <= radius && distanceToCenter >= radius - 30)//putting distanceToCenter == radius brings more accurate pixel hits but low pixel hits overall
                {
                    shield.SetPixels(x, y, 30, 30, colors);
                    isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                }
            }
        }

        shield.Apply();
        return shield;
    }

    private Texture2D drawSword(ref Texture2D drawCanvas, ref int pixelHits)
    {
        Texture2D Sword = new Texture2D(textureSize, textureSize);
        int swordWidth = (lowestCoord.x - highestCoord.x) / 9;
        int handleLocation = (lowestCoord.x - highestCoord.x) / 8;
        int handleSize = (lowestCoord.x - highestCoord.x) / 3;

        for (int x = highestCoord.x; x <= lowestCoord.x && x < textureSize - 30; x += 15)
        {
            for (int y = 0; y < textureSize - 30; y += 15)
            {
                if (x <= lowestCoord.x - handleLocation && x >= lowestCoord.x - 2 * handleLocation)// check if it's time to draw handle
                {
                    if (y >= highestCoord.y - handleSize && y <= highestCoord.y + handleSize)
                    {
                        if (y <= highestCoord.y - handleSize + handleSize / 2 || y >= highestCoord.y + handleSize - handleSize / 2)
                        {
                            Sword.SetPixels(x, y, 30, 30, colors);
                            isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                        }
                    }
                }
                else if (y >= highestCoord.y - swordWidth && y <= highestCoord.y + swordWidth)
                {
                    if (y <= highestCoord.y - swordWidth + swordWidth / 2 || y >= highestCoord.y + swordWidth - swordWidth / 2)
                    {
                        Sword.SetPixels(x, y, 30, 30, colors);
                        isPixelSet(x, y, ref pixelHits, ref drawCanvas);
                    }
                }
            }
        }

        Sword.Apply();
        return Sword;
    }

    private void isPixelSet(int x, int y, ref int pixelHits, ref Texture2D drawCanvas)
    {
        if (drawCanvas.GetPixel(x, y) == Color.red)
        {
            pixelHits++;
        }
    }

    private void encodeDrawing2PNG(string fileName, ref Texture2D drawing)
    {
        byte[] encodedImage = drawing.EncodeToPNG();
        string filePath = Application.dataPath + "/" + fileName;

        using (FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
        {
            stream.Write(encodedImage, 0, encodedImage.Length);
        }
    }
}