using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ConstructionSceneManager;
using static PlayerData;
using static TankRoomConstellation;

public class ConstructionSceneGeometry : MonoBehaviour
{
    public static ConstructionSceneGeometry instance;

    public RoomPosition[,] _roomPosMatrix;
    public GameObject TankGeometryParent { get; private set; }
    public GameObject RoomsParent { get; private set; }
    public List<Room> AllRooms { get; private set; }
    private GameObject _tankBounds;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        int modifier = 1;
        if (Input.GetKey(KeyCode.LeftShift)) modifier = -1;
        if (Input.GetKeyDown(KeyCode.A))
        {
            ModifyVehicleSize(1 * modifier, 0, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ModifyVehicleSize(0, 1 * modifier, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ModifyVehicleSize(0, 0, 1 * modifier, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ModifyVehicleSize(0, 0, 0, 1 * modifier);
        }
    }
    public void LoadVehicle()
    {
        LoadRooms();
        PositionTankObjects();
        LoadWalls();
        LoadTires();
        LoadSystems();
    }
    public void DeleteRoomAtPos(int roomPositionX, int roomPositionY)
    {
        if (_roomPosMatrix[roomPositionX, roomPositionY])
        {
            if (_roomPosMatrix[roomPositionX, roomPositionY].ParentRoom)
            {
                ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[roomPositionX].YStuff[roomPositionY].RoomPrefabPath = null;

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
        _roomPosMatrix = new RoomPosition[ConstructionSceneManager.instance._tmpVehicleData._savedXSize, ConstructionSceneManager.instance._tmpVehicleData._savedYSize];
        AllRooms = new List<Room>();
        if (_tankBounds) Destroy(_tankBounds);

        if (RoomsParent) Destroy(RoomsParent);
        RoomsParent = new GameObject("All Tank Rooms");
        RoomsParent.transform.parent = transform;
        RoomsParent.transform.localPosition = Vector3.zero;

        for (int y = 0; y < ConstructionSceneManager.instance._tmpVehicleData._savedYSize; y++)
        {
            for (int x = 0; x < ConstructionSceneManager.instance._tmpVehicleData._savedXSize; x++)
            {
                if (x >= ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray.Length
                    || y >= ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[0].YStuff.Length
                    || ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y] == null
                    || ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].RoomPrefabPath == null
                    || ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].RoomPrefabPath == "") continue;
                LoadRoomAtPos(x, y);
            }
        }
    }
    public void LoadRoomAtPos(int x, int y)
    {
        Room room = Instantiate(Resources.Load(ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].RoomPrefabPath, typeof(Room))) as Room;
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

        room._floorType = ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].FloorType;
        room._roofType = ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].RoofType;
        ChangeFloorAtPos(x, y, room._floorType);
        ChangeRoofAtPos(x, y, room._roofType);
    }
    public void CreateRoomAtPos(int x, int y, GameObject roomToCreate)
    {
        if (roomToCreate == null)
        {
            if (_roomPosMatrix[x, y])
                Destroy(_roomPosMatrix[x, y].ParentRoom.gameObject);
            Destroy(_roomPosMatrix[x, y]._spawnedTire);
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y] = PlayerData.RoomInfo.NewRoomInfo();

            AllRooms.Remove(_roomPosMatrix[x, y].ParentRoom);
            _roomPosMatrix[x, y] = null;
            return;
        }
        ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y] = new PlayerData.RoomInfo
        {
            RoomPrefabPath = GS.RoomPrefabs(roomToCreate.name),
            FloorType = FloorType.FloorA,
            RoofType = RoofType.RoofA,
            RoomCurrentHP = ConstructionSceneManager.instance._tmpVehicleData.VehicleRoomMaxHP
        };

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


        room._floorType = ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].FloorType;
        room._roofType = ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].RoofType;
        ChangeFloorAtPos(x, y, room._floorType);
        ChangeRoofAtPos(x, y, room._roofType);
    }

    public void ChangeFloorAtPos(int x, int y, FloorType floorType)
    {
        ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].FloorType = floorType;
        _roomPosMatrix[x, y].ParentRoom._floorRenderer.sprite = Resources.Load(GS.RoomGraphics(floorType.ToString()) + "3", typeof(Sprite)) as Sprite;
        _roomPosMatrix[x, y].ParentRoom.ShowFloor(ConstructionSceneUI.instance._floorShown);
    }
    public void ChangeRoofAtPos(int x, int y, RoofType roofType)
    {
        ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].RoofType = roofType;
        _roomPosMatrix[x, y].ParentRoom._roofRenderer.sprite = Resources.Load(GS.RoomGraphics(roofType.ToString()), typeof(Sprite)) as Sprite;

        _roomPosMatrix[x, y].ParentRoom.ShowRoof(ConstructionSceneUI.instance._roofShown);
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
    //  Walls

    public void LoadWalls()
    {
        for (int y = 0; y < ConstructionSceneManager.instance._tmpVehicleData._savedYSize; y++)
        {
            for (int x = 0; x < ConstructionSceneManager.instance._tmpVehicleData._savedXSize; x++)
            {
                if (x >= ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray.Length || y >= ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[0].YStuff.Length) continue;
                if (ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].Equals(new PlayerData.RoomInfo())) continue;
                if (ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y]._topWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedTopWall = wall;
                }
                if (ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y]._rightWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedRightWall = wall;
                }
                if (ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y]._bottomWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                    _roomPosMatrix[x, y]._spawnedBottomWall = wall;
                }
                if (ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y]._leftWallExists)
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
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY]._topWallExists = false;
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY]._rightWallExists = false;
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY]._bottomWallExists = false;
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY]._leftWallExists = false;

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
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY]._topWallExists = true;
            wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject);
            _roomPosMatrix[posX, posY]._spawnedTopWall = wall;
        }
        else if (direction == "left")
        {
            if (_roomPosMatrix[posX, posY]._spawnedLeftWall != null) return;
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY]._leftWallExists = true;
            wall = Instantiate(Resources.Load(GS.WallPrefabs("WallLeft"), typeof(GameObject)) as GameObject);
            _roomPosMatrix[posX, posY]._spawnedLeftWall = wall;
        }
        else if (direction == "right")
        {
            if (_roomPosMatrix[posX, posY]._spawnedRightWall != null) return;
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY]._rightWallExists = true;
            wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
            _roomPosMatrix[posX, posY]._spawnedRightWall = wall;
        }
        else if (direction == "down")
        {
            if (_roomPosMatrix[posX, posY]._spawnedBottomWall != null) return;
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY]._bottomWallExists = true;
            wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
            _roomPosMatrix[posX, posY]._spawnedBottomWall = wall;
        }
        wall.transform.SetParent(_roomPosMatrix[posX, posY].transform);
        wall.transform.localPosition = Vector3.zero;
    }

    //  Tires

    public void LoadTires()
    {
        for (int x = 0; x < ConstructionSceneManager.instance._tmpVehicleData._savedXSize; x++)
        {
            for (int y = 0; y < ConstructionSceneManager.instance._tmpVehicleData._savedYSize; y++)
            {
                if (_roomPosMatrix[x, y] == null
                    || ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].MovementPrefabPath == null
                    || ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].MovementPrefabPath == "") continue;

                GameObject tire = Instantiate(Resources.Load(ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].MovementPrefabPath, typeof(GameObject)) as GameObject);
                tire.transform.parent = _roomPosMatrix[x, y].transform;
                tire.transform.localPosition = Vector3.zero;
                _roomPosMatrix[x, y]._spawnedTire = tire.gameObject;
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
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY].MovementPrefabPath = null;
            return;
        }

        if (_roomPosMatrix[posX, posY]._spawnedTire) Destroy(_roomPosMatrix[posX, posY]._spawnedTire);

        ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY].MovementPrefabPath = GS.Movement(movementPrefab.name);
        GameObject tire = Instantiate(movementPrefab);
        _roomPosMatrix[posX, posY]._spawnedTire = tire;
        tire.transform.SetParent(_roomPosMatrix[posX, posY].transform);
        tire.transform.localPosition = Vector3.zero;
    }

    //  Systems

    public void LoadSystems()
    {
        for (int x = 0; x < ConstructionSceneManager.instance._tmpVehicleData._savedXSize; x++)
        {
            for (int y = 0; y < ConstructionSceneManager.instance._tmpVehicleData._savedYSize; y++)
            {
                if (_roomPosMatrix[x, y] == null
                    || ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath == null
                    || ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath == "") continue;

                GameObject sysObjects = Instantiate(Resources.Load(ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath, typeof(GameObject))) as GameObject;
                ASystem system = sysObjects.GetComponent<ASystem>();
                system._direction = ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemDirection;
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
            ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY].SystemPrefabPath = "";
            return;
        }
        // spawn

        if (_roomPosMatrix[posX, posY]._spawnedSystem) Destroy(_roomPosMatrix[posX, posY]._spawnedSystem);

        ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY].SystemPrefabPath = GS.SystemPrefabs(sysPrefab.name);
        ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY].SystemDirection =
            Enum.GetValues(typeof(ASystem.DirectionToSpawnIn)).Cast<ASystem.DirectionToSpawnIn>().ToList()[ConstructionSceneUI.instance._directionDropDown.value];

        GameObject sysObj = Instantiate(sysPrefab);
        ASystem system = sysObj.GetComponent<ASystem>();
        if (sysObj.TryGetComponent(out AWeapon wep)) Destroy(wep.WeaponUI.gameObject);

        system._direction = ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[posX].YStuff[posY].SystemDirection;
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
        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * ConstructionSceneManager.instance._tmpVehicleData._savedXSize, 0.25f * ConstructionSceneManager.instance._tmpVehicleData._savedYSize, 0);

        _tankBounds = new GameObject("TankBounds");
        _tankBounds.transform.SetParent(transform, false);
        SpriteRenderer sr = _tankBounds.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "VehicleUI";
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.color = new Color(1f, 0.2f, 0.2f, 0.25f);
        sr.sprite = Resources.Load(GS.UIGraphics("TankBounds"), typeof(Sprite)) as Sprite;
        ResizeTankBounds(new Vector2Int(ConstructionSceneManager.instance._tmpVehicleData._savedXSize, ConstructionSceneManager.instance._tmpVehicleData._savedYSize));
        ConstructionSceneTools.instance._alignmentGrid.transform.position =
            _tankBounds.transform.position +
            new Vector3(ConstructionSceneManager.instance._tmpVehicleData._savedXSize / -4f, ConstructionSceneManager.instance._tmpVehicleData._savedYSize / 4f);
    }
    public void PrintMatrix()
    {
        string matrix = "";
        for (int y = 0; y < ConstructionSceneManager.instance._tmpVehicleData._savedYSize; y++)
        {
            matrix += "Y:" + y.ToString() + ": ";
            for (int x = 0; x < ConstructionSceneManager.instance._tmpVehicleData._savedXSize; x++)
            {
                if (_roomPosMatrix[x, y]) matrix += "(" + _roomPosMatrix[x, y].name + ") ";
                else matrix += "__NONE__, ";
            }
            matrix += "\n";
        }
        print(matrix);
    }

    public void ModifyVehicleSize(int left, int right, int up, int down)
    {
        int xOldTankSize = ConstructionSceneManager.instance._tmpVehicleData._savedXSize;
        int yOldTankSize = ConstructionSceneManager.instance._tmpVehicleData._savedYSize;

        if (left < 0 && xOldTankSize <= 1 || right < 0 && xOldTankSize <= 1 || up < 0 && yOldTankSize <= 1 || down < 0 && yOldTankSize <= 1)
        {
            Debug.Log("Tank Bounds would be smaller than 1, returning!");
            return;
        }

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

        PlayerVehicleMatrix expandedMatrix = new PlayerVehicleMatrix(xExpandedTankSize, yExpandedTankSize);
        RoomPosition[,] expandedPosMatrix = new RoomPosition[xExpandedTankSize, yExpandedTankSize];

        for (int y = 0; y < yExpandedTankSize; y++)
        {
            for (int x = 0; x < xExpandedTankSize; x++)
            {
                int xCopyPos = x - left;
                int yCopyPos = y - up;

                if (xCopyPos < 0 || xCopyPos >= ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray.Length
                    || yCopyPos < 0 || yCopyPos >= ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[0].YStuff.Length) continue;

                //  Fix References for the matrix
                expandedMatrix.XArray[x].YStuff[y] = ConstructionSceneManager.instance._tmpVehicleData.VehicleMatrix.XArray[xCopyPos].YStuff[yCopyPos];

                //  Set indices of individual room positions and move them to the appropriate position
                if (!_roomPosMatrix[xCopyPos, yCopyPos]) continue;
                expandedPosMatrix[x, y] = _roomPosMatrix[xCopyPos, yCopyPos];
                expandedPosMatrix[x, y]._xPos = x;
                expandedPosMatrix[x, y]._yPos = y;
            }
        }
        //  Save Changes into the temp

        VehicleData expandedVehicleData = new VehicleData()
        {
            VehicleMatrix = expandedMatrix,
            _savedXSize = xExpandedTankSize,
            _savedYSize = yExpandedTankSize,
            VehicleRoomMaxHP = ConstructionSceneManager.instance._tmpVehicleData.VehicleRoomMaxHP,

            FloorColorR = ConstructionSceneManager.instance._tmpVehicleData.FloorColorR,
            FloorColorG = ConstructionSceneManager.instance._tmpVehicleData.FloorColorG,
            FloorColorB = ConstructionSceneManager.instance._tmpVehicleData.FloorColorB,

            RoofColorR = ConstructionSceneManager.instance._tmpVehicleData.RoofColorR,
            RoofColorG = ConstructionSceneManager.instance._tmpVehicleData.RoofColorG,
            RoofColorB = ConstructionSceneManager.instance._tmpVehicleData.RoofColorB,
        };

        DataStorage.CopyVehicleDataFromTo(expandedVehicleData, ref ConstructionSceneManager.instance._tmpVehicleData);
        _roomPosMatrix = expandedPosMatrix;

        //Show Grid Size & Reposition geometry

        ResizeTankBounds(new Vector2Int(xExpandedTankSize, yExpandedTankSize));
        _tankBounds.transform.localPosition += 0.25f * new Vector3(-left + right, up - down, 0);

        Vector3 shiftVector = 0.5f * new Vector3(-left, up);
        ShiftSpawnedVehicleParts(shiftVector);
        ConstructionSceneTools.instance._alignmentGrid.transform.localPosition += shiftVector;
    }
    private void ShiftSpawnedVehicleParts(Vector3 shiftVector)
    {
        RoomsParent.transform.localPosition += shiftVector;
        foreach (Room r in AllRooms)
        {
            if (r) r.transform.localPosition -= shiftVector;
        }
    }
    private void ResizeTankBounds(Vector2Int newSize)
    {
        _tankBounds.GetComponent<SpriteRenderer>().size = new Vector2(newSize.x / 2f, newSize.y / 2f);
        ConstructionSceneUI.instance.UpdateSize(newSize.x, newSize.y);
    }
    private Vector2Int GetRoomSize(int x, int y)
    {
        return new Vector2Int(_roomPosMatrix[x, y].ParentRoom._sizeX, _roomPosMatrix[x, y].ParentRoom._sizeY);
    }
}