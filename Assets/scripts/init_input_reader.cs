using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class init_input_reader : MonoBehaviour
{
    List<InputDevice> inputDevices = new List<InputDevice>();
    void Start()
    {
        InitializeInputReader();
    }
    void InitializeInputReader()
    {
        InputDevices.GetDevices(inputDevices);
        foreach (var inputDevice in inputDevices) 
        {
            Debug.Log(inputDevice.name + " " + inputDevice.characteristics);
        }
    }
    private void Update()
    {
        if(inputDevices.Count < 2) 
        {
            InitializeInputReader();
        }
    }
}
