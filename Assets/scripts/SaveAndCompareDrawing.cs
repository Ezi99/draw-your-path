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
    public FireBallManagement FireBallManage;
    public RegenerateHealth HealthManage;
    public HammerManagement HammerManage;
    public DrawCanvas LeftDrawCanvas;
    public DrawCanvas RightDrawCanvas;
    public Draw RightItemMarker;
    public Draw RightSpellMarker;
    public Draw LeftItemMarker;
    public Draw LeftSpellMarker;

    private const int k_NumOfPixelsThreshold = 70;
    private const int k_DrawingLengthThreshold = 200;
    private const int k_DrawingWidthThreshold = 200;
    private string playersDrawingName = "PlayersDrawing.png";
    private bool leftItemDrew = false;
    private bool leftSpellDrew = false;
    private bool rightItemDrew = false;
    private bool rightSpellDrew = false;
    private Color[] colors;
    private Coordinates highestXCoord = new Coordinates();
    private Coordinates lowestXCoord = new Coordinates();
    private Coordinates highestYCoord = new Coordinates();
    private Coordinates lowestYCoord = new Coordinates();
    private int numOfDrawnPixels;
    private int maxItemPixelHits = 0;
    private int accuracyLimit;

    void Start()
    {
        colors = Enumerable.Repeat(Color.red, 30 * 30).ToArray();
    }

    void Update()
    {
        checkIfStartedDrawing();
    }

    private void checkIfStartedDrawing()
    {
        checkIfMarkerStartedDrawing(RightItemMarker, LeftDrawCanvas, ref rightItemDrew);
        checkIfMarkerStartedDrawing(LeftItemMarker, RightDrawCanvas, ref leftItemDrew);
        checkIfMarkerStartedDrawing(RightSpellMarker, LeftDrawCanvas, ref rightSpellDrew);
        checkIfMarkerStartedDrawing(LeftSpellMarker, RightDrawCanvas, ref leftSpellDrew);
    }

    private void checkIfMarkerStartedDrawing(Draw marker, DrawCanvas drawCanvas, ref bool Drew)
    {
        if (marker.isActiveAndEnabled == true)
        {
            numOfDrawnPixels = marker.GetNumOfDrawnPixels();
            marker.GetCoordinates(highestXCoord, lowestXCoord, highestYCoord, lowestYCoord);
            Drew = true;
        }
        else
        {
            if (Drew == true)
            {
                Debug.Log($"number of total pixels - {numOfDrawnPixels}");
                analyseDrawing(playersDrawingName, drawCanvas, marker);
                resetStats(marker);
                Drew = false;
            }
        }

    }

    private void analyseDrawing(string fileName, DrawCanvas drawCanvas, Draw marker)
    {
        int drawingLength = lowestXCoord.x - highestXCoord.x;
        int drawingWidth = highestYCoord.y - lowestYCoord.y;
        if (numOfDrawnPixels > k_NumOfPixelsThreshold && drawingLength > k_DrawingLengthThreshold && drawingWidth > k_DrawingWidthThreshold) // small drawings will bring unexpected results so we put this limit
        {
            if (marker == LeftItemMarker || marker == RightItemMarker)
            {
                compareItemDrawing(drawCanvas);
            }
            else
            {
                compareSpellDrawing(drawCanvas, marker);
            }
        }
        encodeDrawing2PNG(fileName, drawCanvas.texture);
        if (numOfDrawnPixels != 0)
        {
            drawCanvas.ResetCanvas();
        }
    }

    private void compareSpellDrawing(DrawCanvas drawCanvas, Draw marker)
    {
        accuracyLimit = (int)(numOfDrawnPixels * 0.5);
        string result = "nothing";
        int FireBallPixelHits = FireBallManage.CheckIfFireBall(drawCanvas, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);
        int HealthPixelHits = HealthManage.CheckIfHealth(drawCanvas, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);
        int bridgePixelHits = BridgeManage.CheckIfBridge(drawCanvas, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);


        comparePixelHits(FireBallPixelHits, "FireBall", ref result);
        comparePixelHits(HealthPixelHits, "Health", ref result);
        comparePixelHits(bridgePixelHits, "bridge", ref result);

        if (result == "FireBall")
        {
            if (BridgeManage.HitPercentage < FireBallManage.HitPercentage)
            {
                FireBallManage.SpawnFireBall(FireBallPixelHits, marker, rightSpellDrew);
            }
        }
        else if (result == "Health")
        {
            HealthManage.SpawnHealth(HealthPixelHits);
        }
        else if (result == "bridge")
        {
            if (BridgeManage.HitPercentage > FireBallManage.HitPercentage)
            {
                BridgeManage.SpawnBridge(bridgePixelHits);
            }
        }


        Debug.Log($"CONGRATS YOU GOT {result} with accuracy above {accuracyLimit} pixels !!!");

    }

    private void resetStats(Draw marker)
    {
        marker.ResetCoords(highestXCoord, lowestXCoord, highestYCoord, lowestYCoord);
        marker.ResetNumOfPixels();
        FireBallManage.ResetTotalPixelHitAttempt();
        ShieldManage.ResetTotalPixelHitAttempt();
        SwordManage.ResetTotalPixelHitAttempt();
        BridgeManage.ResetTotalPixelHitAttempt();
        HammerManage.ResetTotalPixelHitAttempt();
        HealthManage.ResetTotalPixelHitAttempt();
        maxItemPixelHits = 0;
    }

    private void compareItemDrawing(DrawCanvas drawCanvas)
    {
        accuracyLimit = (int)(numOfDrawnPixels * 0.5);
        string result = "nothing";
        int shieldPixelHits = ShieldManage.CheckIfShield(drawCanvas, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);
        int swordPixelHits = SwordManage.CheckIfSword(drawCanvas, highestXCoord, lowestXCoord, colors);
        int HammerPixelHits = HammerManage.CheckIfHammer(drawCanvas, highestXCoord, lowestXCoord, highestYCoord, lowestYCoord, colors);

        comparePixelHits(shieldPixelHits, "shield", ref result);
        comparePixelHits(swordPixelHits, "sword", ref result);
        comparePixelHits(HammerPixelHits, "hammer", ref result);

        if (result == "sword")
        {
            SwordManage.SpawnSword(swordPixelHits);
        }
        else if (result == "shield")
        {
            ShieldManage.SpawnShield(shieldPixelHits);
        }
        else if (result == "hammer")
        {
            HammerManage.SpawnHammer(HammerPixelHits);
        }

        Debug.Log($"CONGRATS YOU GOT {result} with accuracy above {accuracyLimit} pixels !!!");
    }

    private void comparePixelHits(int ObjectPixelHits, string itemName, ref string result)
    {
        if (ObjectPixelHits > 10 && ObjectPixelHits >= maxItemPixelHits)//itemPixelHits > accuracyLimit && itemPixelHits >= maxItemPixelHits)
        {
            maxItemPixelHits = ObjectPixelHits;
            result = itemName;
        }
    }

    private void encodeDrawing2PNG(string fileName, Texture2D drawing)
    {
        byte[] encodedImage = drawing.EncodeToPNG();
        string filePath = Application.dataPath + "/" + fileName;

        using (FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
        {
            stream.Write(encodedImage, 0, encodedImage.Length);
        }
    }
}