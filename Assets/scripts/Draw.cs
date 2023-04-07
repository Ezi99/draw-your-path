using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Draw : MonoBehaviour
{
    [SerializeField] private Transform m_Tip;
    [SerializeField] private int m_PenSize = 30;
    private Renderer m_Renderer;
    private Color[] m_Colors;
    private float m_TipHeight;
    private RaycastHit m_TouchedSurface;
    private DrawCanvas m_Canvas = null;
    private Vector2 m_TouchedSurfacePos, m_LastTouchedSurfacePos;
    private bool m_TouchedSurfaceLastFrame;

    void Start()
    {
        m_Renderer = m_Tip.GetComponent<Renderer>();
        m_Colors = Enumerable.Repeat(Color.black, m_PenSize * m_PenSize).ToArray();
        m_TipHeight = m_Tip.localScale.y;
    }

    void Update()
    {
        Drawing();
    }

    private void Drawing()
    {
        if (Physics.Raycast(m_Tip.position, transform.forward, out m_TouchedSurface, 0.035f))
        {
            if (m_TouchedSurface.transform.CompareTag("DrawCanvas") == true)
            {
                if (m_Canvas == null)
                {
                    m_Canvas = m_TouchedSurface.transform.GetComponent<DrawCanvas>();
                }

                m_TouchedSurfacePos = new Vector2(m_TouchedSurface.textureCoord.x, m_TouchedSurface.textureCoord.y);
                var x = (int)(m_TouchedSurfacePos.x * m_Canvas.textureSize.x - (m_PenSize / 2));
                var y = (int)(m_TouchedSurfacePos.y * m_Canvas.textureSize.y - (m_PenSize / 2));

                if (y < 0 || y > m_Canvas.textureSize.y || x < 0 || x > m_Canvas.textureSize.x)
                {
                    return;
                }

                if (m_TouchedSurfaceLastFrame == true)
                {
                    m_Canvas.texture.SetPixels(x, y, m_PenSize, m_PenSize, m_Colors);
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(m_LastTouchedSurfacePos.x, x, f);
                        var lerpy = (int)Mathf.Lerp(m_LastTouchedSurfacePos.y, y, f);
                        m_Canvas.texture.SetPixels(lerpX, lerpy, m_PenSize, m_PenSize, m_Colors);
                    }

                    m_Canvas.texture.Apply();
                }

                m_LastTouchedSurfacePos = new Vector2(x, y);
                m_TouchedSurfaceLastFrame = true;
                return;
            }
        }

        m_Canvas = null;
        m_TouchedSurfaceLastFrame = false;
    }
}
