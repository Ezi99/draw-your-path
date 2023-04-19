using System.Linq;
using UnityEngine;

public class DrawCanvas : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(1024, 1024);
    void Start()
    {
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        r.material.mainTexture = texture;
    }

    public void ResetCanvas()
    {
        Color[] emptyColors = Enumerable.Repeat(new Color(0, 0, 0, 0), 1024 * 1024).ToArray();
        texture.SetPixels(emptyColors);
        texture.Apply();
    }


}
