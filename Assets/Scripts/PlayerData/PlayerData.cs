using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public const int ScrapIndex = 14;

    public List<InventoryItemData> InventoryList;
    public List<WizardData> WizardList;
    public List<EventNode> CurrentEventPath;
    public int CurrentEventPathIndex;
    public float TimeInSecondsPlayed;
    public VehicleData vehicleData;

    //  Vehicle Data

    public PlayerData(List<InventoryItemData> invItemList, List<WizardData> wizList, List<EventNode> events, float timePlayed, int pathIndex, VehicleData vehicleConstellation)
    {
        InventoryList = invItemList;
        WizardList = wizList;
        CurrentEventPath = events;
        TimeInSecondsPlayed = timePlayed;
        CurrentEventPathIndex = pathIndex;
        vehicleData = vehicleConstellation;
    }

    //  Helper Methods
    public static List<EventNode> GenerateTestRoute()
    {
        int enumLength = Enum.GetValues(typeof(NodeEventType)).Length;
        List<EventNode> route = new List<EventNode>();
        for (int i = 0; i < enumLength; i++)
        {
            route.Add(new EventNode(i, false));
        }
        return route;
    }

    public int GetScrap()
    {
        return InventoryList[ScrapIndex].Amount;
    }
    public void SetScrap(int money)
    {
        InventoryList[ScrapIndex].Amount = money; 
    }

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
    public class InventoryItemData
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
    public class WizardData
    {
        public WizardType WizType;
        public int RoomPositionX;
        public int RoomPositionY;
        public int Happiness;
    }

    // Vehicle


    [Serializable]
    public class VehicleData
    {
        public PlayerVehicleMatrix VehicleMatrix;

        public int _savedXSize = 0; //just the amount of Tiles in a given direction
        public int _savedYSize = 0;
        public int VehicleRoomMaxHP;

        public float FloorColorR = 1;
        public float FloorColorG = 1;
        public float FloorColorB = 1;

        public float RoofColorR = 1;
        public float RoofColorG = 1;
        public float RoofColorB = 1;
    }
    [Serializable]
    public class PlayerVehicleMatrix
    {
        public YValues[] XArray = null;
        public PlayerVehicleMatrix(int xLength, int yLength)
        {
            XArray = new YValues[xLength];
            for (int i = 0; i < xLength; i++)
            {
                XArray[i] = new YValues(yLength);
            }
        }
    }
    [Serializable]
    public class YValues
    {
        public RoomInfo[] YStuff = null;
        public YValues(int yLength)
        {
            YStuff = new RoomInfo[yLength];
        }
    }
    [Serializable]
    public enum FloorType
    {
        FloorA,
        FloorB
    }
    [Serializable]
    public enum RoofType
    {
        RoofA,
        RoofB
    }
    [Serializable]
    public class RoomInfo
    {
        public string RoomPrefabPath;
        public int RoomCurrentHP;

        public FloorType FloorType;

        public RoofType RoofType;

        public string MovementPrefabPath;

        public string SystemPrefabPath;

        public ASystem.DirectionToSpawnIn SystemDirection;

        public bool _topWallExists;
        public bool _rightWallExists;
        public bool _bottomWallExists;
        public bool _leftWallExists;

        public static RoomInfo NewRoomInfo()
        {
            return new RoomInfo {
                RoomPrefabPath = "",
                FloorType = FloorType.FloorA,
                RoofType = RoofType.RoofA,
                RoomCurrentHP = -1,
                SystemPrefabPath = "",
                MovementPrefabPath = "",
                SystemDirection = ASystem.DirectionToSpawnIn.Right,
                _topWallExists = false,
                _rightWallExists = false,
                _bottomWallExists = false,
                _leftWallExists = false,
            };
        }
    }
}
