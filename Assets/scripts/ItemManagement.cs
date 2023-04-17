using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ItemManagement : MonoBehaviour
{
    public GameObject WeakItem;
    public GameObject RegularItem;
    public GameObject StrongItem;
    public Transform ItemSpawn;
    protected List<GameObject> ItemList = new List<GameObject>();

    protected void checkNumOfItems(int limit = 2)
    {
        if (ItemList.Count >= limit)
        {
            Destroy(ItemList.First());
            ItemList.RemoveAt(0);
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
