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

    protected void isPixelSet(int x, int y, ref int pixelHits, ref Texture2D drawCanvas)
    {
        if (drawCanvas.GetPixel(x, y) == Color.red)
        {
            pixelHits++;
        }
    }

}