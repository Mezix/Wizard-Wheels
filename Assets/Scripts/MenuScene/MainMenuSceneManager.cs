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
        REF.mMenu = this;
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
        Instantiate(Resources.Load(GS.Prefabs("LoadingScreen")));
        yield return new WaitForSeconds(0.5f);
        Loader.Load(Loader.Scene.RouteTransitionScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
