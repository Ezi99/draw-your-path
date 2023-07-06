using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject StartLayer;
    public GameObject AboutLayer;

    public void StartButton()
    {
        SceneManager.LoadScene("TheGame");
    }

    public void AboutButton()
    {
        StartLayer.SetActive(false);
        AboutLayer.SetActive(true);
    }

    public void BackButton()
    {
        AboutLayer.SetActive(false);
        StartLayer.SetActive(true);
    }
}
