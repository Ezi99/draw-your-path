using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingDummy : MonoBehaviour
{
    public int timer = 10;
    public Animator animator;
    private bool block = false;
    
    void Update()
    {

        if (block == false)
        {
            Invoke("Block", timer);
            block = true;
        }
    }
    private void Block()
    {

        animator.SetTrigger("Block");
        block = false;

    }

}
