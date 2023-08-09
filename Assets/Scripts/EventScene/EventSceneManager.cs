using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSceneManager : MonoBehaviour
{
    public ShopEventUI _shopEventUI;
    public DialogueEventUI _dialogueEventUI;
    public FreeLootEventUI _freeLootEventUI;
    public FreeWizardEventUI _freeWizardEventUI;

    private void Start()
    {
        _dialogueEventUI.Show(false);
        _shopEventUI.Show(false);
        _freeLootEventUI.Show(false);
        _freeWizardEventUI.Show(false);
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
        else if (eventType.Equals(PlayerData.NodeEventType.FreeLoot))
        {
            _freeLootEventUI.Show(true);
            _freeLootEventUI.Init();
        }
        else if (eventType.Equals(PlayerData.NodeEventType.FreeWizard))
        {
            _freeWizardEventUI.Show(true);
            _freeWizardEventUI.Init();
        }
        else
        {
            _dialogueEventUI.Show(true);
            _dialogueEventUI.Init();
        }
    }
}
