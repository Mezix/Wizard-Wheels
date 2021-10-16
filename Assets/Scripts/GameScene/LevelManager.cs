using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private void Awake()
    {
        instance = this;
        Events.instance.PlayerTankDestroyed += GameOver;
    }

    private void GameOver()
    {
        References.UI.SpawnGameOverScreen();
    }

    public void GoToMainMenu()
    {
        StartCoroutine(ShowLoadingScreen());
    }

    public IEnumerator ShowLoadingScreen()
    {
        Instantiate((GameObject)Resources.Load("LoadingScreen"));
        Time.timeScale = 1;
        yield return new WaitForSeconds(2f);
        Loader.Load(Loader.Scene.MenuScene);
    }

}
