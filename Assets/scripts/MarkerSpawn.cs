using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class MarkerSpawn : MonoBehaviour
{
    public InputDeviceCharacteristics RinputDeviceCharacteristics, LinputDeviceCharacteristics;
    public GameObject Rmarker;
    public GameObject Lmarker;
    public GameObject LeftHandDrawCanvas;
    public GameObject RightHandDrawCanvas;
    public GameObject RightHand;
    public GameObject LeftHand;
    public eraser _eraser;

    private InputDevice RtargetDevice, LtargetDevice;
    private int counter = 0;
    void Start()
    {
        InitializeHand();
    }

    private void InitializeHand()
    {
        List<InputDevice> Rdevices = new List<InputDevice>(), Ldevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(RinputDeviceCharacteristics, Rdevices);
        InputDevices.GetDevicesWithCharacteristics(LinputDeviceCharacteristics, Ldevices);
        if (Rdevices.Count > 0)
        {
            RtargetDevice = Rdevices[0];
            LtargetDevice = Ldevices[0];
            Debug.Log($"target device connected {Rdevices.Count}");
            Debug.Log($"target device connected {Ldevices.Count}");
        }
    }
    void Update()
    {
        if (!RtargetDevice.isValid)
        {
            InitializeHand();
        }
        else
        {
            UpdateHand();
        }
    }

    private void UpdateHand()
    {
        RtargetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool Rres);
        LtargetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool Lres);
        if (Lres == false)
        {
            if (Rres)
            {
                if (counter == 0) //Spawn the draw canvas and marker
                {
                    PullMarkerAndCanvas(Rmarker, LeftHandDrawCanvas);
                }
            }
            else
            {
                if (counter == 1)//DeSpawn the draw canvas and marker
                {
                    BringHandsBack();
                }
            }
        }
        if (Rres == false)
        {
            if (Lres)
            {
                if (counter == 0) //Spawn the draw canvas and marker
                {
                    PullMarkerAndCanvas(Lmarker, RightHandDrawCanvas);
                }
            }
            else
            {
                if (counter == 1)//DeSpawn the draw canvas and marker
                {
                    BringHandsBack();
                }
            }
        }
    }
    private void BringHandsBack()
    {
        counter = 0;
        _eraser.erase();
        Lmarker.SetActive(false);
        RightHandDrawCanvas.SetActive(false);
        Rmarker.SetActive(false);
        LeftHandDrawCanvas.SetActive(false);
        RightHand.SetActive(true);
        LeftHand.SetActive(true);
    }
    private void PullMarkerAndCanvas(GameObject marker, GameObject canvas)
    {
        counter = 1;
        RightHand.SetActive(false);
        LeftHand.SetActive(false);
        marker.SetActive(true);
        canvas.SetActive(true);
    }
}