using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public List<InventoryItemData> InventoryList;
    public List<EventNode> CurrentEventPath;

    public PlayerData(List<InventoryItemData> invItemList, List<EventNode> events)
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

        foreach(EventNode node in events)
        {
            EventNode newNode = new EventNode();
            newNode._event = node._event;
            newNode._visited = node._visited;
            CurrentEventPath.Add(newNode);
        }
    }

    //  Structs

    [System.Serializable]
    public struct EventNode
    {
        public EventType _event;
        public bool _visited;

        public EventNode(EventType type, bool visited)
        {
            _event = type;
            _visited = visited;
        }
    }
    [System.Serializable]
    public enum EventType
    {
        Combat,
        Shop,
        Dialogue,
        NewWizard,
        FreeLoot,
        Construction
    }

    [System.Serializable]
    public struct InventoryItemData
    {
        public string Name;
        public string SpritePath;
        public int Amount;
    }
}
