using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static PlayerData;

public class MainMenuSceneTankGeometry : MonoBehaviour
{
    private List<VehicleGeometry> _starterVehicleGeometries = new List<VehicleGeometry>();
    private List<VehicleInfo> _starterVehicleStatsList = new List<VehicleInfo>();
    private List<GameObject> _spawnedVehicles = new List<GameObject>();
    public int vehicleSelectIndex;

    private VehicleGeometry _playerDataVehicleGeometry;
    private VehicleInfo _playerDataVehicleInfo;

    //  Tank Spawning; all of these are temporary values that get overwritten with each tank

    public RoomPosition[,] _tmpRoomPosMatrix;
    public GameObject TmpVehicleParent { get; private set; }
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
    private void Start()
    {
        InitStarterTanks();
        SpawnStarterTanks();
        HideAllVehicles();
        vehicleSelectIndex = 0;
    }
    public void InitPlayerDataVehicle()
    {
        int nrOfStarters = _starterVehicleGeometries.Count;
        if (_spawnedVehicles.Count == nrOfStarters + 1)
        {
            Destroy(_spawnedVehicles[nrOfStarters]);
            _spawnedVehicles.RemoveAt(nrOfStarters);
        }

        _playerDataVehicleGeometry = DataStorage.Singleton.playerData.Geometry;
        _playerDataVehicleInfo = DataStorage.Singleton.playerData.Info;
        SpawnTankForCreator(_playerDataVehicleGeometry);

        if (DataStorage.Singleton.playerData.RunStarted) vehicleSelectIndex = nrOfStarters;
        else vehicleSelectIndex = 0;
        SelectVehicle();
    }
    private void InitStarterTanks()
    {
        string vehicleType = "StarterVehicle";
        for (int i = 0; i < 3; i++)
        {
            VehicleConstellation StarterVehicleConstellation = Resources.Load(GS.VehicleConstellations(vehicleType + i.ToString()), typeof(VehicleConstellation)) as VehicleConstellation;
            VehicleStats vehicleStats = Resources.Load(GS.VehicleStats(vehicleType + i.ToString() + "stats"), typeof(VehicleStats)) as VehicleStats;

            _starterVehicleGeometries.Add(ConvertVehicleConstellationToVehicleData(StarterVehicleConstellation));
            _starterVehicleStatsList.Add(ConvertVehicleStatsToVehicleInfo(vehicleStats));
        }
    }
    private void SpawnStarterTanks()
    {
        foreach (VehicleGeometry geometry in _starterVehicleGeometries)
        {
            SpawnTankForCreator(geometry);
        }
    }
    public void ShowVehicle(int index)
    {
        HideAllVehicles();
        _spawnedVehicles[index].SetActive(true);
    }
    public void HideAllVehicles()
    {
        foreach (GameObject g in _spawnedVehicles)
        {
            g.SetActive(false);
        }
    }
    //  UI Stuff

    public void NextTank()
    {
        vehicleSelectIndex++;
        if (vehicleSelectIndex >= _starterVehicleGeometries.Count)
        {
            vehicleSelectIndex = 0;
        }
        SelectVehicle();
    }
    public void PreviousTank()
    {
        vehicleSelectIndex--;
        if (vehicleSelectIndex < 0)
        {
            vehicleSelectIndex = _starterVehicleGeometries.Count - 1;
        }
        SelectVehicle();
    }
    public void ShowOrbBG(bool show)
    {
        _vehicleBGOrb.gameObject.SetActive(show);
    }
    public void SelectVehicle()
    {
        HideAllVehicles();
        ShowVehicle(vehicleSelectIndex);
        _spawnedVehicles[vehicleSelectIndex].SetActive(true);
        if(vehicleSelectIndex < _starterVehicleGeometries.Count)
        {
            DataStorage.Singleton.playerData.Geometry = _starterVehicleGeometries[vehicleSelectIndex];
            DataStorage.Singleton.playerData.Info = _starterVehicleStatsList[vehicleSelectIndex];
            REF.mUI.menuSceneTankStats.UpdateNewRunVehicleText(_starterVehicleStatsList[vehicleSelectIndex]);
        }
        else
        {
            REF.mUI.menuSceneTankStats.UpdateCurrentRunVehicleStats(DataStorage.Singleton.playerData);
        }
    }
    //  Spawning

    public void SpawnTankForCreator(VehicleGeometry geometry)
    {
        LoadRooms(geometry);
        PositionTankObjects(geometry);
        CreateWalls(geometry);
        CreateTires(geometry);
        CreateSystems(geometry);
        MakeCollidersTriggers();
        DisableTankUI();
        SetSpriteValues();
    }
    private void LoadRooms(VehicleGeometry geometry)
    {
        _tmpRoomPosMatrix = new RoomPosition[geometry.SavedXSize, geometry.SavedYSize];
        TmpAllRooms = new List<Room>();

        TmpRoomsParent = new GameObject("All Tank Rooms");
        TmpRoomsParent.transform.parent = transform;
        TmpRoomsParent.transform.localPosition = Vector3.zero;
        TmpRoomsParent.transform.localScale = Vector3.one;

        for (int y = 0; y < geometry.SavedYSize; y++)
        {
            for (int x = 0; x < geometry.SavedXSize; x++)
            {
                if (x >= geometry.VehicleMatrix.Columns.Length
                    || y >= geometry.VehicleMatrix.Columns[0].ColumnContent.Length
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y] == null
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath == null
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath == "") continue;
                LoadRoomAtPos(x, y, geometry);
            }
        }
    }
    public void LoadRoomAtPos(int x, int y, VehicleGeometry geometry)
    {
        Room room = Instantiate(Resources.Load(geometry.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath, typeof(Room))) as Room;
        room.ID = geometry.GetHashCode();
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



        room._floorType = geometry.VehicleMatrix.Columns[x].ColumnContent[y].FloorType;
        room._roofType = geometry.VehicleMatrix.Columns[x].ColumnContent[y].RoofType;
        _tmpRoomPosMatrix[x, y].ParentRoom._floorRenderer.sprite = Resources.Load(GS.RoomGraphics(geometry.VehicleMatrix.Columns[x].ColumnContent[y].FloorType.ToString()) + "3", typeof(Sprite)) as Sprite;
        _tmpRoomPosMatrix[x, y].ParentRoom.ShowFloor(true);
        _tmpRoomPosMatrix[x, y].ParentRoom.ShowRoof(false);
    }
    public void CreateWalls(VehicleGeometry geometry)
    {
        for (int y = 0; y < geometry.SavedYSize; y++)
        {
            for (int x = 0; x < geometry.SavedXSize; x++)
            {
                if (x >= geometry.VehicleMatrix.Columns.Length || y >= geometry.VehicleMatrix.Columns[0].ColumnContent.Length) continue;
                if (geometry.VehicleMatrix.Columns[x].ColumnContent[y]._topWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_tmpRoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (geometry.VehicleMatrix.Columns[x].ColumnContent[y]._rightWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_tmpRoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (geometry.VehicleMatrix.Columns[x].ColumnContent[y]._bottomWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_tmpRoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (geometry.VehicleMatrix.Columns[x].ColumnContent[y]._leftWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallLeft"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_tmpRoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
            }
        }
    }
    public void CreateTires(VehicleGeometry geometry)
    {
        for (int x = 0; x < geometry.SavedXSize; x++)
        {
            for (int y = 0; y < geometry.SavedYSize; y++)
            {
                if (x >= geometry.VehicleMatrix.Columns.Length
                    || y >= geometry.VehicleMatrix.Columns[0].ColumnContent.Length
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y] == null
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath == null
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath == "") continue;

                GameObject tirePrefab = Instantiate(Resources.Load(geometry.VehicleMatrix.Columns[x].ColumnContent[y].MovementPrefabPath, typeof(GameObject))) as GameObject;
                tirePrefab.transform.parent = _tmpRoomPosMatrix[x, y].transform;
                tirePrefab.transform.localPosition = Vector3.zero;
                tirePrefab.transform.localScale = Vector3.one;
            }
        }
    }
    private void PositionTankObjects(VehicleGeometry geometry)
    {
        //create overarching object for all our spawned objects
        TmpVehicleParent = new GameObject("Tank Geometry Parent");
        _spawnedVehicles.Add(TmpVehicleParent);
        TmpVehicleParent.transform.parent = gameObject.transform;
        TmpVehicleParent.transform.localPosition = Vector3.zero;
        TmpVehicleParent.transform.localScale = Vector3.one;

        // parent all spawnedObjects to this parent
        TmpRoomsParent.transform.parent = TmpVehicleParent.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        TmpVehicleParent.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        TmpVehicleParent.transform.localPosition += new Vector3(-0.25f * geometry.SavedXSize, 0.25f * geometry.SavedYSize, 0);
    }
    public void CreateSystems(VehicleGeometry geometry)
    {
        for (int x = 0; x < geometry.SavedXSize; x++)
        {
            for (int y = 0; y < geometry.SavedYSize; y++)
            {
                if (x >= geometry.VehicleMatrix.Columns.Length
                    || y >= geometry.VehicleMatrix.Columns[0].ColumnContent.Length
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y] == null
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath == null
                    || geometry.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath == "") continue;

                ASystem system = Instantiate(Resources.Load(geometry.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath, typeof(ASystem)) as ASystem);
                system._direction = geometry.VehicleMatrix.Columns[x].ColumnContent[y].SystemDirection;
                system.transform.parent = _tmpRoomPosMatrix[x, y].transform;
                system.transform.localPosition = Vector3.zero;
                system.transform.localScale = Vector3.one;
                system.SpawnInCorrectDirection();
            }
        }

    }
    private void MakeCollidersTriggers()
    {
        List<Collider2D> colliders = TmpVehicleParent.GetComponentsInChildren<Collider2D>().ToList();
        foreach (Collider2D c in colliders)
        {
            c.isTrigger = true;
        }
    }

    private void DisableTankUI()
    {
        List<Canvas> canvases = TmpVehicleParent.GetComponentsInChildren<Canvas>().ToList();
        foreach (Canvas c in canvases)
        {
            c.gameObject.SetActive(false);
        }
    }
    private void SetSpriteValues()
    {
        List<SpriteRenderer> srs = TmpVehicleParent.GetComponentsInChildren<SpriteRenderer>().ToList();
        foreach (SpriteRenderer rend in srs)
        {
            rend.color = Color.white;
            rend.sortingLayerName = "Wizards";
            rend.sortingOrder += 20;
        }
    }
}
