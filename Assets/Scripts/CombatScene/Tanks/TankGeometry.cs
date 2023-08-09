﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TankGeometry : MonoBehaviour
{
    public TankRoomConstellation _tankRoomConstellation;
    public RoomPosition[,] RoomPosMatrix;
    public GameObject TankGeometryParent { get; private set; }
    public GameObject RoomsParent { get; private set; }
    public List<Room> AllRooms { get; private set; }
    public Tilemap FloorTilemap { get; private set; }
    public GameObject RoofParent { get; private set; }
    public Tilemap RoofTilemap { get; private set; }
    [SerializeField]
    private List<SpriteRenderer> systemIcons = new List<SpriteRenderer>();
    public Color FloorColor;
    public Color RoofColor;
    public void CreateTankGeometry()
    {
        CreateRooms();
        CreateBGAndRoof();
        PositionTankObjects();
        InitWeaponsAndSystems();
        CreateSystemIcons();
        CreateWalls();

        if(REF.CombatUI) REF.CombatUI.TurnOnXRay(REF.CombatUI._xrayOn);
    }
    private void CreateBGAndRoof()
    {
        CreateFloorAndRoofTilemap();
        for (int x = 0; x < _tankRoomConstellation._savedXSize; x++)
        {
            for (int y = 0; y < _tankRoomConstellation._savedYSize; y++)
            {
                if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].RoomPrefab)
                {
                    int sizeX = _tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].RoomPrefab.GetComponent<Room>().sizeX;
                    int sizeY = _tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].RoomPrefab.GetComponent<Room>().sizeY;
                    CreateFloorAtPos(x, y, sizeX, sizeY);
                    CreateRoofAtTile(x, y, sizeX, sizeY);
                }
            }
        }
    }

    public void AddScrapCollector()
    {
        FloorTilemap.gameObject.AddComponent<TilemapCollider2D>();
        FloorTilemap.gameObject.GetComponent<TilemapCollider2D>().isTrigger = true;
        FloorTilemap.gameObject.AddComponent<TankScrapCollector>();
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
    private void CreateFloorAtPos(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                Tile t = (Tile)Resources.Load("Tiles/Floor/DefaultFloorTile");
                //t.color = FloorColor;
                FloorTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
                
            }
        }
    }
    private void CreateRoofAtTile(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                Tile t = (Tile)Resources.Load("Tiles/Roof/DefaultRoofTile");
                t.color = RoofColor;
                RoofTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), t);
            }
        }
    }
    private void CreateRooms()
    {
        RoomPosMatrix = new RoomPosition[_tankRoomConstellation._savedXSize, _tankRoomConstellation._savedYSize];
        AllRooms = new List<Room>();

        RoomsParent = new GameObject("All Tank Rooms");
        RoomsParent.transform.parent = gameObject.transform;
        RoomsParent.transform.localPosition = Vector3.zero;

        for (int y = 0; y < _tankRoomConstellation._savedYSize; y++)
        {
            for (int x = 0; x < _tankRoomConstellation._savedXSize; x++)
            {
                if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].RoomPrefab)
                {
                    GameObject rGO = Instantiate(_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].RoomPrefab);
                    Room r = rGO.GetComponent<Room>();
                    r.tGeo = this;
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
                            RoomPosMatrix[x + roomX, y + roomY] = r.allRoomPositions[roomPosNr];
                            RoomPosMatrix[x + roomX, y + roomY]._xPos = x + RoomPosMatrix[x + roomX, y + roomY]._xRel;
                            RoomPosMatrix[x + roomX, y + roomY]._yPos = y + RoomPosMatrix[x + roomX, y + roomY]._yRel;

                            RoomPosMatrix[x + roomX, y + roomY].name = "X" + RoomPosMatrix[x + roomX, y + roomY]._xPos.ToString() 
                                                                  + " , Y" + RoomPosMatrix[x + roomX, y + roomY]._yPos.ToString() + ", ";
                            roomPosNr++;
                        }
                    }

                    //sets the corner of the room that doesnt get caught with the matrix
                    RoomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1] = r.allRoomPositions[r.sizeX * r.sizeY-1];
                    RoomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1]._xPos = x + r.sizeX - 1;
                    RoomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1]._yPos = y + r.sizeY - 1;

                    RoomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1].name = "X" + (x + r.sizeX - 1).ToString() + " , Y" + (y + r.sizeY - 1).ToString();
                }
            }
        }
    }
    public void CreateSystemIcons()
    {
        for (int x = 0; x < _tankRoomConstellation._savedXSize; x++)
        {
            for (int y = 0; y < _tankRoomConstellation._savedYSize; y++)
            {
                if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].SystemPrefab)
                {
                    ASystem sys = _tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].SystemPrefab.GetComponent<ASystem>();
                    if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].SystemPrefab.TryGetComponent(out AWeapon wep))
                    {
                        RoomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = Resources.Load("Art/WeaponSystemIcon", typeof(Sprite)) as Sprite;
                    }
                    else
                    {
                        RoomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = sys.SystemSprite;
                        systemIcons.Add(RoomPosMatrix[x, y].ParentRoom.roomSystemRenderer);
                    }
                }
            }
        }
    }
    public void CreateWalls()
    {
        for (int y = 0; y < _tankRoomConstellation._savedYSize; y++)
        {
            for (int x = 0; x < _tankRoomConstellation._savedXSize; x++)
            {
                if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y]._topWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof (GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y]._rightWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y]._bottomWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y]._leftWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallLeft"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
    public void InitWeaponsAndSystems()
    {
        TankWeaponsAndSystems twep = GetComponent<TankWeaponsAndSystems>();
        for (int x = 0; x < _tankRoomConstellation._savedXSize; x++)
        {
            for (int y = 0; y < _tankRoomConstellation._savedYSize; y++)
            {
                if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].SystemPrefab)
                {
                    GameObject prefab = _tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].SystemPrefab;
                    //Our object should either be a Weapon or a System, so check for both cases
                    if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].SystemPrefab.GetComponent<AWeapon>() != null)
                    {
                        //if (!RoomPosMatrix[x, y]) continue;
                        GameObject weaponObj = Instantiate(prefab);
                        weaponObj.transform.parent = RoomPosMatrix[x, y].ParentRoom.transform;
                        weaponObj.transform.localPosition = Vector3.zero;
                        AWeapon wep = weaponObj.GetComponent<AWeapon>();
                        PositionSystemInRoom(weaponObj.GetComponent<ASystem>(), weaponObj.transform.parent.GetComponent<Room>());
                        wep.RoomPosForInteraction = RoomPosMatrix[x, y].ParentRoom.allRoomPositions[0];
                        twep.AWeaponArray.Add(wep);

                        //Set the reference to the rooms
                        RoomPosMatrix[x, y].ParentRoom.roomSystem = wep;
                    }
                    else if (_tankRoomConstellation._savedMatrix.XArray[x].YStuff[y].SystemPrefab.GetComponent<ASystem>() != null)
                    {
                        GameObject systemObj = Instantiate(prefab);
                        systemObj.transform.parent = RoomPosMatrix[x, y].ParentRoom.transform;

                        systemObj.transform.localPosition = Vector3.zero;
                        ASystem sys = systemObj.GetComponent<ASystem>();
                        PositionSystemInRoom(systemObj.GetComponent<ASystem>(), systemObj.transform.parent.GetComponent<Room>());
                        twep.ASystemArray.Add(sys);
                        sys.RoomPosForInteraction = RoomPosMatrix[x, y].ParentRoom.allRoomPositions[0];

                        //Set the reference to the rooms
                        RoomPosMatrix[x, y].ParentRoom.roomSystem = sys;
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
        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * _tankRoomConstellation._savedXSize, 0.25f * _tankRoomConstellation._savedYSize, 0);

        //print("finished creating Tank Geometry");
    }
    public void ShowRoof(bool b)
    {
        if(RoofParent) RoofParent.SetActive(b);
        SetSystemIconLayer(b);
    }
    public void SetSystemIconLayer(bool top)
    {
        if (systemIcons.Count == 0) return;
        foreach(SpriteRenderer sr in systemIcons)
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
        for(int i = 0; i < AllRooms.Count; i++)
        {
            Room tmpRoom = allRoomsTMP[UnityEngine.Random.Range(0, allRoomsTMP.Count - 1)];
            for(int j = 0; j < tmpRoom.freeRoomPositions.Length; j++)
            {
                if(tmpRoom.freeRoomPositions[j] != null)
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
        for (int y = 0; y < _tankRoomConstellation._savedYSize; y++)
        {
            matrix += "Y:" + y.ToString() + ": ";
            for (int x = 0; x < _tankRoomConstellation._savedXSize; x++)
            {
                if (RoomPosMatrix[x, y]) matrix += "(" + RoomPosMatrix[x, y].name + ") ";
                else matrix += "__NONE__, ";
            }
            matrix += "\n";
        }
        print(matrix);
    }
}