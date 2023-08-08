using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSceneManager : MonoBehaviour
{
    public ShopEventUI _shopEventUI;
    public DialogueEventUI _dialogueEventUI;

    private void Start()
    {
        _dialogueEventUI.Show(false);
        _shopEventUI.Show(false);

        SetScene();
    }

    private void SetScene()
    {
        PlayerData.NodeEventType eventType = DataStorage.Singleton.playerData.CurrentEventPath[DataStorage.Singleton.playerData.CurrentEventPathIndex]._event;

        if (eventType.Equals(PlayerData.NodeEventType.Dialogue))
        {
            _dialogueEventUI.Show(true);
            _dialogueEventUI.Init();
        }
        else if (eventType.Equals(PlayerData.NodeEventType.Shop))
        {
            _shopEventUI.Show(true);
            _shopEventUI.Init();
        }
        else
        {
            //TEMP
            _dialogueEventUI.Show(true);
            _dialogueEventUI.Init();
        }
    }
}
