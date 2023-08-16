using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TankRoomConstellation;

public class MainMenuSceneTankPreview : MonoBehaviour
{
    public List<TankRoomConstellation> _playerTankConstellations;
    public List<GameObject> _spawnedTanks = new List<GameObject>();
    public int tankIndex;

    //  Tank Spawning; all of these are temporary values that get overwritten with each tank

    [HideInInspector] public TankRoomConstellation _trc;
    public RoomPosition[,] _roomPosMatrix;
    public GameObject Tmp { get; private set; }
    public GameObject TmpRoomsParent { get; private set; }
    public List<Room> TmpAllRooms { get; private set; }
    public Tilemap TmpFloorTilemap { get; private set; }
    public GameObject TmpRoofParent { get; private set; }
    public Tilemap TmpRoofTilemap { get; private set; }

    // Extra Display stuff

    public GameObject _vehicleBGOrb;
    public Color _translucentColor;

    private void Awake()
    {
        REF.TankPreview = this;
    }
    private void Start()
    {
        SpawnAllTanks();
        HideAllTanks();
        tankIndex = 0;
        REF.mUI.UpdateSelectedTankText(_playerTankConstellations[tankIndex].name);
    }

    private void SpawnAllTanks()
    {
        foreach (TankRoomConstellation trc in _playerTankConstellations)
        {
            _trc = trc;
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
        tankIndex++;
        if (tankIndex >= _playerTankConstellations.Count)
        {
            tankIndex = 0;
        }
        HideAllTanks();
        _spawnedTanks[tankIndex].SetActive(true);
        REF.mUI.UpdateSelectedTankText(_playerTankConstellations[tankIndex].name);
    }
    public void PreviousTank()
    {
        tankIndex--;
        if (tankIndex < 0)
        {
            tankIndex = _playerTankConstellations.Count - 1;
        }
        HideAllTanks();
        _spawnedTanks[tankIndex].SetActive(true);
        REF.mUI.UpdateSelectedTankText(_playerTankConstellations[tankIndex].name);
    }

    //  Spawning

    public void SpawnTankForCreator()
    {
        LoadRooms();
        CreateFloorAndRoof();
        PositionTankObjects();
        CreateWalls();
        CreateTires();
        CreateSystems();
        MakeCollidersTriggers();
        DisableTankUI();
        SetSpriteValues();
    }
    private void CreateFloorAndRoof()
    {
        CreateFloorAndRoofTilemap();

        TmpRoofTilemap.color = new Color(_trc.RoofColorR, _trc.RoofColorG, _trc.RoofColorB, 1);
        TmpFloorTilemap.color = new Color(_trc.FloorColorR, _trc.FloorColorG, _trc.FloorColorB, 1);

        for (int x = 0; x < _trc._savedXSize; x++)
        {
            for (int y = 0; y < _trc._savedYSize; y++)
            {
                if (x >= _trc._savedMatrix.XArray.Length || y >= _trc._savedMatrix.XArray[0].YStuff.Length) continue;

                if (_trc._savedMatrix.XArray[x].YStuff[y].RoomPrefabPath != "")
                {
                    Room room = Resources.Load(_trc._savedMatrix.XArray[x].YStuff[y].RoomPrefabPath, typeof(Room)) as Room;
                    int sizeX = room.sizeX;
                    int sizeY = room.sizeY;
                    LoadFloorAtPos(x, y, sizeX, sizeY);
                    LoadRoofAtPos(x, y, sizeX, sizeY);
                }
            }
        }
    }
    private void CreateFloorAndRoofTilemap()
    {
        //  Floor

        GameObject floor = new GameObject("FloorTilemap");
        floor.transform.parent = transform;
        floor.transform.localPosition = Vector3.zero;
        floor.transform.localScale = Vector3.one;

        //  Create Grid
        Grid floorGrid = floor.AddComponent<Grid>();
        floorGrid.cellSize = new Vector3(0.5f, 0.5f, 0);

        //  Create Tilemap
        TmpFloorTilemap = floor.AddComponent<Tilemap>();
        TmpFloorTilemap.tileAnchor = new Vector3(0, 1, 0);

        //  Create Renderer
        TilemapRenderer floorRend = floor.AddComponent<TilemapRenderer>();
        floorRend.sortingLayerName = "Vehicles";

        //  Roof

        TmpRoofParent = new GameObject("RoofParent");
        TmpRoofParent.transform.parent = transform;
        TmpRoofParent.transform.localPosition = Vector3.zero;
        TmpRoofParent.transform.localScale = Vector3.one;

        GameObject roofTilemap = new GameObject("RoofTilemap");
        roofTilemap.transform.parent = TmpRoofParent.transform;
        roofTilemap.transform.localPosition = Vector3.zero;
        roofTilemap.transform.localScale = Vector3.one;

        //  Create Grid
        Grid roofGrid = roofTilemap.AddComponent<Grid>();
        roofGrid.cellSize = new Vector3(0.5f, 0.5f, 0);

        //  Create Tilemap
        TmpRoofTilemap = roofTilemap.AddComponent<Tilemap>();
        TmpRoofTilemap.tileAnchor = new Vector3(0, 1, 0);

        //  Create Renderer
        TilemapRenderer roofRend = roofTilemap.AddComponent<TilemapRenderer>();
        roofRend.sortingLayerName = "VehicleRoof";
    }
    public void LoadFloorAtPos(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                if (_trc._tmpMatrix.XArray[x].YStuff[y].FloorTilePrefabPath == "") continue;
                Tile t = Instantiate(Resources.Load(_trc._tmpMatrix.XArray[x].YStuff[y].FloorTilePrefabPath, typeof(Tile))) as Tile;
                TmpFloorTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
            }
        }
    }
    public void LoadRoofAtPos(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                if (_trc._tmpMatrix.XArray[x].YStuff[y].RoofTilePrefabPath == "") continue;
                Tile t = Instantiate(Resources.Load(_trc._tmpMatrix.XArray[x].YStuff[y].RoofTilePrefabPath, typeof(Tile))) as Tile;
                TmpRoofTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
            }
        }
    }
    private void LoadRooms()
    {
        _roomPosMatrix = new RoomPosition[_trc._savedXSize, _trc._savedYSize];
        TmpAllRooms = new List<Room>();
        if (TmpFloorTilemap) TmpFloorTilemap.ClearAllTiles();
        if (TmpRoofTilemap) TmpRoofTilemap.ClearAllTiles();

        TmpRoomsParent = new GameObject("All Tank Rooms");
        TmpRoomsParent.transform.parent = transform;
        TmpRoomsParent.transform.localPosition = Vector3.zero;
        TmpRoomsParent.transform.localScale = Vector3.one;

        for (int y = 0; y < _trc._savedYSize; y++)
        {
            for (int x = 0; x < _trc._savedXSize; x++)
            {
                if (x >= _trc._savedMatrix.XArray.Length || y >= _trc._savedMatrix.XArray[0].YStuff.Length) continue;
                if (_trc._savedMatrix.XArray[x].YStuff[y].RoomPrefabPath != "")
                {
                    LoadRoomAtPos(x, y);
                }
            }
        }
    }
    public void LoadRoomAtPos(int x, int y)
    {
        Room room = Instantiate(Resources.Load(_trc._savedMatrix.XArray[x].YStuff[y].RoomPrefabPath, typeof(Room))) as Room;
        room.tr = _trc;
        room.gameObject.transform.parent = TmpRoomsParent.transform;
        room.gameObject.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
        room.gameObject.transform.localScale = Vector3.one;
        TmpAllRooms.Add(room);

        // Set the Room Positions
        int roomPosNr = 0;
        for (int roomY = 0; roomY < room.sizeY; roomY++)
        {
            for (int roomX = 0; roomX < room.sizeX; roomX++)
            {
                _roomPosMatrix[x + roomX, y + roomY] = room.allRoomPositions[roomPosNr];
                _roomPosMatrix[x + roomX, y + roomY]._xPos = x + _roomPosMatrix[x + roomX, y + roomY]._xRel;
                _roomPosMatrix[x + roomX, y + roomY]._yPos = y + _roomPosMatrix[x + roomX, y + roomY]._yRel;

                _roomPosMatrix[x + roomX, y + roomY].name = "X" + _roomPosMatrix[x + roomX, y + roomY]._xPos.ToString()
                                                      + " , Y" + _roomPosMatrix[x + roomX, y + roomY]._yPos.ToString() + ", ";
                roomPosNr++;
            }
        }

        foreach(RoomPosition rPos in room.allRoomPositions) rPos.GetComponent<SpriteRenderer>().enabled = false; // Disable for preview!

        //sets the corner of the room that doesnt get caught with the matrix
        _roomPosMatrix[x + room.sizeX - 1, y + room.sizeY - 1] = room.allRoomPositions[room.sizeX * room.sizeY - 1];
        _roomPosMatrix[x + room.sizeX - 1, y + room.sizeY - 1]._xPos = x + room.sizeX - 1;
        _roomPosMatrix[x + room.sizeX - 1, y + room.sizeY - 1]._yPos = y + room.sizeY - 1;

        _roomPosMatrix[x + room.sizeX - 1, y + room.sizeY - 1].name = "X" + (x + room.sizeX - 1).ToString() + " , Y" + (y + room.sizeY - 1).ToString();
    }
    public void CreateWalls()
    {
        for (int y = 0; y < _trc._savedYSize; y++)
        {
            for (int x = 0; x < _trc._savedXSize; x++)
            {
                if (x >= _trc._savedMatrix.XArray.Length || y >= _trc._savedMatrix.XArray[0].YStuff.Length) continue;
                if (_trc._savedMatrix.XArray[x].YStuff[y]._topWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof (GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (_trc._savedMatrix.XArray[x].YStuff[y]._rightWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (_trc._savedMatrix.XArray[x].YStuff[y]._bottomWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
                if (_trc._savedMatrix.XArray[x].YStuff[y]._leftWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallLeft"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one;
                }
            }
        }
    }
    public void CreateTires()
    {
        for (int x = 0; x < _trc._savedXSize; x++)
        {
            for (int y = 0; y < _trc._savedYSize; y++)
            {
                if (_trc._savedMatrix.XArray[x].YStuff[y].MovementPrefabPath != "")
                {
                    if (_roomPosMatrix[x, y] == null) continue;

                    GameObject tirePrefab = Instantiate(Resources.Load(_trc._savedMatrix.XArray[x].YStuff[y].MovementPrefabPath, typeof(GameObject))) as GameObject;
                    tirePrefab.transform.parent = _roomPosMatrix[x, y].transform;
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
        TmpRoomsParent.transform.parent = TmpRoofParent.transform.parent = TmpFloorTilemap.transform.parent = Tmp.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        Tmp.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        Tmp.transform.localPosition += new Vector3(-0.25f * _trc._savedXSize, 0.25f * _trc._savedYSize, 0);
    }
    public void CreateSystems()
    {
        for (int x = 0; x < _trc._savedXSize; x++)
        {
            for (int y = 0; y < _trc._savedYSize; y++)
            {
                if (_trc._savedMatrix.XArray[x].YStuff[y].SystemPrefabPath != "")
                {
                    GameObject system = Resources.Load(_trc._savedMatrix.XArray[x].YStuff[y].SystemPrefabPath, typeof(GameObject)) as GameObject;
                    if (_roomPosMatrix[x, y]._spawnedSystem != null) return;
                    //if (!_roomPosMatrix[x, y]) continue;
                    GameObject systemObj = Instantiate(system);
                    systemObj.transform.parent = _roomPosMatrix[x, y].transform;
                    systemObj.transform.localPosition = Vector3.zero;
                    systemObj.transform.localScale = Vector3.one;
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
            //rend.color = _translucentColor;
            rend.color = Color.white;
            rend.sortingLayerName = "Wizards";
            rend.sortingOrder += 20;
        }
        //FloorTilemap.color = _translucentColor;
        //RoofTilemap.color = _translucentColor;
    }
    private Vector2Int GetRoomSize(int x, int y)
    {
        return new Vector2Int(_roomPosMatrix[x, y].ParentRoom.sizeX, _roomPosMatrix[x, y].ParentRoom.sizeY);
    }
}
