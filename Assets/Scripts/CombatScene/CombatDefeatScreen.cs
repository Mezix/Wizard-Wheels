using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatDefeatScreen : MonoBehaviour
{
    public Button _returnToMainMenuButton;
    private void Awake()
    {
        _returnToMainMenuButton.onClick.AddListener(() => AcceptDefeat());
    }

    private void AcceptDefeat()
    {
        SavePlayerData.DeleteSaveFile(DataStorage.Singleton.saveSlot);
        CombatSceneManager.instance.GoToMainMenu();
    }
}
