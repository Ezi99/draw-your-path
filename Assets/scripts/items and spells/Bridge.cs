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

    public void WhenSelected()
    {
        gameObject.layer = LayerMask.NameToLayer("grabbed bridge");
    }

    public void WhenExited()
    {
        gameObject.layer = LayerMask.NameToLayer("bridge");
    }
}
