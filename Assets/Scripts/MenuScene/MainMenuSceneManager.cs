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
        wiz.movementLocked = true;
    }
    private void Start()
    {
        REF.mUI.ShowMMUI();
    }

    public void ContinueRun()
    {
        DataStorage.Singleton.playerData.RunStarted = true;
        Loader.Load(Loader.SceneType.RouteTransitionScene);
    }
    public void ResetRun()
    {

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
