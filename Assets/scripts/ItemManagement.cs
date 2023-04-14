using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManagement : MonoBehaviour
{
    public GameObject WeakItem;
    public GameObject RegularItem;
    public GameObject StrongItem;
    public Transform ItemSpawn;
    protected List<GameObject> ItemList = new List<GameObject>();

    protected void checkNumOfItems(int limit = 2)
    {
        if (ItemList.Count >= limit)
        {
            Destroy(ItemList.First());
            ItemList.RemoveAt(0);
        }
    }
}
