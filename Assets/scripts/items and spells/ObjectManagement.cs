using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ObjectManagement : MonoBehaviour
{
    public GameObject Weak;
    public GameObject Regular;
    public GameObject Strong;
    public Transform SpawnLocation;
    protected int textureSize = 1024;
    protected List<GameObject> ObjectList = new List<GameObject>();
    protected int m_TotalPixelHitAttempt;
    protected float m_HitPercentage;

    public int TotalPixelHitAttempt
    {
        get { return m_TotalPixelHitAttempt; }  
    }

    public float HitPercentage
    {
        get { return m_HitPercentage; }
    }

    protected void checkNumOfItems(int limit = 2)
    {
        if (ObjectList.Count >= limit)
        {
            Destroy(ObjectList.First());
            ObjectList.RemoveAt(0);
        }
    }

    protected void encodeDrawing2PNG(string fileName, ref Texture2D drawing)
    {
        byte[] encodedImage = drawing.EncodeToPNG();
        string filePath = Application.dataPath + "/" + fileName;

        using (FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
        {
            stream.Write(encodedImage, 0, encodedImage.Length);
        }
    }

    protected bool isPixelSet(int x, int y, ref int pixelHits, Texture2D drawCanvas)
    {
        if (drawCanvas.GetPixel(x, y) == Color.red)
        {
            pixelHits++;
            return true;
        }
        return false;
    }

    protected void checkIfEnoughPixelHits(ref int pixelHits, int totalPixelHitAttempt, float limit)
    {

        if (pixelHits < totalPixelHitAttempt * limit)
        {
            pixelHits = 0;
        }
    }

    public void ResetTotalPixelHitAttempt()
    {
        m_TotalPixelHitAttempt = 0;
    }
}
