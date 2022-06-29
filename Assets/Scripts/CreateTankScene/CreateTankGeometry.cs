using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TankRoomConstellation;

public class CreateTankGeometry : MonoBehaviour
{
    [HideInInspector]
    public TankRoomConstellation _trc;
    public RoomPosition[,] _roomPosMatrix;
    public GameObject TankGeometryParent { get; private set; }
    public GameObject RoomsParent { get; private set; }
    public List<Room> AllRooms { get; private set; }
    public Tilemap FloorTilemap { get; private set; }
    public GameObject RoofParent { get; private set; }
    public Tilemap RoofTilemap { get; private set; }
    [SerializeField]
    private List<SpriteRenderer> systemIcons = new List<SpriteRenderer>();
    private GameObject _visibleGrid;

    //public Color FloorColor;
    //public Color RoofColor;
    public void SpawnTankForCreator()
    {
        LoadRooms();
        CreateFloorAndRoof();
        PositionTankObjects();
        CreateWalls();
        CreateTires();
        CreateSystems();

        //InitWeaponsAndSystems();

        CreateTankSceneManager.instance._tools._tempFloorGrid.transform.position = FloorTilemap.transform.position;
        CreateTankSceneManager.instance._tools._tempRoofGrid.transform.position = RoofTilemap.transform.position;
    }
    private void Update()
    {
        int modifier = 1;
        if (Input.GetKey(KeyCode.LeftShift)) modifier = -1;
        if (Input.GetKeyDown(KeyCode.A))
        {
            ModifyTankSize(1 * modifier, 0, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ModifyTankSize(0, 1 * modifier, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ModifyTankSize(0, 0, 1 * modifier, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ModifyTankSize(0, 0, 0, 1 * modifier);
        }
    }
    private void CreateFloorAndRoof()
    {
        CreateFloorAndRoofTilemap();
        for (int x = 0; x < _trc._savedXSize; x++)
        {
            for (int y = 0; y < _trc._savedYSize; y++)
            {
                if (x >= _trc._savedMatrix.XArray.Length || y >= _trc._savedMatrix.XArray[0].YStuff.Length) continue;
                if (_trc._savedMatrix.XArray[x].YStuff[y].RoomPrefab)
                {
                    int sizeX = _trc._savedMatrix.XArray[x].YStuff[y].RoomPrefab.GetComponent<Room>().sizeX;
                    int sizeY = _trc._savedMatrix.XArray[x].YStuff[y].RoomPrefab.GetComponent<Room>().sizeY;
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

        //  Create Grid
        Grid floorGrid = floor.AddComponent<Grid>();
        floorGrid.cellSize = new Vector3(0.5f, 0.5f, 0);

        //  Create Tilemap
        FloorTilemap = floor.AddComponent<Tilemap>();
        FloorTilemap.tileAnchor = new Vector3(0, 1, 0);

        //  Create Renderer
        TilemapRenderer floorRend = floor.AddComponent<TilemapRenderer>();
        floorRend.sortingLayerName = "Vehicles";

        //  Roof

        RoofParent = new GameObject("RoofParent");
        RoofParent.transform.parent = transform;
        RoofParent.transform.localPosition = Vector3.zero;

        GameObject roofTilemap = new GameObject("RoofTilemap");
        roofTilemap.transform.parent = RoofParent.transform;
        roofTilemap.transform.localPosition = Vector3.zero;

        //  Create Grid
        Grid roofGrid = roofTilemap.AddComponent<Grid>();
        roofGrid.cellSize = new Vector3(0.5f, 0.5f, 0);

        //  Create Tilemap
        RoofTilemap = roofTilemap.AddComponent<Tilemap>();
        RoofTilemap.tileAnchor = new Vector3(0, 1, 0);

        //  Create Renderer
        TilemapRenderer roofRend = roofTilemap.AddComponent<TilemapRenderer>();
        roofRend.sortingLayerName = "VehicleRoof";
    }
    private void CreateWallsTilemap()
    {
        GameObject walls = new GameObject("WallsTilemap");
        walls.transform.parent = gameObject.transform;
        walls.transform.localPosition = Vector3.zero;

        //Create the Grid
        Grid g = walls.AddComponent<Grid>();
        g.cellSize = new Vector3(0.5f, 0.5f, 0);

        //  Create Tilemap
        FloorTilemap = walls.AddComponent<Tilemap>();
        FloorTilemap.tileAnchor = new Vector3(0, 1, 0);

        //  Create Renderer
        TilemapRenderer r = walls.AddComponent<TilemapRenderer>();
        r.sortingLayerName = "Vehicles";

        //  Create Collider
        TilemapCollider2D c = walls.AddComponent<TilemapCollider2D>();
    }
    public void LoadFloorAtPos(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                Tile t = _trc._tmpMatrix.XArray[x].YStuff[y].FloorTilePrefab;
                t.color = _trc._tmpMatrix.XArray[x].YStuff[y].FloorColor;
                FloorTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
                t.color = Color.white;
            }
        }
    }
    public void ChangeFloorAtPos(int startX, int startY, int sizeX, int sizeY, Tile t, Color c)
    {
        if (t != null)
        {
            //  Check if we overstepped the edges of our matrix and need to expand first!
            if (startX < 0 || startY < 0 || startX > _trc._tmpXSize - 1 || startY > _trc._tmpYSize - 1)
            {
                /*
                int expandL = Math.Abs(Math.Min(0, startX));
                int expandR = Math.Max(_tRC._tmpXSize - 1, startX) - _tRC._tmpXSize + 1;
                int expandU = Math.Abs(Math.Min(0, startY));
                int expandD = Math.Max(_tRC._tmpYSize - 1, startY) - _tRC._tmpYSize + 1;
                ModifyTankSize(expandL, expandR, expandU, expandD);
                */
                //print("left: " + expandL + ", right: " + expandR +  ", up :" + expandU + ", down: " + expandD);
            }
            else if(!_roomPosMatrix[startX, startY])
            {
                GameObject roomToLoad = (GameObject)Resources.Load("Rooms/1x1Room");
                if (sizeX == 1)
                {
                    if (sizeY == 2)
                    {
                        roomToLoad = (GameObject)Resources.Load("Rooms/1x2Room");
                        //check if we overlap with a room and we have to delete
                    }
                }
                if (sizeX == 2)
                {
                    if (sizeY == 1)
                    {
                        roomToLoad = (GameObject)Resources.Load("Rooms/2x1Room");

                        //check if we overlap with a room and we have to delete
                    }
                    if (sizeY == 2)
                    {
                        roomToLoad = (GameObject)Resources.Load("Rooms/2x2Room");

                        //check if we overlap with a room and we have to delete
                    }
                }
                CreateNewEmptyRoomAtPos(startX, startY, roomToLoad);
            }
            for (int x = startX; x < startX + sizeX; x++)
            {
                for (int y = startY; y < startY + sizeY; y++)
                {
                    t.color = c;
                    FloorTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
                    _trc._tmpMatrix.XArray[x].YStuff[y].FloorTilePrefab = t;
                    _trc._tmpMatrix.XArray[x].YStuff[y].FloorColor = c;
                    t.color = Color.white; //   reset the tile objects color
                }
            }
        }
        //  If we were erasing, check here if we can make our matrix smaller again!
        else
        {
            DeleteRoomAtPos(startX, startY);
        }
    }
    public void DeleteRoomAtPos(int roomPositionX, int roomPositionY)
    {
        FloorTilemap.SetTile(new Vector3Int(roomPositionX, -(roomPositionY + 1), 0), null);
        RoofTilemap.SetTile(new Vector3Int(roomPositionX, -(roomPositionY + 1), 0), null);
        if (_roomPosMatrix[roomPositionX, roomPositionY])
        {
            if(_roomPosMatrix[roomPositionX, roomPositionY].ParentRoom)
            {
                _trc._tmpMatrix.XArray[roomPositionX].YStuff[roomPositionY].RoomPrefab = null;

                Room rParent = _roomPosMatrix[roomPositionX, roomPositionY].ParentRoom;
                Vector2Int roomsize = GetRoomSize(roomPositionX, roomPositionY);
                int firstRoomPosX = rParent.allRoomPositions[0]._xPos;
                int firstRoomPosY = rParent.allRoomPositions[0]._yPos;
                for (int x = firstRoomPosX; x < firstRoomPosX + roomsize.x; x++)
                {
                    for (int y = firstRoomPosY; y < firstRoomPosY + roomsize.y; y++)
                    {
                        FloorTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), null);
                        RoofTilemap.SetTile(new Vector3Int(roomPositionX, -(roomPositionY + 1), 0), null);

                        CreateTireAtPos(x, y, null);
                        CreateSystemAtPos(x, y, null);
                        CreateWallAtPos(x, y, "delete");
                    }
                }
                Destroy(_roomPosMatrix[roomPositionX, roomPositionY].ParentRoom.gameObject);
            }
        }
    }

    //  Roof

    public void LoadRoofAtPos(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                if(!_trc._tmpMatrix.XArray[x].YStuff[y].RoofTilePrefab) return;
                Tile t = _trc._tmpMatrix.XArray[x].YStuff[y].RoofTilePrefab;
                t.color = _trc._tmpMatrix.XArray[x].YStuff[y].RoofColor;
                RoofTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
                t.color = Color.white;
            }
        }
    }
    public void ChangeRoofAtPos(int startX, int startY, int sizeX, int sizeY, Tile t, Color c)
    {
        print(t);
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                RoofTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
                _trc._tmpMatrix.XArray[x].YStuff[y].RoofTilePrefab = t;
                t.color = c;
                _trc._tmpMatrix.XArray[x].YStuff[y].RoofColor = c;
                t.color = Color.white;
            }
        }
    }

    //  Rooms

    private void LoadRooms()
    {
        _roomPosMatrix = new RoomPosition[_trc._savedXSize, _trc._savedYSize];
        AllRooms = new List<Room>();
        if (FloorTilemap) FloorTilemap.ClearAllTiles();
        if (RoofTilemap) RoofTilemap.ClearAllTiles();
        if (_visibleGrid) Destroy(_visibleGrid);

        if (RoomsParent) Destroy(RoomsParent);
        RoomsParent = new GameObject("All Tank Rooms");
        RoomsParent.transform.parent = transform;
        RoomsParent.transform.localPosition = Vector3.zero;

        for (int y = 0; y < _trc._savedYSize; y++)
        {
            for (int x = 0; x < _trc._savedXSize; x++)
            {
                if (x >= _trc._savedMatrix.XArray.Length || y >= _trc._savedMatrix.XArray[0].YStuff.Length) continue;
                if (_trc._savedMatrix.XArray[x].YStuff[y].RoomPrefab)
                {
                    LoadRoomAtPos(x,y);
                }
            }
        }
    }
    public void LoadRoomAtPos(int x, int y)
    {
        GameObject rGO = Instantiate(_trc._savedMatrix.XArray[x].YStuff[y].RoomPrefab);
        Room r = rGO.GetComponent<Room>();
        r.tr = _trc;
        rGO.transform.parent = RoomsParent.transform;
        rGO.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
        AllRooms.Add(r);

        // Set the Room Positions
        int roomPosNr = 0;
        for (int roomY = 0; roomY < r.sizeY; roomY++)
        {
            for (int roomX = 0; roomX < r.sizeX; roomX++)
            {
                _roomPosMatrix[x + roomX, y + roomY] = r.allRoomPositions[roomPosNr];
                _roomPosMatrix[x + roomX, y + roomY]._xPos = x + _roomPosMatrix[x + roomX, y + roomY]._xRel;
                _roomPosMatrix[x + roomX, y + roomY]._yPos = y + _roomPosMatrix[x + roomX, y + roomY]._yRel;

                _roomPosMatrix[x + roomX, y + roomY].name = "X" + _roomPosMatrix[x + roomX, y + roomY]._xPos.ToString()
                                                      + " , Y" + _roomPosMatrix[x + roomX, y + roomY]._yPos.ToString() + ", ";
                roomPosNr++;
            }
        }

        //sets the corner of the room that doesnt get caught with the matrix
        _roomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1] = r.allRoomPositions[r.sizeX * r.sizeY - 1];
        _roomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1]._xPos = x + r.sizeX - 1;
        _roomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1]._yPos = y + r.sizeY - 1;

        _roomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1].name = "X" + (x + r.sizeX - 1).ToString() + " , Y" + (y + r.sizeY - 1).ToString();
    }
    public void CreateNewEmptyRoomAtPos(int x, int y, GameObject roomToCreate)
    {
        _trc._tmpMatrix.XArray[x].YStuff[y].RoomPrefab = roomToCreate;

        GameObject rGO = Instantiate(roomToCreate);
        Room r = rGO.GetComponent<Room>();
        r.tr = _trc;
        rGO.transform.parent = RoomsParent.transform;
        rGO.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
        AllRooms.Add(r);

        // Set the Room Positions
        int roomPosNr = 0;
        for (int roomY = 0; roomY < r.sizeY; roomY++)
        {
            for (int roomX = 0; roomX < r.sizeX; roomX++)
            {
                _roomPosMatrix[x + roomX, y + roomY] = r.allRoomPositions[roomPosNr];
                _roomPosMatrix[x + roomX, y + roomY]._xPos = x + _roomPosMatrix[x + roomX, y + roomY]._xRel;
                _roomPosMatrix[x + roomX, y + roomY]._yPos = y + _roomPosMatrix[x + roomX, y + roomY]._yRel;

                _roomPosMatrix[x + roomX, y + roomY].name = "X" + _roomPosMatrix[x + roomX, y + roomY]._xPos.ToString()
                                                      + " , Y" + _roomPosMatrix[x + roomX, y + roomY]._yPos.ToString() + ", ";
                roomPosNr++;
            }
        }

        //sets the corner of the room that doesnt get caught with the matrix
        _roomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1] = r.allRoomPositions[r.sizeX * r.sizeY - 1];
        _roomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1]._xPos = x + r.sizeX - 1;
        _roomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1]._yPos = y + r.sizeY - 1;

        _roomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1].name = "X" + (x + r.sizeX - 1).ToString() + " , Y" + (y + r.sizeY - 1).ToString();
    }

    //  Walls

    public void CreateWalls()
    {
        for (int y = 0; y < _trc._savedYSize; y++)
        {
            for (int x = 0; x < _trc._savedXSize; x++)
            {
                if (x >= _trc._savedMatrix.XArray.Length || y >= _trc._savedMatrix.XArray[0].YStuff.Length) continue;

                if (_trc._savedMatrix.XArray[x].YStuff[y]._topWallExists)
                {
                    GameObject wall = (GameObject)Instantiate(Resources.Load("Rooms/Walls/WallUp"));
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedTopWall = wall;
                }
                if (_trc._savedMatrix.XArray[x].YStuff[y]._rightWallExists)
                {
                    GameObject wall = (GameObject)Instantiate(Resources.Load("Rooms/Walls/WallRight"));
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedRightWall = wall;
                }
                if (_trc._savedMatrix.XArray[x].YStuff[y]._bottomWallExists)
                {
                    GameObject wall = (GameObject)Instantiate(Resources.Load("Rooms/Walls/WallDown"));
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedBottomWall = wall;
                }
                if (_trc._savedMatrix.XArray[x].YStuff[y]._leftWallExists)
                {
                    GameObject wall = (GameObject)Instantiate(Resources.Load("Rooms/Walls/WallLeft"));
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedLeftWall = wall;
                }
            }
        }
    }
    public void CreateWallAtPos(int posX, int posY, string direction)
    {
        if (direction == "delete")
        {
            _trc._tmpMatrix.XArray[posX].YStuff[posY]._topWallExists = false;
            _trc._tmpMatrix.XArray[posX].YStuff[posY]._rightWallExists = false;
            _trc._tmpMatrix.XArray[posX].YStuff[posY]._bottomWallExists = false;
            _trc._tmpMatrix.XArray[posX].YStuff[posY]._leftWallExists = false;

            if (_roomPosMatrix[posX, posY]._spawnedTopWall) Destroy(_roomPosMatrix[posX, posY]._spawnedTopWall);
            if (_roomPosMatrix[posX, posY]._spawnedRightWall) Destroy(_roomPosMatrix[posX, posY]._spawnedRightWall);
            if (_roomPosMatrix[posX, posY]._spawnedBottomWall) Destroy(_roomPosMatrix[posX, posY]._spawnedBottomWall);
            if (_roomPosMatrix[posX, posY]._spawnedLeftWall) Destroy(_roomPosMatrix[posX, posY]._spawnedLeftWall);

            _roomPosMatrix[posX, posY]._spawnedTopWall = null;
            _roomPosMatrix[posX, posY]._spawnedRightWall = null;
            _roomPosMatrix[posX, posY]._spawnedBottomWall = null;
            _roomPosMatrix[posX, posY]._spawnedLeftWall = null;
            return;
        }

        GameObject wall = null;
        if (direction == "up")
        {
            if (_roomPosMatrix[posX, posY]._spawnedTopWall != null) return;
            _trc._tmpMatrix.XArray[posX].YStuff[posY]._topWallExists = true;
            wall = (GameObject)Instantiate(Resources.Load("Rooms/Walls/WallUp"));
            _roomPosMatrix[posX, posY]._spawnedTopWall = wall;
        }
        else if (direction == "left")
        {
            if (_roomPosMatrix[posX, posY]._spawnedLeftWall != null) return;
            _trc._tmpMatrix.XArray[posX].YStuff[posY]._leftWallExists = true;
            wall = (GameObject)Instantiate(Resources.Load("Rooms/Walls/WallLeft"));
            _roomPosMatrix[posX, posY]._spawnedLeftWall = wall;
        }
        else if (direction == "right")
        {
            if (_roomPosMatrix[posX, posY]._spawnedRightWall != null) return;
            _trc._tmpMatrix.XArray[posX].YStuff[posY]._rightWallExists = true;
            wall = (GameObject)Instantiate(Resources.Load("Rooms/Walls/WallRight"));
            _roomPosMatrix[posX, posY]._spawnedRightWall = wall;
        }
        else if (direction == "down")
        {

            if (_roomPosMatrix[posX, posY]._spawnedBottomWall != null) return;
            _trc._tmpMatrix.XArray[posX].YStuff[posY]._bottomWallExists = true;
            wall = (GameObject)Instantiate(Resources.Load("Rooms/Walls/WallDown"));
            _roomPosMatrix[posX, posY]._spawnedBottomWall = wall;
        }
        wall.transform.SetParent(_roomPosMatrix[posX, posY].transform);
        wall.transform.localPosition = Vector3.zero;
    }

    //  Tires

    public void CreateTires()
    {
        GameObject rotatableObjects = new GameObject("RotatableObjects");
        rotatableObjects.transform.parent = transform;
        rotatableObjects.transform.localPosition = Vector3.zero;

        for (int x = 0; x < _trc._savedXSize; x++)
        {
            for (int y = 0; y < _trc._savedYSize; y++)
            {
                if (_trc._savedMatrix.XArray[x].YStuff[y].TirePrefab)
                {
                    GameObject tire = _trc._savedMatrix.XArray[x].YStuff[y].TirePrefab;

                    if (_roomPosMatrix[x, y]._spawnedTire != null) return;
                    //if (!_roomPosMatrix[x, y]) continue;
                    GameObject tireObj = Instantiate(tire);
                    tireObj.transform.parent = _roomPosMatrix[x, y].transform;
                    tireObj.transform.localPosition = Vector3.zero;
                    tireObj.transform.parent = rotatableObjects.transform;
                    _roomPosMatrix[x, y]._spawnedTire = tireObj;
                }
            }
        }

    }

    public void CreateTireAtPos(int posX, int posY, GameObject tirePrefab)
    {
        if (tirePrefab == null) //delete tire
        {
            if (_roomPosMatrix[posX, posY]._spawnedTire)
            {
                Destroy(_roomPosMatrix[posX, posY]._spawnedTire);
                _roomPosMatrix[posX, posY]._spawnedTire = null;
            }
            _trc._tmpMatrix.XArray[posX].YStuff[posY].TirePrefab = null;
            return;
        }

        if (!_trc._tmpMatrix.XArray[posX].YStuff[posY].TirePrefab) // spawn
        {
            _trc._tmpMatrix.XArray[posX].YStuff[posY].TirePrefab = tirePrefab;
            GameObject tire = Instantiate(tirePrefab);
            tire.transform.SetParent(_roomPosMatrix[posX, posY].transform);
            tire.transform.localPosition = Vector3.zero;
        }
    }

    //  Systems

    public void CreateSystems()
    {
        for (int x = 0; x < _trc._savedXSize; x++)
        {
            for (int y = 0; y < _trc._savedYSize; y++)
            {
                if (_trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab)
                {
                    GameObject system = _trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab;

                    if (_roomPosMatrix[x, y]._spawnedSystem != null) return;
                    //if (!_roomPosMatrix[x, y]) continue;
                    GameObject systemObj = Instantiate(system);
                    systemObj.transform.parent = _roomPosMatrix[x, y].transform;
                    systemObj.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedSystem = systemObj;
                }
            }
        }

    }

    public void CreateSystemAtPos(int posX, int posY, GameObject sysPrefab)
    { 
        //delete
        if (sysPrefab == null)
        {
            if (_roomPosMatrix[posX, posY]._spawnedSystem)
            {
                Destroy(_roomPosMatrix[posX, posY]._spawnedSystem);
                _roomPosMatrix[posX, posY]._spawnedSystem = null;
            }
            _trc._tmpMatrix.XArray[posX].YStuff[posY].SystemPrefab = null;
            return;
        }
        // spawn
        if (!_trc._tmpMatrix.XArray[posX].YStuff[posY].SystemPrefab)
        {
            _trc._tmpMatrix.XArray[posX].YStuff[posY].SystemPrefab = sysPrefab;
            GameObject system = Instantiate(sysPrefab);
            system.transform.SetParent(_roomPosMatrix[posX, posY].transform);
            system.transform.localPosition = Vector3.zero;
            _roomPosMatrix[posX, posY]._spawnedSystem = system.gameObject;
        }
    }

    
    
    
    
    
    //  misc for now
    public void CreateSystemIcons()
    {
        for (int x = 0; x < _trc._savedXSize; x++)
        {
            for (int y = 0; y < _trc._savedYSize; y++)
            {
                if (_trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab)
                {
                    ASystem sys = _trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab.GetComponent<ASystem>();
                    if (_trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab.TryGetComponent(out AWeapon wep))
                    {
                        _roomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = Resources.Load("Art/WeaponSystemIcon", typeof(Sprite)) as Sprite;
                    }
                    else
                    {
                        _roomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = sys.SystemSprite;
                        systemIcons.Add(_roomPosMatrix[x, y].ParentRoom.roomSystemRenderer);
                    }
                }
            }
        }
    }

    public void InitWeaponsAndSystems()
    {
        TankWeaponsAndSystems twep = GetComponent<TankWeaponsAndSystems>();
        for (int x = 0; x < _trc._savedXSize; x++)
        {
            for (int y = 0; y < _trc._savedYSize; y++)
            {
                if (_trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab)
                {
                    GameObject prefab = _trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab;
                    //Our object should either be a Weapon or a System, so check for both cases


                    //  Weapons
                    if (_trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab.GetComponent<AWeapon>() != null)
                    {
                        //if (!RoomPosMatrix[x, y]) continue;
                        GameObject weaponObj = Instantiate(prefab);
                        weaponObj.transform.parent = _roomPosMatrix[x, y].ParentRoom.transform;
                        weaponObj.transform.localPosition = Vector3.zero;
                        AWeapon wep = weaponObj.GetComponent<AWeapon>();
                        PositionSystemInRoom(weaponObj.GetComponent<ASystem>(), weaponObj.transform.parent.GetComponent<Room>());
                        wep.RoomPosForInteraction = _roomPosMatrix[x, y].ParentRoom.allRoomPositions[0];
                        twep.AWeaponArray.Add(wep);

                        //Set the reference to the rooms
                        _roomPosMatrix[x, y].ParentRoom.roomSystem = wep;
                    }

                    //  Systems
                    else if (_trc._savedMatrix.XArray[x].YStuff[y].SystemPrefab.GetComponent<ASystem>() != null)
                    {
                        GameObject systemObj = Instantiate(prefab);
                        systemObj.transform.parent = _roomPosMatrix[x, y].ParentRoom.transform;

                        systemObj.transform.localPosition = Vector3.zero;
                        ASystem sys = systemObj.GetComponent<ASystem>();
                        PositionSystemInRoom(systemObj.GetComponent<ASystem>(), systemObj.transform.parent.GetComponent<Room>());
                        twep.ASystemArray.Add(sys);
                        sys.RoomPosForInteraction = _roomPosMatrix[x, y].ParentRoom.allRoomPositions[0];

                        //Set the reference to the rooms
                        _roomPosMatrix[x, y].ParentRoom.roomSystem = sys;
                    }
                }
            }
        }
    }
    private void PositionSystemInRoom(ASystem system, Room parentRoom)
    {
        system.SystemObj.transform.localPosition = Vector2.zero;
        if (parentRoom.sizeX > 1) system.SystemObj.transform.localPosition += new Vector3(0.25f, 0);
        if (parentRoom.sizeY > 1) system.SystemObj.transform.localPosition += new Vector3(0, -0.25f);
    }
    private void PositionTankObjects()
    {
        //create overarching object for all our spawned objects
        TankGeometryParent = new GameObject("Tank Geometry Parent");
        TankGeometryParent.transform.parent = gameObject.transform;
        TankGeometryParent.transform.localPosition = Vector3.zero;

        // parent all spawnedObjects to this parent
        RoomsParent.transform.parent = RoofParent.transform.parent = FloorTilemap.transform.parent = TankGeometryParent.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        TankGeometryParent.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * _trc._savedXSize, 0.25f * _trc._savedYSize, 0);

        _visibleGrid = new GameObject("VisibleGrid");
        SpriteRenderer sr = _visibleGrid.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        sr.sprite = Resources.Load("Art/white_square_border", typeof(Sprite)) as Sprite;

        ResizeGrid();
    }
    public void SetSystemIconLayer(bool top)
    {
        if (systemIcons.Count == 0) return;
        foreach (SpriteRenderer sr in systemIcons)
        {
            if (top)
            {
                sr.sortingLayerName = "VehicleUI";
                sr.sortingOrder = 1;
            }
            else
            {
                sr.sortingLayerName = "Vehicles";
                sr.sortingOrder = 2;
            }
        }
    }
    public Room FindRandomRoomWithSpace()
    {
        List<Room> allRoomsTMP = AllRooms;
        // searches through all possible rooms until it finds one it can occupy
        for (int i = 0; i < AllRooms.Count; i++)
        {
            Room tmpRoom = allRoomsTMP[UnityEngine.Random.Range(0, allRoomsTMP.Count - 1)];
            for (int j = 0; j < tmpRoom.freeRoomPositions.Length; j++)
            {
                if (tmpRoom.freeRoomPositions[j] != null)
                {
                    //print("found a random free room");
                    return tmpRoom;
                }
            }
        }
        //if it has has found no free rooms, return
        print("no free rooms found");
        return null;
    }
    public void VisualizeMatrix()
    {
        string matrix = "";
        for (int y = 0; y < _trc._savedYSize; y++)
        {
            matrix += "Y:" + y.ToString() + ": ";
            for (int x = 0; x < _trc._savedXSize; x++)
            {
                if (_roomPosMatrix[x, y]) matrix += "(" + _roomPosMatrix[x, y].name + ") ";
                else matrix += "__NONE__, ";
            }
            matrix += "\n";
        }
        print(matrix);
    }
    public Vector2Int TilemapToCellPos(Vector3Int tmPos)
    {
        return new Vector2Int(tmPos.x, -(tmPos.y + 1));
    }
    public void ModifyTankSize(int left, int right, int up, int down)
    {
        int xOldTankSize = _trc._tmpXSize;
        int yOldTankSize = _trc._tmpYSize;

        int xExpandedTankSize = xOldTankSize + left + right;
        int yExpandedTankSize = yOldTankSize + up + down;

        //  Shrink in case of negative parameters
        if (yExpandedTankSize < 0 || xExpandedTankSize < 0) return;
        if (left < 0 || right < 0 || up < 0 || down < 0)
        {
            if (left < 0)   for (int y = 0; y < yOldTankSize; y++) for (int x = 0; x < Mathf.Abs(left); x++)                    DeleteRoomAtPos(x, y);
            if (right < 0)  for (int y = 0; y < yOldTankSize; y++) for (int x = xOldTankSize-1; x >= xOldTankSize + right; x--) DeleteRoomAtPos(x, y);
            if (up < 0)     for (int x = 0; x < xOldTankSize; x++) for (int y = 0; y < Mathf.Abs(up); y++)                      DeleteRoomAtPos(x, y);
            if (down < 0)   for (int x = 0; x < xOldTankSize; x++) for (int y = yOldTankSize-1; y >= yOldTankSize + down; y--)  DeleteRoomAtPos(x, y);
        }

        //  Create Temp Matrix of new size and add each room into the correct position

        XValues expandedMatrix = new XValues(xExpandedTankSize, yExpandedTankSize);
        RoomPosition[,] expandedPosMatrix = new RoomPosition[xExpandedTankSize, yExpandedTankSize];

        for (int y = 0; y < yExpandedTankSize; y++)
        {
            for (int x = 0; x < xExpandedTankSize; x++)
            {
                int xCopyPos = x - left;
                int yCopyPos = y - up;

                if (xCopyPos < 0 || xCopyPos >= _trc._tmpMatrix.XArray.Length
                    || yCopyPos < 0 || yCopyPos >= _trc._tmpMatrix.XArray[0].YStuff.Length) continue;

                //  Fix References for the matrix
                expandedMatrix.XArray[x].YStuff[y] = _trc._tmpMatrix.XArray[xCopyPos].YStuff[yCopyPos];

                //  Set indices of individual room positions and move them to the appropriate position
                print(_roomPosMatrix[xCopyPos, yCopyPos]);
                if (!_roomPosMatrix[xCopyPos, yCopyPos]) continue;
                expandedPosMatrix[x, y] = _roomPosMatrix[xCopyPos, yCopyPos];
                expandedPosMatrix[x, y]._xPos = x;
                expandedPosMatrix[x, y]._yPos = y;
                if (expandedPosMatrix[x, y]._xRel == 0 && expandedPosMatrix[x, y]._yRel == 0)
                    expandedPosMatrix[x, y].ParentRoom.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
            }
        }

        //  Reposition Tilemaps

        ShiftTilemap(FloorTilemap, xExpandedTankSize, yExpandedTankSize, left, right, up, down);
        CreateTankSceneManager.instance._tools._tempFloorGrid.transform.position = FloorTilemap.transform.position;

        ShiftTilemap(RoofTilemap, xExpandedTankSize, yExpandedTankSize, left, right, up, down);
        CreateTankSceneManager.instance._tools._tempRoofGrid.transform.position = FloorTilemap.transform.position;


        //  Reposition geometry

        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * (left + right), 0.25f * (up + down), 0);

        //  Save Changes into the temp

        _trc._tmpMatrix = expandedMatrix;
        _roomPosMatrix = expandedPosMatrix;
        _trc._tmpXSize = xExpandedTankSize;
        _trc._tmpYSize = yExpandedTankSize;

        //Show Grid Size

        ResizeGrid();
    }
    private void ShiftTilemap(Tilemap actualTilemap, int sizeX, int sizeY, int left, int right, int up, int down)
    {
        GameObject tmpObj = new GameObject("tmp");
        Grid tmpGrid = tmpObj.AddComponent<Grid>();
        tmpGrid.cellSize = new Vector3(0.5f, 0.5f, 0);

        Tilemap tilemapStorage = tmpObj.AddComponent<Tilemap>();
        tilemapStorage.tileAnchor = new Vector3(0, 1, 0);

        //  Copy the entire old tilemap
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                TileBase t = actualTilemap.GetTile(new Vector3Int(x, -y, 0));
                //TODO set tile color!
                tilemapStorage.SetTile(new Vector3Int(x, -y, 0), t);
            }
        }
        //  Clear the old tilemap
        actualTilemap.ClearAllTiles();

        //  Copy the old tiles into the cleared tilemap

        //TODO, going up and down negatively chomps one too many!
        //  Losing information somehow?? range seems to be fine
        //  figure out damn coor´dinate system transfomration

        for (int y = 0; y < sizeY + up; y++)
        {
            for (int x = 0; x < sizeX + left ; x++)
            {
                actualTilemap.SetTile(new Vector3Int(x, -y, 0), tilemapStorage.GetTile(new Vector3Int(x - left, -(y - up), 0)));
            }
        }
        Destroy(tmpObj);
    }
    private void ResizeGrid()
    {
        _visibleGrid.transform.localScale = new Vector3(_trc._tmpXSize, _trc._tmpYSize, 0);
        CreateTankSceneManager.instance._tUI.UpdateSize(_trc._tmpXSize, _trc._tmpYSize);
    }
    private Vector2Int GetRoomSize(int x, int y)
    {
        return new Vector2Int(_roomPosMatrix[x, y].ParentRoom.sizeX, _roomPosMatrix[x, y].ParentRoom.sizeY);
    }
}