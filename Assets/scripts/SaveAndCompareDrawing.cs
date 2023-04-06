using UnityEngine;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class SaveAndCompareDrawing : MonoBehaviour
{
    public GameObject RightMarkerDrawing;
    public GameObject LeftMarkerDrawing;
    public DrawCanvas LeftDrawCanvas;
    public DrawCanvas RightDrawCanvas;
    private string m_PlayersDrawing = "PlayersDrawing.png";
    private bool m_LeftDrew = false;
    private bool m_RightDrew = false;


    void Start()
    {
       
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
        else if (LeftMarkerDrawing.activeSelf == true)
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

    /*private void compareDrawing(ref DrawCanvas drawCanvas)
    {
        checkIfShield(ref drawCanvas);
    }
    private void checkIfShield(ref DrawCanvas drawCanvas)
    {
       
    }*/
}


