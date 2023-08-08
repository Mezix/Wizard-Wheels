using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopEventUI : MonoBehaviour
{
    public GameObject _allObjects;
    public Button _finishEventButton;

    public void Show(bool show)
    {
        _allObjects.SetActive(show);
    }

    public void Init()
    {
        _finishEventButton.onClick.AddListener(() => DataStorage.Singleton.FinishEvent());
        foreach (PlayerData.InventoryItemData invData in DataStorage.Singleton.playerData.InventoryList)
        {

        }
    }
}
