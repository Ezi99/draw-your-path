using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("TheGame");
    }
}
