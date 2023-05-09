using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGate : MonoBehaviour
{
    public int potionCounter;
    private Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int velocity)
    {
        if(potionCounter == 0 && velocity > 4)
        {
            Debug.Log("WE HERE");
            rigidBody.isKinematic = false;
            rigidBody.useGravity = true;
            rigidBody.AddForce(100 * velocity * transform.forward);
        }
    }

    public void PotionTaken()
    {
        potionCounter--;
    }
}
