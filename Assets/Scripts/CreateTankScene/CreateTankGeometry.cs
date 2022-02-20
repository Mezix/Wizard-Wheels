using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TankRoomConstellation;

public class CreateTankGeometry : MonoBehaviour
{
    public TankRoomConstellation _tankRoomConstellation;
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

    public int offsetL = 0;
    public int offsetR = 0;
    public int offsetU = 0;
    public int offsetD = 0;

    //public Color FloorColor;
    //public Color RoofColor;
    public void SpawnTankForCreator()
    {
        LoadRooms();
        CreateBGAndRoof();
        PositionTankObjects();
        //InitWeaponsAndSystems();
        //CreateWalls();
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
    private void CreateBGAndRoof()
    {
        CreateFloorAndRoofTilemap();
        for (int x = 0; x < _tankRoomConstellation._X; x++)
        {
            for (int y = 0; y < _tankRoomConstellation._Y; y++)
            {
                if (x >= _tankRoomConstellation.SavedPrefabRefMatrix.XArray.Length || y >= _tankRoomConstellation.SavedPrefabRefMatrix.XArray[0].YStuff.Length) continue;
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomPrefab)
                {
                    int sizeX = _tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomPrefab.GetComponent<Room>().sizeX;
                    int sizeY = _tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomPrefab.GetComponent<Room>().sizeY;
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
        //FloorTilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0);
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
        //RoofTilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0);
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
                //t.color = FloorColor;
                Tile t = (Tile)Resources.Load("Tiles\\Floor\\DefaultFloorTile");
                FloorTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);

            }
        }
    }
    public void ChangeFloorAtPos(int startX, int startY, int sizeX, int sizeY, Tile t)
    {
        startX += offsetL;
        startY += offsetU;
        //  Check if we overstepped the edges of our matrix and need to expand first!
        if (startX < 0 || startY < 0 || startX > _tankRoomConstellation._tmpX-1 || startY > _tankRoomConstellation._tmpY-1)
        {
            int expandL = Math.Abs(Math.Min(0, startX));
            int expandR = Math.Max(_tankRoomConstellation._tmpX-1, startX) - _tankRoomConstellation._tmpX+1;
            int expandU = Math.Abs(Math.Min(0, startY));
            int expandD = Math.Max(_tankRoomConstellation._tmpY-1, startY) - _tankRoomConstellation._tmpY+1;
            ModifyTankSize(expandL, expandR, expandU, expandD);
            //print("left: " + expandL + ", right: " + expandR +  ", up :" + expandU + ", down: " + expandD);
        }
        else
        {
            GameObject roomToLoad = (GameObject)Resources.Load("Rooms\\1x1Room");
            if (sizeX == 1)
            {
                if (sizeY == 2)
                {
                    roomToLoad = (GameObject)Resources.Load("Rooms\\1x2Room");

                    //check if we overlap with a room and we have to delete
                }
            }
            if (sizeX == 2)
            {
                if (sizeY == 1)
                {
                    roomToLoad = (GameObject)Resources.Load("Rooms\\2x1Room");

                    //check if we overlap with a room and we have to delete
                }
                if (sizeY == 2)
                {
                    roomToLoad = (GameObject)Resources.Load("Rooms\\2x2Room");

                    //check if we overlap with a room and we have to delete
                }
            }
            CreateNewRoomAtPos(startX, startY, roomToLoad);
        }
        //startX -= offsetL;
        //startY -= offsetU;
        if (t != null)
        {
            for (int x = startX; x < startX + sizeX; x++)
            {
                for (int y = startY; y < startY + sizeY; y++)
                {
                    //t.color = FloorColor;
                    FloorTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
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
        if (_roomPosMatrix[roomPositionX, roomPositionY])
        {
            if(_roomPosMatrix[roomPositionX, roomPositionY].ParentRoom)
            {
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
                    }
                }
                Destroy(_roomPosMatrix[roomPositionX, roomPositionY].ParentRoom.gameObject);
            }
        }
    }
    public void LoadRoofAtPos(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                Tile t = (Tile)Resources.Load("Tiles\\Roof\\DefaultRoofTile");
                //t.color = RoofColor;
                RoofTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
            }
        }
    }
    public void CreateRoofAtPos(int startX, int startY, int sizeX, int sizeY, Tile t)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                //t.color = RoofColor;
                RoofTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
            }
        }
    }
    private void LoadRooms()
    {
        _roomPosMatrix = new RoomPosition[_tankRoomConstellation._X, _tankRoomConstellation._Y];
        AllRooms = new List<Room>();

        RoomsParent = new GameObject("All Tank Rooms");
        RoomsParent.transform.parent = gameObject.transform;
        RoomsParent.transform.localPosition = Vector3.zero;

        for (int y = 0; y < _tankRoomConstellation._Y; y++)
        {
            for (int x = 0; x < _tankRoomConstellation._X; x++)
            {
                if (x >= _tankRoomConstellation.SavedPrefabRefMatrix.XArray.Length || y >= _tankRoomConstellation.SavedPrefabRefMatrix.XArray[0].YStuff.Length) continue;
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomPrefab)
                {
                    LoadRoomAtPos(x,y);
                }
            }
        }
    }
    public void LoadRoomAtPos(int x, int y)
    {
        GameObject rGO = Instantiate(_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomPrefab);
        Room r = rGO.GetComponent<Room>();
        r.tr = _tankRoomConstellation;
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
    public void CreateNewRoomAtPos(int x, int y, GameObject roomToCreate)
    {
        GameObject rGO = Instantiate(roomToCreate);
        Room r = rGO.GetComponent<Room>();
        r.tr = _tankRoomConstellation;
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
    public void CreateSystemIcons()
    {
        for (int x = 0; x < _tankRoomConstellation._X; x++)
        {
            for (int y = 0; y < _tankRoomConstellation._Y; y++)
            {
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab)
                {
                    ISystem sys = _tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<ISystem>();
                    if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.TryGetComponent(out AWeapon wep))
                    {
                        _roomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = Resources.Load("Art\\WeaponSystemIcon", typeof(Sprite)) as Sprite;
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
    public void CreateWalls()
    {
        for (int y = 0; y < _tankRoomConstellation._Y; y++)
        {
            for (int x = 0; x < _tankRoomConstellation._X; x++)
            {
                if (x >= _tankRoomConstellation.SavedPrefabRefMatrix.XArray.Length || y >= _tankRoomConstellation.SavedPrefabRefMatrix.XArray[0].YStuff.Length) continue;
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].WallUp)
                {
                    GameObject wall = (GameObject)Instantiate(Resources.Load("Rooms\\Walls\\WallUp"));
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].WallRight)
                {
                    GameObject wall = (GameObject)Instantiate(Resources.Load("Rooms\\Walls\\WallRight"));
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].WallDown)
                {
                    GameObject wall = (GameObject)Instantiate(Resources.Load("Rooms\\Walls\\WallDown"));
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].WallLeft)
                {
                    GameObject wall = (GameObject)Instantiate(Resources.Load("Rooms\\Walls\\WallLeft"));
                    wall.transform.SetParent(_roomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
    public void InitWeaponsAndSystems()
    {
        TankWeaponsAndSystems twep = GetComponent<TankWeaponsAndSystems>();
        for (int x = 0; x < _tankRoomConstellation._X; x++)
        {
            for (int y = 0; y < _tankRoomConstellation._Y; y++)
            {
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab)
                {
                    GameObject prefab = _tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab;
                    //Our object should either be a Weapon or a System, so check for both cases
                    if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<AWeapon>() != null)
                    {
                        //if (!RoomPosMatrix[x, y]) continue;
                        GameObject weaponObj = Instantiate(prefab);
                        weaponObj.transform.parent = _roomPosMatrix[x, y].ParentRoom.transform;
                        weaponObj.transform.localPosition = Vector3.zero;
                        AWeapon wep = weaponObj.GetComponent<AWeapon>();
                        PositionSystemInRoom(weaponObj.GetComponent<ISystem>(), weaponObj.transform.parent.GetComponent<Room>());
                        wep.RoomPosForInteraction = _roomPosMatrix[x, y].ParentRoom.allRoomPositions[0];
                        twep.AWeaponArray.Add(wep);

                        //Set the reference to the rooms
                        _roomPosMatrix[x, y].ParentRoom.roomSystem = wep;
                    }
                    else if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<ISystem>() != null)
                    {
                        GameObject systemObj = Instantiate(prefab);
                        systemObj.transform.parent = _roomPosMatrix[x, y].ParentRoom.transform;

                        systemObj.transform.localPosition = Vector3.zero;
                        ISystem sys = systemObj.GetComponent<ISystem>();
                        PositionSystemInRoom(systemObj.GetComponent<ISystem>(), systemObj.transform.parent.GetComponent<Room>());
                        twep.ISystemArray.Add(sys);
                        sys.RoomPosForInteraction = _roomPosMatrix[x, y].ParentRoom.allRoomPositions[0];

                        //Set the reference to the rooms
                        _roomPosMatrix[x, y].ParentRoom.roomSystem = sys;
                    }
                }
            }
        }
    }
    private void PositionSystemInRoom(ISystem system, Room parentRoom)
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
        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * _tankRoomConstellation._X, 0.25f * _tankRoomConstellation._Y, 0);

        _visibleGrid = new GameObject("VisibleGrid");
        SpriteRenderer sr = _visibleGrid.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load("Art\\white_square_border", typeof(Sprite)) as Sprite;

        ResizeGrid();
    }
    public void ShowRoof(bool b)
    {
        if (RoofParent) RoofParent.SetActive(b);
        SetSystemIconLayer(b);
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
        for (int y = 0; y < _tankRoomConstellation._Y; y++)
        {
            matrix += "Y:" + y.ToString() + ": ";
            for (int x = 0; x < _tankRoomConstellation._X; x++)
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
        offsetL += left;
        offsetR += right;
        offsetU += up;
        offsetD += down;

        int xOldTankSize = _tankRoomConstellation._tmpX;
        int yOldTankSize = _tankRoomConstellation._tmpY;

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

                if (xCopyPos < 0 || xCopyPos >= _tankRoomConstellation._tmpPrefabRefMatrix.XArray.Length
                    || yCopyPos < 0 || yCopyPos >= _tankRoomConstellation._tmpPrefabRefMatrix.XArray[0].YStuff.Length) continue;

                //  Fix References for the matrix
                expandedMatrix.XArray[x].YStuff[y] = _tankRoomConstellation._tmpPrefabRefMatrix.XArray[xCopyPos].YStuff[yCopyPos];

                //  Set indices of individual room positions and move them to the appropriate position
                if (!_roomPosMatrix[xCopyPos, yCopyPos]) continue;
                expandedPosMatrix[x, y] = _roomPosMatrix[xCopyPos, yCopyPos];
                expandedPosMatrix[x, y]._xPos = x;
                expandedPosMatrix[x, y]._yPos = y;
                if (expandedPosMatrix[x, y]._xRel == 0 && expandedPosMatrix[x, y]._yRel == 0)
                    expandedPosMatrix[x, y].ParentRoom.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
            }
        }

        //  Reposition geometry

        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * (left + right), 0.25f * (up + down), 0);

        //  Reposition Tilemaps

        FloorTilemap.transform.position += new Vector3(0.5f * (left + right), -0.5f * (up + down), 0);
        CreateTankSceneManager.instance._tools._tempFloorGrid.transform.position = FloorTilemap.transform.position;
        RoofTilemap.transform.position += new Vector3(0.5f * (left + right), -0.5f * (up + down), 0);
        CreateTankSceneManager.instance._tools._tempRoofGrid.transform.position = FloorTilemap.transform.position;

        //  Save Changes into the temp

        _tankRoomConstellation._tmpPrefabRefMatrix = expandedMatrix;
        _roomPosMatrix = expandedPosMatrix;
        _tankRoomConstellation._tmpX = xExpandedTankSize;
        _tankRoomConstellation._tmpY = yExpandedTankSize;

        //Show Grid Size

        ResizeGrid();
    }
    private void ResizeGrid()
    {
        _visibleGrid.transform.localScale = new Vector3(_tankRoomConstellation._tmpX, _tankRoomConstellation._tmpY, 0);
    }
    private Vector2Int GetRoomSize(int x, int y)
    {
        return new Vector2Int(_roomPosMatrix[x, y].ParentRoom.sizeX, _roomPosMatrix[x, y].ParentRoom.sizeY);
    }
}