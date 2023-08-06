﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static TankRoomConstellation playerTankConstellationFromSelectScreen;
    public PlayerData playerData;
    public int saveSlot = 0;

    private void Awake()
    {
        instance = this;
        Events.instance.PlayerTankDestroyed += GameOver;
    }
    private void Start()
    {
        playerData = SavePlayerData.LoadPlayer(saveSlot);
        REF.InvUI.SpawnInventory(playerData);

        if (playerTankConstellationFromSelectScreen)
        {
            REF.PCon.TGeo._tankRoomConstellation = playerTankConstellationFromSelectScreen;
        }
        REF.PCon.SpawnTank();
    }

    //  Random Gen

    private void GameOver()
    {
        REF.UI.SpawnGameOverScreen();
    }
    public void GoToMainMenu()
    {
        StartCoroutine(ShowLoadingScreen());
    }
    public IEnumerator ShowLoadingScreen()
    {
        Instantiate(Resources.Load(GS.Prefabs( "LoadingScreen"), typeof (GameObject)) as GameObject);
        Time.timeScale = 1;
        yield return new WaitForSeconds(2f);
        Loader.Load(Loader.Scene.MenuScene);
    }

    public void CombatHasBeenWon()
    {
        SavePlayerData.SavePlayer(saveSlot, REF.InvUI.SceneInventoryList, playerData.CurrentEventPath);

        Instantiate(Resources.Load(GS.UIPrefabs("CombatVictoryScreen"), typeof(GameObject)) as GameObject, REF.UI.transform, false);
        REF.TM.TriggerGradualSlowdown(0.2f);
    }
}
