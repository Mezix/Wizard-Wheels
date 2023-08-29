using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static PlayerData;
using static TankRoomConstellation;

public class CreateTankGeometry : MonoBehaviour
{
    public RoomPosition[,] _roomPosMatrix;
    public GameObject TankGeometryParent { get; private set; }
    public GameObject RoomsParent { get; private set; }
    public List<Room> AllRooms { get; private set; }
    [SerializeField]
    private List<SpriteRenderer> systemIcons = new List<SpriteRenderer>();
    private GameObject _visibleGrid;
    public void SpawnTankForCreator()
    {
        LoadRooms();
        PositionTankObjects();
        LoadWalls();
        LoadTires();
        LoadSystems();
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
    public void DeleteRoomAtPos(int roomPositionX, int roomPositionY)
    {
        if (_roomPosMatrix[roomPositionX, roomPositionY])
        {
            if (_roomPosMatrix[roomPositionX, roomPositionY].ParentRoom)
            {
                CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[roomPositionX].YStuff[roomPositionY].RoomPrefabPath = null;

                Room rParent = _roomPosMatrix[roomPositionX, roomPositionY].ParentRoom;
                Vector2Int roomsize = GetRoomSize(roomPositionX, roomPositionY);
                int firstRoomPosX = rParent.allRoomPositions[0]._xPos;
                int firstRoomPosY = rParent.allRoomPositions[0]._yPos;
                for (int x = firstRoomPosX; x < firstRoomPosX + roomsize.x; x++)
                {
                    for (int y = firstRoomPosY; y < firstRoomPosY + roomsize.y; y++)
                    {
                        CreateTireAtPos(x, y, null);
                        CreateSystemAtPos(x, y, null);
                        CreateWallAtPos(x, y, "delete");
                    }
                }
                Destroy(_roomPosMatrix[roomPositionX, roomPositionY].ParentRoom.gameObject);
            }
        }
    }
    //  Rooms

    private void LoadRooms()
    {
        _roomPosMatrix = new RoomPosition[CreateTankSceneManager.instance.tankToEdit._savedXSize, CreateTankSceneManager.instance.tankToEdit._savedYSize];
        AllRooms = new List<Room>();
        if (_visibleGrid) Destroy(_visibleGrid);

        if (RoomsParent) Destroy(RoomsParent);
        RoomsParent = new GameObject("All Tank Rooms");
        RoomsParent.transform.parent = transform;
        RoomsParent.transform.localPosition = Vector3.zero;

        for (int y = 0; y < CreateTankSceneManager.instance.tankToEdit._savedYSize; y++)
        {
            for (int x = 0; x < CreateTankSceneManager.instance.tankToEdit._savedXSize; x++)
            {
                if (x >= CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray.Length || y >= CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[0].YStuff.Length) continue;
                if (CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].RoomPrefabPath != "")
                {
                    LoadRoomAtPos(x, y);
                }
            }
        }
    }

    public void ChangeFloorAtPos(int x, int y, FloorType floorType)
    {
        CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].FloorType = floorType;
        _roomPosMatrix[x,y].ParentRoom._floorRenderer.sprite = Resources.Load(GS.RoomGraphics(floorType.ToString()) + "3", typeof(Sprite)) as Sprite;
    }
    public void ChangeRoofAtPos(int x, int y, RoofType roofType)
    {
        CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].RoofType = roofType;
        _roomPosMatrix[x,y].ParentRoom._roofRenderer.sprite = Resources.Load(GS.RoomGraphics(roofType.ToString()), typeof(Sprite)) as Sprite;
    }

    public void ShowRoof(bool b)
    {
        foreach (Room r in AllRooms)
        {
            r.ShowRoof(b);
        }
    }
    public void ShowFloor(bool b)
    {
        foreach (Room r in AllRooms)
        {
            r.ShowFloor(b);
        }
    }
    public void ShowWalls(bool b)
    {
        foreach (Room r in AllRooms)
        {
            r.ShowWalls(b);
        }
    }
    public void ShowSystems(bool b)
    {
        foreach (Room r in AllRooms)
        {
            r.ShowSystem(b);
        }
    }
    public void ShowTires(bool b)
    {
        foreach (Room r in AllRooms)
        {
            r.ShowTire(b);
        }
    }
    public void LoadRoomAtPos(int x, int y)
    {
        Room room = Instantiate(Resources.Load(CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].RoomPrefabPath, typeof(Room))) as Room;
        room.transform.parent = RoomsParent.transform;
        room.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);


        AllRooms.Add(room);

        // Set the Room Positions
        int roomPosNr = 0;
        for (int roomY = 0; roomY < room._sizeY; roomY++)
        {
            for (int roomX = 0; roomX < room._sizeX; roomX++)
            {
                _roomPosMatrix[x + roomX, y + roomY] = room.allRoomPositions[roomPosNr];
                _roomPosMatrix[x + roomX, y + roomY]._xPos = x + _roomPosMatrix[x + roomX, y + roomY]._xRel;
                _roomPosMatrix[x + roomX, y + roomY]._yPos = y + _roomPosMatrix[x + roomX, y + roomY]._yRel;

                _roomPosMatrix[x + roomX, y + roomY].name = "X" + _roomPosMatrix[x + roomX, y + roomY]._xPos.ToString()
                                                      + " , Y" + _roomPosMatrix[x + roomX, y + roomY]._yPos.ToString() + ", ";
                roomPosNr++;
            }
        }

        //sets the corner of the room that doesnt get caught with the matrix
        _roomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1] = room.allRoomPositions[room._sizeX * room._sizeY - 1];
        _roomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1]._xPos = x + room._sizeX - 1;
        _roomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1]._yPos = y + room._sizeY - 1;

        _roomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1].name = "X" + (x + room._sizeX - 1).ToString() + " , Y" + (y + room._sizeY - 1).ToString();

        room._floorType = CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].FloorType;
        room._roofType = CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].RoofType;
        ChangeFloorAtPos(x, y, room._floorType);
        ChangeRoofAtPos(x, y, room._roofType);
    }
    public void CreateRoomAtPos(int x, int y, GameObject roomToCreate)
    {
        if (roomToCreate == null)
        {
            Debug.Log("Destroy");
            if(_roomPosMatrix[x, y]) Destroy(_roomPosMatrix[x, y].ParentRoom.gameObject);
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y] = new TankRoomConstellation.RoomInfo();
            Destroy(_roomPosMatrix[x, y]._spawnedTire);
            _roomPosMatrix[x, y] = null;
            return;
        }
        CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].RoomPrefabPath = GS.RoomPrefabs(roomToCreate.name);

        GameObject rGO = Instantiate(roomToCreate);
        Room room = rGO.GetComponent<Room>();
        rGO.transform.parent = RoomsParent.transform;
        rGO.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
        AllRooms.Add(room);

        // Set the Room Positions
        int roomPosNr = 0;
        for (int roomY = 0; roomY < room._sizeY; roomY++)
        {
            for (int roomX = 0; roomX < room._sizeX; roomX++)
            {
                _roomPosMatrix[x + roomX, y + roomY] = room.allRoomPositions[roomPosNr];
                _roomPosMatrix[x + roomX, y + roomY]._xPos = x + _roomPosMatrix[x + roomX, y + roomY]._xRel;
                _roomPosMatrix[x + roomX, y + roomY]._yPos = y + _roomPosMatrix[x + roomX, y + roomY]._yRel;

                _roomPosMatrix[x + roomX, y + roomY].name = "X" + _roomPosMatrix[x + roomX, y + roomY]._xPos.ToString()
                                                      + " , Y" + _roomPosMatrix[x + roomX, y + roomY]._yPos.ToString() + ", ";
                roomPosNr++;
            }
        }

        //sets the corner of the room that doesnt get caught with the matrix
        _roomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1] = room.allRoomPositions[room._sizeX * room._sizeY - 1];
        _roomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1]._xPos = x + room._sizeX - 1;
        _roomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1]._yPos = y + room._sizeY - 1;

        _roomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1].name = "X" + (x + room._sizeX - 1).ToString() + " , Y" + (y + room._sizeY - 1).ToString();


        room._floorType = CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].FloorType;
        room._roofType = CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].RoofType;
        ChangeFloorAtPos(x, y, room._floorType);
        ChangeRoofAtPos(x, y, room._roofType);
    }

    //  Walls

    public void LoadWalls()
    {
        for (int y = 0; y < CreateTankSceneManager.instance.tankToEdit._savedYSize; y++)
        {
            for (int x = 0; x < CreateTankSceneManager.instance.tankToEdit._savedXSize; x++)
            {
                if (x >= CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray.Length || y >= CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[0].YStuff.Length) continue;

                if (CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y]._topWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedTopWall = wall;
                }
                if (CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y]._rightWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedRightWall = wall;
                }
                if (CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y]._bottomWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedBottomWall = wall;
                }
                if (CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y]._leftWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallLeft"), typeof(GameObject)) as GameObject);
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
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY]._topWallExists = false;
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY]._rightWallExists = false;
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY]._bottomWallExists = false;
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY]._leftWallExists = false;

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
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY]._topWallExists = true;
            wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject);
            _roomPosMatrix[posX, posY]._spawnedTopWall = wall;
        }
        else if (direction == "left")
        {
            if (_roomPosMatrix[posX, posY]._spawnedLeftWall != null) return;
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY]._leftWallExists = true;
            wall = Instantiate(Resources.Load(GS.WallPrefabs("WallLeft"), typeof(GameObject)) as GameObject);
            _roomPosMatrix[posX, posY]._spawnedLeftWall = wall;
        }
        else if (direction == "right")
        {
            if (_roomPosMatrix[posX, posY]._spawnedRightWall != null) return;
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY]._rightWallExists = true;
            wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
            _roomPosMatrix[posX, posY]._spawnedRightWall = wall;
        }
        else if (direction == "down")
        {
            if (_roomPosMatrix[posX, posY]._spawnedBottomWall != null) return;
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY]._bottomWallExists = true;
            wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
            _roomPosMatrix[posX, posY]._spawnedBottomWall = wall;
        }
        wall.transform.SetParent(_roomPosMatrix[posX, posY].transform);
        wall.transform.localPosition = Vector3.zero;
    }

    //  Tires

    public void LoadTires()
    {
        GameObject rotatableObjects = new GameObject("RotatableObjects");
        rotatableObjects.transform.parent = RoomsParent.transform;
        rotatableObjects.transform.localPosition = Vector3.zero;

        for (int x = 0; x < CreateTankSceneManager.instance.tankToEdit._savedXSize; x++)
        {
            for (int y = 0; y < CreateTankSceneManager.instance.tankToEdit._savedYSize; y++)
            {
                if (_roomPosMatrix[x, y] == null) continue;
                if (CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].MovementPrefabPath != "")
                {
                    GameObject tire = Instantiate(Resources.Load(CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].MovementPrefabPath, typeof(GameObject)) as GameObject);
                    tire.transform.parent = _roomPosMatrix[x, y].transform;
                    tire.transform.localPosition = Vector3.zero;
                    tire.transform.parent = rotatableObjects.transform;
                    _roomPosMatrix[x, y]._spawnedTire = tire.gameObject;
                }
            }
        }

    }

    public void CreateTireAtPos(int posX, int posY, GameObject movementPrefab)
    {
        if (movementPrefab == null) //delete tire
        {
            if (_roomPosMatrix[posX, posY]._spawnedTire)
            {
                Destroy(_roomPosMatrix[posX, posY]._spawnedTire);
                _roomPosMatrix[posX, posY]._spawnedTire = null;
            }
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY].MovementPrefabPath = null;
            return;
        }

        CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY].MovementPrefabPath = GS.Movement(movementPrefab.name);
        GameObject tire = Instantiate(movementPrefab);
        tire.transform.SetParent(_roomPosMatrix[posX, posY].transform);
        tire.transform.localPosition = Vector3.zero;
    }

    //  Systems

    public void LoadSystems()
    {
        for (int x = 0; x < CreateTankSceneManager.instance.tankToEdit._savedXSize; x++)
        {
            for (int y = 0; y < CreateTankSceneManager.instance.tankToEdit._savedYSize; y++)
            {
                if (_roomPosMatrix[x, y] == null) continue;
                if (CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].SystemPrefabPath != "")
                {
                    GameObject sysObjects = Instantiate(Resources.Load(CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].SystemPrefabPath, typeof(GameObject))) as GameObject;
                    ASystem system = sysObjects.GetComponent<ASystem>();
                    system._direction = CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[x].YStuff[y].SystemDirection;
                    system.SpawnInCorrectDirection();
                    if (system.TryGetComponent(out AWeapon wep))
                    {
                        Destroy(wep.WeaponUI.gameObject);
                    }
                    sysObjects.transform.parent = _roomPosMatrix[x, y].transform;
                    sysObjects.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedSystem = sysObjects.gameObject;
                }
            }
        }

    }

    public void CreateSystemAtPos(int posX, int posY, GameObject sysPrefab)
    {
        //delete
        if (sysPrefab == null)
        {
            if (!_roomPosMatrix[posX, posY]) return;
            if (_roomPosMatrix[posX, posY]._spawnedSystem)
            {
                Destroy(_roomPosMatrix[posX, posY]._spawnedSystem);
                _roomPosMatrix[posX, posY]._spawnedSystem = null;
            }
            CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY].SystemPrefabPath = null;
            return;
        }
        // spawn

        CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY].SystemPrefabPath = GS.SystemPrefabs(sysPrefab.name);

        CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY].SystemDirection =
            Enum.GetValues(typeof(ASystem.DirectionToSpawnIn)).Cast<ASystem.DirectionToSpawnIn>().ToList()[CreateTankSceneManager.instance._tUI._directionDropDown.value];

        GameObject sysObj = Instantiate(sysPrefab);
        ASystem system = sysObj.GetComponent<ASystem>();
        if (sysObj.TryGetComponent(out AWeapon wep)) Destroy(wep.WeaponUI.gameObject);

        system._direction = CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[posX].YStuff[posY].SystemDirection;
        system.SpawnInCorrectDirection();
        sysObj.transform.SetParent(_roomPosMatrix[posX, posY].transform);
        sysObj.transform.localPosition = Vector3.zero;
        _roomPosMatrix[posX, posY]._spawnedSystem = sysObj.gameObject;
    }


    //  misc for now

    private void PositionSystemInRoom(ASystem system, Room parentRoom)
    {
        system.SystemObj.transform.localPosition = Vector2.zero;
        if (parentRoom._sizeX > 1) system.SystemObj.transform.localPosition += new Vector3(0.25f, 0);
        if (parentRoom._sizeY > 1) system.SystemObj.transform.localPosition += new Vector3(0, -0.25f);
    }
    private void PositionTankObjects()
    {
        //create overarching object for all our spawned objects
        TankGeometryParent = new GameObject("Tank Geometry Parent");
        TankGeometryParent.transform.parent = gameObject.transform;
        TankGeometryParent.transform.localPosition = Vector3.zero;

        // parent all spawnedObjects to this parent
        RoomsParent.transform.parent = TankGeometryParent.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        TankGeometryParent.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * CreateTankSceneManager.instance.tankToEdit._savedXSize, 0.25f * CreateTankSceneManager.instance.tankToEdit._savedYSize, 0);

        _visibleGrid = new GameObject("VisibleGrid");
        SpriteRenderer sr = _visibleGrid.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        sr.sprite = Resources.Load(GS.UIGraphics("White Square Border"), typeof(Sprite)) as Sprite;

        ResizeGrid();
    }
    public void VisualizeMatrix()
    {
        string matrix = "";
        for (int y = 0; y < CreateTankSceneManager.instance.tankToEdit._savedYSize; y++)
        {
            matrix += "Y:" + y.ToString() + ": ";
            for (int x = 0; x < CreateTankSceneManager.instance.tankToEdit._savedXSize; x++)
            {
                if (_roomPosMatrix[x, y]) matrix += "(" + _roomPosMatrix[x, y].name + ") ";
                else matrix += "__NONE__, ";
            }
            matrix += "\n";
        }
        print(matrix);
    }

    public void ModifyTankSize(int left, int right, int up, int down)
    {
        int xOldTankSize = CreateTankSceneManager.instance.tankToEdit._tmpXSize;
        int yOldTankSize = CreateTankSceneManager.instance.tankToEdit._tmpYSize;

        int xExpandedTankSize = xOldTankSize + left + right;
        int yExpandedTankSize = yOldTankSize + up + down;

        //  Shrink in case of negative parameters
        if (yExpandedTankSize < 0 || xExpandedTankSize < 0) return;
        if (left < 0 || right < 0 || up < 0 || down < 0)
        {
            if (left < 0) for (int y = 0; y < yOldTankSize; y++) for (int x = 0; x < Mathf.Abs(left); x++) DeleteRoomAtPos(x, y);
            if (right < 0) for (int y = 0; y < yOldTankSize; y++) for (int x = xOldTankSize - 1; x >= xOldTankSize + right; x--) DeleteRoomAtPos(x, y);
            if (up < 0) for (int x = 0; x < xOldTankSize; x++) for (int y = 0; y < Mathf.Abs(up); y++) DeleteRoomAtPos(x, y);
            if (down < 0) for (int x = 0; x < xOldTankSize; x++) for (int y = yOldTankSize - 1; y >= yOldTankSize + down; y--) DeleteRoomAtPos(x, y);
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

                if (xCopyPos < 0 || xCopyPos >= CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray.Length
                    || yCopyPos < 0 || yCopyPos >= CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[0].YStuff.Length) continue;

                //  Fix References for the matrix
                expandedMatrix.XArray[x].YStuff[y] = CreateTankSceneManager.instance.tankToEdit._tmpMatrix.XArray[xCopyPos].YStuff[yCopyPos];

                //  Set indices of individual room positions and move them to the appropriate position
                if (!_roomPosMatrix[xCopyPos, yCopyPos]) continue;
                expandedPosMatrix[x, y] = _roomPosMatrix[xCopyPos, yCopyPos];
                expandedPosMatrix[x, y]._xPos = x;
                expandedPosMatrix[x, y]._yPos = y;
                //if (expandedPosMatrix[x, y]._xRel == 0 && expandedPosMatrix[x, y]._yRel == 0) expandedPosMatrix[x, y].ParentRoom.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
            }
        }
        //  Reposition geometry

        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * (left + right), 0.25f * (up + down), 0);

        //  Save Changes into the temp

        CreateTankSceneManager.instance.tankToEdit._tmpMatrix = expandedMatrix;
        _roomPosMatrix = expandedPosMatrix;
        CreateTankSceneManager.instance.tankToEdit._tmpXSize = xExpandedTankSize;
        CreateTankSceneManager.instance.tankToEdit._tmpYSize = yExpandedTankSize;

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
            for (int x = 0; x < sizeX + left; x++)
            {
                actualTilemap.SetTile(new Vector3Int(x, -y, 0), tilemapStorage.GetTile(new Vector3Int(x - left, -(y - up), 0)));
            }
        }
        Destroy(tmpObj);
    }
    private void ResizeGrid()
    {
        _visibleGrid.transform.localScale = new Vector3(CreateTankSceneManager.instance.tankToEdit._tmpXSize, CreateTankSceneManager.instance.tankToEdit._tmpYSize, 0);
        CreateTankSceneManager.instance._tUI.UpdateSize(CreateTankSceneManager.instance.tankToEdit._tmpXSize, CreateTankSceneManager.instance.tankToEdit._tmpYSize);
    }
    private Vector2Int GetRoomSize(int x, int y)
    {
        return new Vector2Int(_roomPosMatrix[x, y].ParentRoom._sizeX, _roomPosMatrix[x, y].ParentRoom._sizeY);
    }
}