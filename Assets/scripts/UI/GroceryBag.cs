using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryBag : MonoBehaviour
{
    public WallTutorial m_Wall;

    public void WhenSelected()
    {
        BagTaken();
    }

    protected virtual void BagTaken()
    {
        m_Wall.CollapseWall();
    }
}
