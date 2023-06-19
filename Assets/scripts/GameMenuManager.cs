using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class GameMenuManager : MonoBehaviour
{
    public Transform m_PlayersHead;
    public float m_SpawnDistance;
    public GameObject m_Menu;
    private InputDevice m_LeftTargetDevice;
    public InputDeviceCharacteristics LinputDeviceCharacteristics;
    private bool m_CheckIfMenuButtonPressed = true;

    private void Start()
    {
        InitializeHand();
    }

    private void InitializeHand()
    {
        List<InputDevice>  Ldevices = new List<InputDevice>();
        LinputDeviceCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(LinputDeviceCharacteristics, Ldevices);
        if (Ldevices.Count > 0)
        {
            m_LeftTargetDevice = Ldevices[0];
            Debug.Log($"target device connected in GameMenuManager {Ldevices.Count}");
        }
    }

    void Update()
    {
        if (!m_LeftTargetDevice.isValid)
        {
            InitializeHand();
        }
        else
        {
            CheckIfMenuButtonPressed();
        }

    }

    private void CheckIfMenuButtonPressed()
    {
        m_LeftTargetDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool IsMenuButton);

        if (IsMenuButton == true && m_CheckIfMenuButtonPressed == true)
        {
            m_Menu.SetActive(!m_Menu.activeSelf);
            m_Menu.transform.position = m_PlayersHead.position + new Vector3(m_PlayersHead.forward.x, 0, m_PlayersHead.forward.z).normalized * m_SpawnDistance;
            m_CheckIfMenuButtonPressed = false;
            Invoke("MenuButtonCooldown", 0.5f);
        }
        
        m_Menu.transform.LookAt(new Vector3(m_PlayersHead.position.x, m_Menu.transform.position.y, m_PlayersHead.position.z));
        m_Menu.transform.forward *= -1;

    }

    private void MenuButtonCooldown()
    {
        m_CheckIfMenuButtonPressed = true;
    }
}
