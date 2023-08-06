using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEventUI : MonoBehaviour
{
    public GameObject _allObjects;

    public void Show(bool show)
    {
        _allObjects.SetActive(show);
    }

    public void Init(PlayerData data)
    {
        foreach(PlayerData.InventoryItemData invData in data.InventoryList)
        {

        }
    }
}
