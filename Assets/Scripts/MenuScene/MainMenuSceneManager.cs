using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {

    }

    public void StartGame()
    {
        StartCoroutine(ShowLoadingScreen());
    }

    public IEnumerator ShowLoadingScreen()
    {
        Instantiate((GameObject) Resources.Load("LoadingScreen"));
        yield return new WaitForSeconds(0.5f);
        Loader.Load(Loader.Scene.GameScene);
    }

    public void LaunchSettings()
    {
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
