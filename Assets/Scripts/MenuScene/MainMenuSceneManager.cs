using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{
    //  Player
    public OverworldWizard wiz;
    public GameObject orb;
    public MainMenuSceneTankPreview _mmTankPreview;

    private void Awake()
    {
        Ref.mMenu = this;
    }
    private void Start()
    {
        wiz.movementLocked = true;
    }
    private void Update()
    {
    }
    

    public void LaunchGame()
    {
        StartCoroutine(ShowLoadingScreen());
    }

    public IEnumerator ShowLoadingScreen()
    {
        Instantiate((GameObject) Resources.Load("LoadingScreen"));
        yield return new WaitForSeconds(0.5f);
        LevelManager.playerTankConstellationFromSelectScreen = _mmTankPreview._playerTankConstellations[_mmTankPreview.tankIndex];
        Loader.Load(Loader.Scene.GameScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
