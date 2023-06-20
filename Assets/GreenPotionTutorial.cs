using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenPotionTutorial : GreenPotion
{
    public WallTutorial m_Wall;
    public Area3Ui m_Area3Ui;

    protected override void Despawn()
    {
        m_Area3Ui.SpawnEnemies();
        m_Wall.CollapseWall();
        Destroy(gameObject);
    }
}
