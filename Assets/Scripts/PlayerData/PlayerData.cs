using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public List<InventoryItemData> InventoryList;

    [System.Serializable]
    public struct InventoryItemData
    {
        public string Name;
        public string SpritePath;
        public int Amount;
    }

    public PlayerData(List<InventoryItemData> invItemList)
    {
        InventoryList = new List<InventoryItemData>();
        foreach (InventoryItemData item in invItemList)
        {
            InventoryItemData newItem = new InventoryItemData();
            newItem.Name = item.Name;
            newItem.SpritePath = item.SpritePath;
            newItem.Amount = item.Amount;
            InventoryList.Add(newItem);
        }
    }
}
