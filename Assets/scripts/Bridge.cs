using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    private int lifeSpan;

    public void DespawnCountDown(int lifeSpan)
    {
        Destroy(gameObject, lifeSpan);
    }
}
