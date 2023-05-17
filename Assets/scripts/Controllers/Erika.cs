using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erika : MonoBehaviour
{
    public GameObject arrowObj;
    public Transform arrowPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void shoot()
    {
        GameObject arrow= Instantiate(arrowObj,arrowPoint.position,transform.rotation);
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward*25,ForceMode.Impulse);
    }
}
