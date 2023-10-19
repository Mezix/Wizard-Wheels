using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public const int GemIndex = 5;
    public const int MetalBarIndex = 12;
    public const int PlankIndex = 13;
    public const int ScrapIndex = 14;
    public bool RunStarted = false;

    public List<InventoryItemData> InventoryList;
    public List<WizardData> WizardList;
    public List<EventNode> CurrentEventPath;
    public int CurrentEventPathIndex;
    public float TimeInSecondsPlayed;
    public VehicleGeometry Geometry;
    public VehicleInfo Info;

    //  Vehicle Data

    public PlayerData(List<InventoryItemData> invItemList, List<WizardData> wizList, List<EventNode> events, float timePlayed, int pathIndex, VehicleGeometry vehicleGeometry, VehicleInfo vehicleInfo)
    {
        InventoryList = invItemList;
        WizardList = wizList;
        CurrentEventPath = events;
        TimeInSecondsPlayed = timePlayed;
        CurrentEventPathIndex = pathIndex;
        Geometry = vehicleGeometry;
        Info = vehicleInfo;
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
    public class VehicleInfo
    {
        public string TankName;
        public int TankHealth;
        public float TankMaxSpeed;
        public float TankAccel;
        public float TankDecel;
        public float RotationSpeed;
        public string FlavorText;
    }

    [Serializable]
    public class VehicleGeometry
    {
        public VehicleMatrix VehicleMatrix;

        public int SavedXSize = 0; //just the amount of Tiles in a given direction
        public int SavedYSize = 0;
        public int VehicleRoomMaxHP;

        public float FloorColorR = 1;
        public float FloorColorG = 1;
        public float FloorColorB = 1;

        public float RoofColorR = 1;
        public float RoofColorG = 1;
        public float RoofColorB = 1;
    }
    [Serializable]
    public class VehicleMatrix
    {
        public Column[] Columns = null;
        public VehicleMatrix(int xLength, int yLength)
        {
            Columns = new Column[xLength];
            for (int i = 0; i < xLength; i++)
            {
                Columns[i] = new Column(yLength);
            }
        }
    }
    [Serializable]
    public class Column
    {
        public RoomInfo[] ColumnContent = null;
        public Column(int yLength)
        {
            ColumnContent = new RoomInfo[yLength];
        }
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
            return new RoomInfo
            {
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

    //  Helper Methods
    public static List<EventNode> GenerateRandomRoute()
    {
        int enumLength = Enum.GetValues(typeof(NodeEventType)).Length;
        List<EventNode> route = new List<EventNode>();
        List<int> randomNodeIndexList = HM.GetRandomUniqueIntList(enumLength, enumLength);
        for (int i = 0; i < enumLength; i++)
        {
            if(randomNodeIndexList[i] == 1) route.Add(new EventNode(0, false)); //replace dialogue with combat
            else route.Add(new EventNode(randomNodeIndexList[i], false));
        }
        return route;
    }
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
    public int CheckIfInventoryContains(InventoryItemData itemToCheckIfExists)
    {
        int index = 0;
        foreach (InventoryItemData itemToCheck in InventoryList)
        {
            if (itemToCheck.Name == itemToCheckIfExists.Name) return index;
            index++;
        }
        return -1;
    }
    public int GetScrap()
    {
        return InventoryList[ScrapIndex].Amount;
    }
    public void SetScrap(int money)
    {
        InventoryList[ScrapIndex].Amount = money;
    }
    public static VehicleGeometry ConvertVehicleConstellationToVehicleData(VehicleConstellation matrixToCopy)
    {
        VehicleGeometry data = new VehicleGeometry();
        data.SavedXSize = matrixToCopy._savedXSize;
        data.SavedYSize = matrixToCopy._savedYSize;
        data.VehicleRoomMaxHP = 5;

        data.RoofColorR = matrixToCopy.RoofColorR;
        data.RoofColorG = matrixToCopy.RoofColorG;
        data.RoofColorB = matrixToCopy.RoofColorB;

        data.FloorColorR = matrixToCopy.FloorColorR;
        data.FloorColorG = matrixToCopy.FloorColorG;
        data.FloorColorB = matrixToCopy.FloorColorB;

        data.VehicleMatrix = new VehicleMatrix(matrixToCopy._savedXSize, matrixToCopy._savedYSize);

        for (int x = 0; x < data.SavedXSize; x++)
        {
            for (int y = 0; y < data.SavedYSize; y++)
            {
                data.VehicleMatrix.Columns[x].ColumnContent[y] = new RoomInfo();
                data.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath = matrixToCopy._savedMatrix.XArray[x].YStuff[y].RoomPrefabPath;
                data.VehicleMatrix.Columns[x].ColumnContent[y].RoomCurrentHP = 5;

                data.VehicleMatrix.Columns[x].ColumnContent[y].FloorType = matrixToCopy._savedMatrix.XArray[x].YStuff[y].FloorType;

                data.VehicleMatrix.Columns[x].ColumnContent[y].RoofType = matrixToCopy._savedMatrix.XArray[x].YStuff[y].RoofType;

                data.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath = matrixToCopy._savedMatrix.XArray[x].YStuff[y].MovementPrefabPath;

                data.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath = matrixToCopy._savedMatrix.XArray[x].YStuff[y].SystemPrefabPath;
                data.VehicleMatrix.Columns[x].ColumnContent[y].SystemDirection = matrixToCopy._savedMatrix.XArray[x].YStuff[y].SystemDirection;

                data.VehicleMatrix.Columns[x].ColumnContent[y]._topWallExists = matrixToCopy._savedMatrix.XArray[x].YStuff[y]._topWallExists;
                data.VehicleMatrix.Columns[x].ColumnContent[y]._rightWallExists = matrixToCopy._savedMatrix.XArray[x].YStuff[y]._rightWallExists;
                data.VehicleMatrix.Columns[x].ColumnContent[y]._bottomWallExists = matrixToCopy._savedMatrix.XArray[x].YStuff[y]._bottomWallExists;
                data.VehicleMatrix.Columns[x].ColumnContent[y]._leftWallExists = matrixToCopy._savedMatrix.XArray[x].YStuff[y]._leftWallExists;
            }
        }

        return data;
    }
    public static VehicleInfo ConvertVehicleStatsToVehicleInfo(VehicleStats stats)
    {
        return new VehicleInfo()
        {
            TankName = stats._tankName,
            TankHealth = stats._tankHealth,
            TankMaxSpeed = stats._tankMaxSpeed,
            TankAccel = stats._tankAccel,
            TankDecel = stats._tankDecel,
            RotationSpeed = stats._rotationSpeed,
            FlavorText = stats._flavorText
        };
    }
    public static void CopyVehicleDataFromTo(VehicleGeometry CopyFrom, ref VehicleGeometry CopyTo)
    {
        CopyTo = new VehicleGeometry
        {
            SavedXSize = CopyFrom.SavedXSize,
            SavedYSize = CopyFrom.SavedYSize,
            VehicleRoomMaxHP = CopyFrom.VehicleRoomMaxHP,

            RoofColorR = CopyFrom.RoofColorR,
            RoofColorG = CopyFrom.RoofColorG,
            RoofColorB = CopyFrom.RoofColorB,

            FloorColorR = CopyFrom.FloorColorR,
            FloorColorG = CopyFrom.FloorColorG,
            FloorColorB = CopyFrom.FloorColorB,

            VehicleMatrix = new VehicleMatrix(CopyFrom.VehicleMatrix.Columns.Length, CopyFrom.VehicleMatrix.Columns[0].ColumnContent.Length)
        };

        for (int x = 0; x < CopyFrom.VehicleMatrix.Columns.Length; x++)
        {
            for (int y = 0; y < CopyFrom.VehicleMatrix.Columns[x].ColumnContent.Length; y++)
            {
                if (CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y] == null) continue;

                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y] = new RoomInfo();
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y].RoomCurrentHP = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y].RoomCurrentHP;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y].RoofType = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y].RoofType;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y].FloorType = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y].FloorType;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y].SystemDirection = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y].SystemDirection;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath;

                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y]._topWallExists = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y]._topWallExists;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y]._bottomWallExists = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y]._bottomWallExists;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y]._leftWallExists = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y]._leftWallExists;
                CopyTo.VehicleMatrix.Columns[x].ColumnContent[y]._rightWallExists = CopyFrom.VehicleMatrix.Columns[x].ColumnContent[y]._rightWallExists;
            }
        }
    }
}
