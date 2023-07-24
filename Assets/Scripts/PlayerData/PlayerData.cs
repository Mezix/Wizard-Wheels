using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public Dictionary<InventoryItem, int> InventoryToItemAmountDictionary;

    public PlayerData(Dictionary<InventoryItem, int> inventoryItems)
    {
        foreach (InventoryItem item in inventoryItems.Keys)
        {
            InventoryItem newItem = new InventoryItem();
            newItem.Name = item.Name;
            newItem.Image = item.Image;
            InventoryToItemAmountDictionary.Add(newItem, inventoryItems[item]);
        }
    }
}
