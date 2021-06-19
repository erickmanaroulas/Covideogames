using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader: MonoBehaviour
{
    int currentScene;
    int nextScene;
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        nextScene = currentScene + 1;
 
    }
    public  void  LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadNextScene()
    {
        if (nextScene >= SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(0);
        else SceneManager.LoadScene(nextScene);
    }

    public void Reload()
    {
        SceneManager.LoadScene(currentScene);
    }
    public void QuitApplication(){
        Application.Quit();
    }
}
