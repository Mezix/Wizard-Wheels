using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public List<InventoryItemData> InventoryList;
    public List<WizardData> WizardList;
    public List<EventNode> CurrentEventPath;
    public int CurrentEventPathIndex;
    public float TimeInSecondsPlayed;

    public PlayerData(List<InventoryItemData> invItemList, List<WizardData> wizList, List<EventNode> events, float timePlayed, int pathIndex)
    {
        InventoryList = invItemList;
        WizardList = wizList;
        CurrentEventPath = events;
        TimeInSecondsPlayed = timePlayed;
        CurrentEventPathIndex = pathIndex;

        /*InventoryList = new List<InventoryItemData>();
        foreach (InventoryItemData item in invItemList)
        {
            InventoryItemData newItem = new InventoryItemData();
            newItem.Name = item.Name;
            newItem.SpritePath = item.SpritePath;
            newItem.Amount = item.Amount;
            InventoryList.Add(newItem);
        }
        CurrentEventPath = new List<EventNode>();
        foreach (EventNode node in events)
        {
            EventNode newNode = new EventNode();
            newNode._event = node._event;
            newNode._visited = node._visited;
            CurrentEventPath.Add(newNode);
        }*/
    }

    //  Helper Methods
    public static List<EventNode> GenerateRandomRoute(int length)
    {
        int enumLength = Enum.GetValues(typeof(NodeEventType)).Length;

        List<EventNode> route = new List<EventNode>();
        for (int i = 0; i < length; i++)
        {
            int randomEnum = UnityEngine.Random.Range(0, enumLength);
            if (i == length - 1)
            {
                route.Add(new EventNode(NodeEventType.Shop, false));
                continue;
            }
            else
            {
                route.Add(new EventNode(randomEnum, false));
            }
        }
        return route;
    }
    
    public int CheckIfInventoryContains(InventoryItemData itemToCheckIfExists)
    {
        int index = 0;
        foreach(InventoryItemData itemToCheck in InventoryList)
        {  
            if (itemToCheck.Name == itemToCheckIfExists.Name) return index;
            index++;
        }
        return -1;
    }

    //  Structs

    [Serializable]
    public struct EventNode
    {
        public NodeEventType _event;
        public bool _visited;

        public EventNode(NodeEventType type, bool visited)
        {
            _event = type;
            _visited = visited;
        }
        public EventNode(int index, bool visited)
        {
            _event = Enum.GetValues(typeof(NodeEventType)).Cast<NodeEventType>().ToList()[index];
            _visited = visited;
        }
    }
    [Serializable]
    public enum NodeEventType
    {
        Combat,
        Dialogue,
        FreeWizard,
        FreeLoot,
        Construction,
        Shop
    }

    [Serializable]
    public struct InventoryItemData
    {
        public string Name;
        public string SpritePath;
        public int Amount;
    }

    [Serializable]
    public enum WizardType
    {
        TechWizard,
        FirstAidFiddler,
        PotionPeddler,
        PortalPriest,
        WoodlandWanderer,
        ImmolatingImp
    }
    [Serializable]
    public struct WizardData
    {
        public WizardType WizType;
        public int RoomPositionX;
        public int RoomPositionY;
    }
}
