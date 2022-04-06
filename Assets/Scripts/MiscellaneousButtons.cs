using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Various methods that are useful for buttons like opening links and loading scenes.
/// </summary>
public class MiscellaneousButtons : MonoBehaviour
{
    public void OpenALink(string link)
    {
        Application.OpenURL(link);
    }



    public void LoadAScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadASceneIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
