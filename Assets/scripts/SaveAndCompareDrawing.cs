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

    private string playersDrawingName = "PlayersDrawing.png";
    private bool leftItemDrew = false;
    private bool leftSpellDrew = false;
    private bool rightItemDrew = false;
    private bool rightSpellDrew = false;
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
        checkIfStartedDrawing();
    }

    private void checkIfStartedDrawing()
    {
        checkIfMarkerStartedDrawing(ref RightItemMarker, ref LeftDrawCanvas, ref rightItemDrew);
        checkIfMarkerStartedDrawing(ref LeftItemMarker, ref RightDrawCanvas, ref leftItemDrew);
        checkIfMarkerStartedDrawing(ref RightSpellMarker, ref LeftDrawCanvas, ref rightSpellDrew);
        checkIfMarkerStartedDrawing(ref LeftSpellMarker, ref RightDrawCanvas, ref leftSpellDrew);
    }

    private void checkIfMarkerStartedDrawing(ref Draw marker, ref DrawCanvas drawCanvas, ref bool Drew)
    {
        if (marker.isActiveAndEnabled == true)
        {
            numOfDrawnPixels = marker.GetNumOfDrawnPixels();
            marker.GetCoordinates(ref highestCoord, ref lowestCoord);
            Drew = true;
        }
        else
        {
            if (Drew == true)
            {
                Debug.Log($"number of total pixels - {numOfDrawnPixels}");
                analyseDrawing(playersDrawingName, ref drawCanvas, ref marker);
                resetStats(ref marker);
                Drew = false;
            }
        }

    }

    private void analyseDrawing(string fileName, ref DrawCanvas drawCanvas, ref Draw marker)
    {
        if (numOfDrawnPixels > 40) // small drawings will bring unexpected results so we put this limit
        {
            if (marker == LeftItemMarker || marker == RightItemMarker)
            {
                compareItemDrawing(ref drawCanvas);
            }
            else
            {
                compareSpellDrawing(ref drawCanvas, ref marker);
            }
        }
        encodeDrawing2PNG(fileName, ref drawCanvas.texture);
        if (numOfDrawnPixels != 0)
        {
            drawCanvas.ResetCanvas();
        }
    }

    private void compareSpellDrawing(ref DrawCanvas drawCanvas, ref Draw marker)
    {
        accuracyLimit = (int)(numOfDrawnPixels * 0.5);
        string result = "nothing";
        int FireBallPixelHits = FireBallManage.CheckIfFireBall(ref drawCanvas, ref highestCoord, ref lowestCoord, ref colors);
        int HealthPixelHits = HealthManage.CheckIfHealth(ref drawCanvas, ref highestCoord, ref lowestCoord, ref colors);
        int bridgePixelHits = BridgeManage.CheckIfBridge(ref drawCanvas, ref highestCoord, ref lowestCoord, ref colors);


        comparePixelHits(FireBallPixelHits, "FireBall", ref result);
        comparePixelHits(HealthPixelHits, "Health", ref result);
        comparePixelHits(bridgePixelHits, "bridge", ref result);

        if (result == "FireBall")
        {
            FireBallManage.SpawnFireBall(FireBallPixelHits, numOfDrawnPixels, ref marker, rightSpellDrew);
        }
        else if (result == "Health")
        {
            HealthManage.SpawnHealth(HealthPixelHits, numOfDrawnPixels);
        }
        else if (result == "bridge")
        {
            BridgeManage.SpawnBridge(bridgePixelHits, numOfDrawnPixels);
        }


        Debug.Log($"CONGRATS YOU GOT {result} with accuracy above {accuracyLimit} pixels !!!");

    }

    private void resetStats(ref Draw marker)
    {
        marker.ResetCoords(ref highestCoord, ref lowestCoord);
        marker.ResetNumOfPixels();
        maxItemPixelHits = 0;
    }

    private void compareItemDrawing(ref DrawCanvas drawCanvas)
    {
        accuracyLimit = (int)(numOfDrawnPixels * 0.5);
        string result = "nothing";
        int shieldPixelHits = ShieldManage.CheckIfShield(ref drawCanvas, ref highestCoord, ref lowestCoord, ref colors);
        int swordPixelHits = SwordManage.CheckIfSword(ref drawCanvas, ref highestCoord, ref lowestCoord, ref colors);
        int HammerPixelHits = HammerManage.CheckIfHammer(ref drawCanvas, ref highestCoord, ref lowestCoord, ref colors);

        comparePixelHits(shieldPixelHits, "shield", ref result);
        comparePixelHits(swordPixelHits, "sword", ref result);
        comparePixelHits(HammerPixelHits, "hammer", ref result);

        if (result == "sword")
        {
            SwordManage.SpawnSword(swordPixelHits, numOfDrawnPixels);
        }
        else if (result == "shield")
        {
            ShieldManage.SpawnShield(shieldPixelHits, numOfDrawnPixels);
        }
        else if (result == "hammer")
        {
            HammerManage.SpawnHammer(HammerPixelHits, numOfDrawnPixels);
        }

        Debug.Log($"CONGRATS YOU GOT {result} with accuracy above {accuracyLimit} pixels !!!");
    }

    private void comparePixelHits(int itemPixelHits, string itemName, ref string result)
    {
        if (itemPixelHits > accuracyLimit && itemPixelHits >= maxItemPixelHits)
        {
            maxItemPixelHits = itemPixelHits;
            result = itemName;
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