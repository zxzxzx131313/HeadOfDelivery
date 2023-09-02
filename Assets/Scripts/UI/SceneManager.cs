using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void BeginGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void TitleScreen()
    {
        SceneManager.LoadScene("Title");
    }
}
