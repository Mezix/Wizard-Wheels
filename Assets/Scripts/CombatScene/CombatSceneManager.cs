using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static PlayerData;

public class CombatSceneManager : MonoBehaviour
{
    public static CombatSceneManager instance;

    private void Awake()
    {
        instance = this;
        Events.instance.PlayerTankDestroyed += GameOver;
    }
    private void Start()
    {
        REF.InvUI.SpawnInventory(DataStorage.Singleton.playerData);
        DataStorage.CopyVehicleDataFromTo(DataStorage.Singleton.playerData.vehicleData, ref REF.PCon.TGeo._vehicleData);
        REF.PCon.SpawnTank();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
        }
    }
    //  Random Gen

    private void GameOver()
    {
        REF.CombatUI.SpawnGameOverScreen();
    }
    public void GoToMainMenu()
    {
        StartCoroutine(ShowLoadingScreen());
    }
    public IEnumerator ShowLoadingScreen()
    {
        //Instantiate(Resources.Load(GS.Prefabs( "LoadingScreen"), typeof (GameObject)) as GameObject);
        Time.timeScale = 1;
        yield return new WaitForSeconds(2f);
        Loader.Load(Loader.SceneType.MenuScene);
    }

    public void CombatHasBeenWon()
    {
        SavePlayerData.SavePlayer(DataStorage.Singleton.saveSlot, DataStorage.Singleton.playerData);
        Instantiate(Resources.Load(GS.UIPrefabs("CombatVictoryScreen"), typeof(GameObject)) as GameObject, REF.CombatUI.transform, false);
        REF.TM.TriggerGradualSlowdown(0.2f);
    }
}
