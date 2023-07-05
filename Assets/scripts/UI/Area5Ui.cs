using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area5Ui : MonoBehaviour
{
    public GameObject m_FinalTaskMessage;
    public GameObject m_ShoppingBag;
    public GameObject m_FlatShoppingBag;

    public void ActivateArea()
    {
        m_FinalTaskMessage.SetActive(true);
        m_ShoppingBag.SetActive(true);
        m_FlatShoppingBag.SetActive(true);
    }

}
