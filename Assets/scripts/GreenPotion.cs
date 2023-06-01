using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GreenPotion : MonoBehaviour
{
    public VillageGate leftGate;
    public VillageGate rightGate;
    public AudioManager m_AudioManager;

    public void WhenSelected()
    {
        Debug.Log("Potion outy");
        m_AudioManager.PlayPotionTakenSound();
        leftGate.PotionTaken();
        rightGate.PotionTaken();
        Destroy(gameObject);
    }
}
