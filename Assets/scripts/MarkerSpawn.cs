using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class MarkerSpawn : MonoBehaviour
{
    public InputDeviceCharacteristics RinputDeviceCharacteristics, LinputDeviceCharacteristics;
    public ActionBasedContinuousMoveProvider playerMovement;
    public GameObject RightItemMarker;
    public GameObject RightSpellMarker;
    public GameObject LeftItemMarker;
    public GameObject LeftSpellMarker;
    public GameObject LeftHandDrawCanvas;
    public GameObject RightHandDrawCanvas;
    public GameObject RightHand;
    public GameObject LeftHand;
    public Eraser Eraser;
    private float saveSpeed;
    private InputDevice RtargetDevice, LtargetDevice;
    private bool drewItem = false;
    private bool drewSpell = false;

    void Start()
    {
        InitializeHand();
        saveSpeed = playerMovement.moveSpeed;
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
            Debug.Log($"target device connected in MarkerSpawn {Rdevices.Count}");
            Debug.Log($"target device connected in MarkerSpawn {Ldevices.Count}");
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
        RtargetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool RightPrimaryRes);
        LtargetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool LeftPrimaryRes);
        RtargetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool RightSecondaryRes);
        LtargetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool LeftSecondaryRes);

        if (RightPrimaryRes == true && RightSecondaryRes == false && LeftPrimaryRes == false && LeftSecondaryRes == false)
        {
            if (drewItem == false)
            {
                PullMarkerAndCanvas(RightItemMarker, LeftHandDrawCanvas);
            }
        }
        else if (LeftPrimaryRes == true && LeftSecondaryRes == false && RightPrimaryRes == false && RightSecondaryRes == false)
        {
            if (drewItem == false)
            {
                PullMarkerAndCanvas(LeftItemMarker, RightHandDrawCanvas);
            }
        }
        else if (RightSecondaryRes == true && RightPrimaryRes == false && LeftSecondaryRes == false && LeftPrimaryRes == false)
        {
            if (drewSpell == false)
            {
                PullMarkerAndCanvas(RightSpellMarker, LeftHandDrawCanvas);
            }
        }
        else if (LeftSecondaryRes == true && LeftPrimaryRes == false && RightPrimaryRes == false && RightSecondaryRes == false)
        {
            if (drewSpell == false)
            {
                PullMarkerAndCanvas(LeftSpellMarker, RightHandDrawCanvas);
            }
        }
        else if (LeftSecondaryRes == false && RightSecondaryRes == false && LeftPrimaryRes == false && RightPrimaryRes == false)
        {
            if (drewItem == true || drewSpell == true)
            {
                BringHandsBack();
            }
        }
    }

    private void BringHandsBack()
    {
        playerMovement.moveSpeed = saveSpeed;
        drewItem = false;
        drewSpell = false;
        LeftItemMarker.SetActive(false);
        LeftSpellMarker.SetActive(false);
        RightHandDrawCanvas.SetActive(false);
        RightItemMarker.SetActive(false);
        RightSpellMarker.SetActive(false);
        LeftHandDrawCanvas.SetActive(false);
        RightHand.SetActive(true);
        LeftHand.SetActive(true);
    }

    private void PullMarkerAndCanvas(GameObject marker, GameObject canvas)
    {
        Eraser.erase();
        playerMovement.moveSpeed = 0;
        if (marker == LeftItemMarker || marker == RightItemMarker)
        {
            drewItem = true;
        }
        else
        {
            drewSpell = true;
        }

        RightHand.SetActive(false);
        LeftHand.SetActive(false);
        marker.SetActive(true);
        canvas.SetActive(true);
        Eraser.erase();
    }
}