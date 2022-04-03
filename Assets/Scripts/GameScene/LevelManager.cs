﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static TankRoomConstellation playerTankConstellationFromSelectScreen;


    private void Awake()
    {
        instance = this;
        Events.instance.PlayerTankDestroyed += GameOver;

    }
    private void Start()
    {
        if (playerTankConstellationFromSelectScreen)
        {
            Ref.PCon.TGeo._tankRoomConstellation = playerTankConstellationFromSelectScreen;
        }
        Ref.PCon.SpawnTank();
    }

    //  Random Gen

    private void GameOver()
    {
        Ref.UI.SpawnGameOverScreen();
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
