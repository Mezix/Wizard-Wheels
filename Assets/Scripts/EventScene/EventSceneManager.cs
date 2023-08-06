using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSceneManager : MonoBehaviour
{
    public ShopEventUI _shopEventUI;
    public DialogueEventUI _dialogueEventUI;
    public PlayerData playerData;

    private void Start()
    {
        _dialogueEventUI.Show(false);
        _shopEventUI.Show(false);

        playerData = SavePlayerData.LoadPlayer(0);
        SetScene();
    }

    private void SetScene()
    {
        for(int i = 0; i < playerData.CurrentEventPath.Count; i++)
        {
            if (playerData.CurrentEventPath[i]._visited) continue;

            if (playerData.CurrentEventPath[i]._event.Equals(PlayerData.EventType.Dialogue))
            {
                _dialogueEventUI.Show(true);
                if(i < playerData.CurrentEventPath.Count - 1) _dialogueEventUI.Init(playerData.CurrentEventPath[i+1]._event);
                break;
            }
            else if (playerData.CurrentEventPath[i]._event.Equals(PlayerData.EventType.Shop))
            {
                _shopEventUI.Show(true);
                if (i < playerData.CurrentEventPath.Count - 1) _shopEventUI.Init(playerData);
                break;
            }
        }
    }
}
