using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingDummy : MonoBehaviour
{
    public int timer = 10;
    public Animator animator;
    private bool block = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Invoke("Block", 10);
    }
    private void Block()
    {

        animator.SetTrigger("Block");
        block = false;

    }

}
