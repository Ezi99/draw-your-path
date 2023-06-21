using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenPotionTutorial : GreenPotion
{
    public WallTutorial m_Wall;

    protected override void BeforeDespawn()
    {
        m_Area3Ui.SpawnEnemies();
        m_Wall.CollapseWall();
    }
}
