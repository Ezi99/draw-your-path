using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GroceryBag : MonoBehaviour
{
    public WallTutorial m_Wall;
    public bool isTutorialBag;

    public void WhenSelected()
    {
        if (isTutorialBag == true)
        {
            m_Wall.CollapseWall();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
