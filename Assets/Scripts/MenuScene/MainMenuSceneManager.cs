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
    public MainMenuSceneTankGeometry _mmTankPreview;

    private void Awake()
    {
        REF.mMenu = this;
    }
    private void Start()
    {
        wiz.movementLocked = true;
        REF.mUI.InitMMUI(DataStorage.Singleton.playerData.RunStarted);
        REF.MMSceneGeometry.InitMMGeometry(DataStorage.Singleton.playerData.RunStarted);
    }
    public void ContinueRun()
    {
        DataStorage.Singleton.playerData.RunStarted = true;
        Loader.Load(Loader.SceneType.RouteTransitionScene);
    }
    public void StartNewRun()
    {
        DataStorage.Singleton.playerData.RunStarted = true;
        SavePlayerData.SavePlayer(DataStorage.Singleton.saveSlot, DataStorage.Singleton.playerData);
        Loader.Load(Loader.SceneType.RouteTransitionScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
