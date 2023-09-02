﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static PlayerData;

public class MainMenuSceneTankGeometry : MonoBehaviour
{
    private List<VehicleGeometry> _starterVehicleGeometries;
    private List<VehicleInfo> _starterVehicleStatsList;

    public List<GameObject> _spawnedTanks = new List<GameObject>();
    public int vehicleIndex;

    //  Tank Spawning; all of these are temporary values that get overwritten with each tank

    [HideInInspector]
    public VehicleGeometry _tmpVehicleGeometry;
    public RoomPosition[,] _tmpRoomPosMatrix;
    public GameObject Tmp { get; private set; }
    public GameObject TmpRoomsParent { get; private set; }
    public List<Room> TmpAllRooms { get; private set; }
    public GameObject TmpRoofParent { get; private set; }

    // Extra Display stuff

    public GameObject _vehicleBGOrb;
    public Color _translucentColor;

    private void Awake()
    {
        REF.MMSceneGeometry = this;
    }
    public void InitMMGeometry(bool runStarted)
    {
        _starterVehicleGeometries = new List<VehicleGeometry>();
        _starterVehicleStatsList = new List<VehicleInfo>();
        if (runStarted)
        {
            _starterVehicleGeometries.Add(DataStorage.Singleton.playerData.Geometry);
            _starterVehicleStatsList.Add(DataStorage.Singleton.playerData.Info);
        }
        else
        {
            InitStarterTanks();
        }
        SpawnAllTanks();
        HideAllTanks();
        vehicleIndex = 0;
        REF.mUI.menuSceneTankStats.UpdateSelectedVehicleText(_starterVehicleStatsList[vehicleIndex]);
    }
    private void InitStarterTanks()
    {

        string vehicleType = "StarterVehicle";
        for (int i = 0; i < 3; i++)
        {
            VehicleConstellation StarterVehicleConstellation = Resources.Load(GS.VehicleConstellations(vehicleType + i.ToString()), typeof(VehicleConstellation)) as VehicleConstellation;
            VehicleStats vehicleStats = Resources.Load(GS.VehicleStats(vehicleType + i.ToString() + "stats"), typeof(VehicleStats)) as VehicleStats;

            _starterVehicleGeometries.Add(ConvertVehicleConstellationToVehicleData(StarterVehicleConstellation));
            _starterVehicleStatsList.Add(ConvertVehicleStatsToVehicleInfo( vehicleStats));
        }
    }

    private void SpawnAllTanks()
    {
        foreach (VehicleGeometry geometry in _starterVehicleGeometries)
        {
            _tmpVehicleGeometry = geometry;
            SpawnTankForCreator();
        }
    }
    public void ShowTank(int index)
    {
        HideAllTanks();
        _vehicleBGOrb.gameObject.SetActive(true);
        _spawnedTanks[index].SetActive(true);
    }
    public void HideAllTanks()
    {
        _vehicleBGOrb.gameObject.SetActive(false);
        foreach (GameObject g in _spawnedTanks)
        {
            g.SetActive(false);
        }
    }
    //  UI Stuff

    public void NextTank()
    {
        vehicleIndex++;
        if (vehicleIndex >= _starterVehicleGeometries.Count)
        {
            vehicleIndex = 0;
        }
        SelectVehicle();
    }
    public void PreviousTank()
    {
        vehicleIndex--;
        if (vehicleIndex < 0)
        {
            vehicleIndex = _starterVehicleGeometries.Count - 1;
        }
        SelectVehicle();
    }

    public void SelectVehicle()
    {
        HideAllTanks();
        _spawnedTanks[vehicleIndex].SetActive(true);
        DataStorage.Singleton.playerData.Geometry = _starterVehicleGeometries[vehicleIndex];
        DataStorage.Singleton.playerData.Info = _starterVehicleStatsList[vehicleIndex];
        REF.mUI.menuSceneTankStats.UpdateSelectedVehicleText(_starterVehicleStatsList[vehicleIndex]);
    }
    //  Spawning

    public void SpawnTankForCreator()
    {
        LoadRooms();
        PositionTankObjects();
        CreateWalls();
        CreateTires();
        CreateSystems();
        MakeCollidersTriggers();
        DisableTankUI();
        SetSpriteValues();
    }
    private void LoadRooms()
    {
        _tmpRoomPosMatrix = new RoomPosition[_tmpVehicleGeometry.SavedXSize, _tmpVehicleGeometry.SavedYSize];
        TmpAllRooms = new List<Room>();

        TmpRoomsParent = new GameObject("All Tank Rooms");
        TmpRoomsParent.transform.parent = transform;
        TmpRoomsParent.transform.localPosition = Vector3.zero;
        TmpRoomsParent.transform.localScale = Vector3.one;

        for (int y = 0; y < _tmpVehicleGeometry.SavedYSize; y++)
        {
            for (int x = 0; x < _tmpVehicleGeometry.SavedXSize; x++)
            {
                if (x >= _tmpVehicleGeometry.VehicleMatrix.Columns.Length
                    || y >= _tmpVehicleGeometry.VehicleMatrix.Columns[0].ColumnContent.Length
                    || _tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y] == null
                    || _tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath == null
                    || _tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath == "") continue;
                LoadRoomAtPos(x, y);
            }
        }
    }
    public void LoadRoomAtPos(int x, int y)
    {
        Room room = Instantiate(Resources.Load(_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath, typeof(Room))) as Room;
        room.ID = _tmpVehicleGeometry.GetHashCode();
        room.gameObject.transform.parent = TmpRoomsParent.transform;
        room.gameObject.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
        room.gameObject.transform.localScale = Vector3.one;
        TmpAllRooms.Add(room);

        // Set the Room Positions
        int roomPosNr = 0;
        for (int roomY = 0; roomY < room._sizeY; roomY++)
        {
            for (int roomX = 0; roomX < room._sizeX; roomX++)
            {
                _tmpRoomPosMatrix[x + roomX, y + roomY] = room.allRoomPositions[roomPosNr];
                _tmpRoomPosMatrix[x + roomX, y + roomY]._xPos = x + _tmpRoomPosMatrix[x + roomX, y + roomY]._xRel;
                _tmpRoomPosMatrix[x + roomX, y + roomY]._yPos = y + _tmpRoomPosMatrix[x + roomX, y + roomY]._yRel;

                _tmpRoomPosMatrix[x + roomX, y + roomY].name = "X" + _tmpRoomPosMatrix[x + roomX, y + roomY]._xPos.ToString()
                                                      + " , Y" + _tmpRoomPosMatrix[x + roomX, y + roomY]._yPos.ToString() + ", ";
                roomPosNr++;
            }
        }

        foreach (RoomPosition rPos in room.allRoomPositions) rPos.GetComponent<SpriteRenderer>().enabled = false; // Disable for preview!

        //sets the corner of the room that doesnt get caught with the matrix
        _tmpRoomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1] = room.allRoomPositions[room._sizeX * room._sizeY - 1];
        _tmpRoomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1]._xPos = x + room._sizeX - 1;
        _tmpRoomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1]._yPos = y + room._sizeY - 1;

        _tmpRoomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1].name = "X" + (x + room._sizeX - 1).ToString() + " , Y" + (y + room._sizeY - 1).ToString();



        room._floorType = _tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].FloorType;
        room._roofType = _tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].RoofType;
        _tmpRoomPosMatrix[x, y].ParentRoom._floorRenderer.sprite = Resources.Load(GS.RoomGraphics(_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].FloorType.ToString()) + "3", typeof(Sprite)) as Sprite;
        _tmpRoomPosMatrix[x, y].ParentRoom.ShowFloor(true);
        _tmpRoomPosMatrix[x, y].ParentRoom.ShowRoof(false);
    }
    public void CreateWalls()
    {
        for (int y = 0; y < _tmpVehicleGeometry.SavedYSize; y++)
        {
            for (int x = 0; x < _tmpVehicleGeometry.SavedXSize; x++)
            {
                if (x >= _tmpVehicleGeometry.VehicleMatrix.Columns.Length || y >= _tmpVehicleGeometry.VehicleMatrix.Columns[0].ColumnContent.Length) continue;
                if (_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y]._topWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_tmpRoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y]._rightWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_tmpRoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y]._bottomWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_tmpRoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y]._leftWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallLeft"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_tmpRoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
            }
        }
    }
    public void CreateTires()
    {
        for (int x = 0; x < _tmpVehicleGeometry.SavedXSize; x++)
        {
            for (int y = 0; y < _tmpVehicleGeometry.SavedYSize; y++)
            {
                if (_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath != "")
                {
                    if (_tmpRoomPosMatrix[x, y] == null) continue;

                    GameObject tirePrefab = Instantiate(Resources.Load(_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath, typeof(GameObject))) as GameObject;
                    tirePrefab.transform.parent = _tmpRoomPosMatrix[x, y].transform;
                    tirePrefab.transform.localPosition = Vector3.zero;
                    tirePrefab.transform.localScale = Vector3.one;
                }
            }
        }
    }
    private void PositionTankObjects()
    {
        //create overarching object for all our spawned objects
        Tmp = new GameObject("Tank Geometry Parent");
        _spawnedTanks.Add(Tmp);
        Tmp.transform.parent = gameObject.transform;
        Tmp.transform.localPosition = Vector3.zero;
        Tmp.transform.localScale = Vector3.one;

        // parent all spawnedObjects to this parent
        TmpRoomsParent.transform.parent = Tmp.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        Tmp.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        Tmp.transform.localPosition += new Vector3(-0.25f * _tmpVehicleGeometry.SavedXSize, 0.25f * _tmpVehicleGeometry.SavedYSize, 0);
    }
    public void CreateSystems()
    {
        for (int x = 0; x < _tmpVehicleGeometry.SavedXSize; x++)
        {
            for (int y = 0; y < _tmpVehicleGeometry.SavedYSize; y++)
            {
                if (_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath != "")
                {
                    ASystem system = Instantiate(Resources.Load(_tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath, typeof(ASystem)) as ASystem);
                    system._direction = _tmpVehicleGeometry.VehicleMatrix.Columns[x].ColumnContent[y].SystemDirection;
                    system.transform.parent = _tmpRoomPosMatrix[x, y].transform;
                    system.transform.localPosition = Vector3.zero;
                    system.transform.localScale = Vector3.one;
                    system.SpawnInCorrectDirection();
                }
            }
        }

    }
    private void MakeCollidersTriggers()
    {
        List<Collider2D> colliders = Tmp.GetComponentsInChildren<Collider2D>().ToList();
        foreach (Collider2D c in colliders)
        {
            c.isTrigger = true;
        }
    }

    private void DisableTankUI()
    {
        List<Canvas> canvases = Tmp.GetComponentsInChildren<Canvas>().ToList();
        foreach (Canvas c in canvases)
        {
            c.gameObject.SetActive(false);
        }
    }
    private void SetSpriteValues()
    {
        List<SpriteRenderer> srs = Tmp.GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (SpriteRenderer rend in srs)
        {
            rend.color = Color.white;
            rend.sortingLayerName = "Wizards";
            rend.sortingOrder += 20;
        }
    }
}
