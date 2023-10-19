using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenUI : MonoBehaviour
{
    public Button _victoryButton;
    private void Awake()
    {
        _victoryButton.onClick.AddListener(() => VictoryButtonEffect());
    }
    private void VictoryButtonEffect()
    {
        //DataStorage.Singleton.playerData.CurrentEventPath = PlayerData.GenerateTestRoute();
        SavePlayerData.DeleteSaveFile(DataStorage.Singleton.saveSlot);
        Loader.Load(Loader.SceneType.MenuScene);
    }
}
