using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject StartLayer;
    public GameObject AboutLayer;
    private AudioSource clickSound;

    private void Start()
    {
        clickSound = GetComponent<AudioSource>();
    }

    public void StartButton()
    {
        clickSound.Play();
        SceneManager.LoadScene("TheGame");
    }

    public void AboutButton()
    {
        clickSound.Play();
        StartLayer.SetActive(false);
        AboutLayer.SetActive(true);
    }

    public void BackButton()
    {
        clickSound.Play();
        AboutLayer.SetActive(false);
        StartLayer.SetActive(true);
    }
}
