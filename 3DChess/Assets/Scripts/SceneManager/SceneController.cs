using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadSceneAtIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
