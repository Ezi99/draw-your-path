using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GreenPotion : MonoBehaviour
{
    public VillageGate leftGate;
    public VillageGate rightGate;

    public void WhenSelected()
    {
        Debug.Log("Potion outy");
        leftGate.PotionTaken();
        rightGate.PotionTaken();
        Destroy(gameObject);
    }
}
