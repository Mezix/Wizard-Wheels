using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static PlayerData;

public class TankGeometry : MonoBehaviour
{
    public VehicleData _vehicleData;
    public RoomPosition[,] RoomPosMatrix;
    public GameObject TankGeometryParent { get; private set; }
    public GameObject RoomsParent { get; private set; }
    public List<Room> AllRooms { get; private set; }
    [SerializeField]
    private List<SpriteRenderer> systemIcons = new List<SpriteRenderer>();
    public Color FloorColor;
    public Color RoofColor;
    public int _roomMaxHP;
    public void CreateTankGeometry()
    {
        _roomMaxHP = 4;
        LoadRooms();
        PositionTankObjects();
        InitWeaponsAndSystems();
        CreateSystemIcons();
        CreateWalls();

        if (REF.CombatUI) REF.CombatUI.TurnOnXRay(REF.CombatUI._xrayOn);
    }
    private void LoadRooms()
    {
        RoomPosMatrix = new RoomPosition[_vehicleData._savedXSize, _vehicleData._savedYSize];
        AllRooms = new List<Room>();

        RoomsParent = new GameObject("All Tank Rooms");
        RoomsParent.transform.parent = gameObject.transform;
        RoomsParent.transform.localPosition = Vector3.zero;

        for (int y = 0; y < _vehicleData._savedYSize; y++)
        {
            for (int x = 0; x < _vehicleData._savedXSize; x++)
            {
                if (_vehicleData.VehicleMatrix.XArray[x].YStuff[y].Equals(new RoomInfo()) 
                    || _vehicleData.VehicleMatrix.XArray[x].YStuff[y].RoomPrefabPath == null
                    || _vehicleData.VehicleMatrix.XArray[x].YStuff[y].RoomPrefabPath == "") continue;

                Room room = Instantiate(Resources.Load(_vehicleData.VehicleMatrix.XArray[x].YStuff[y].RoomPrefabPath, typeof(Room))) as Room;
                room._tGeo = this;
                room.ID = _vehicleData.GetHashCode();
                room.gameObject.transform.parent = RoomsParent.transform;
                room.gameObject.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
                AllRooms.Add(room);

                room._floorType = _vehicleData.VehicleMatrix.XArray[x].YStuff[y].FloorType;
                room._roofType = _vehicleData.VehicleMatrix.XArray[x].YStuff[y].RoofType;
                room.InitHP(_vehicleData.VehicleMatrix.XArray[x].YStuff[y].RoomCurrentHP, _roomMaxHP);

                // Set the Room Positions
                int roomPosNr = 0;
                for (int roomY = 0; roomY < room._sizeY; roomY++)
                {
                    for (int roomX = 0; roomX < room._sizeX; roomX++)
                    {
                        RoomPosMatrix[x + roomX, y + roomY] = room.allRoomPositions[roomPosNr];
                        RoomPosMatrix[x + roomX, y + roomY]._xPos = x + RoomPosMatrix[x + roomX, y + roomY]._xRel;
                        RoomPosMatrix[x + roomX, y + roomY]._yPos = y + RoomPosMatrix[x + roomX, y + roomY]._yRel;

                        RoomPosMatrix[x + roomX, y + roomY].name = "X" + RoomPosMatrix[x + roomX, y + roomY]._xPos.ToString()
                                                              + " , Y" + RoomPosMatrix[x + roomX, y + roomY]._yPos.ToString() + ", ";
                        roomPosNr++;
                    }
                }

                //sets the corner of the room that doesnt get caught with the matrix
                RoomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1] = room.allRoomPositions[room._sizeX * room._sizeY - 1];
                RoomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1]._xPos = x + room._sizeX - 1;
                RoomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1]._yPos = y + room._sizeY - 1;

                RoomPosMatrix[x + room._sizeX - 1, y + room._sizeY - 1].name = "X" + (x + room._sizeX - 1).ToString() + " , Y" + (y + room._sizeY - 1).ToString();

            }
        }
    }
    public void CreateSystemIcons()
    {
        for (int x = 0; x < _vehicleData._savedXSize; x++)
        {
            for (int y = 0; y < _vehicleData._savedYSize; y++)
            {
                if (_vehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath != "")
                {
                    ASystem system = Resources.Load(_vehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath, typeof(ASystem)) as ASystem;
                    if (system == null) continue;
                    if (system.TryGetComponent(out AWeapon wep))
                    {
                        //RoomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = Resources.Load("Art/WeaponSystemIcon", typeof(Sprite)) as Sprite;
                    }
                    else
                    {
                        RoomPosMatrix[x, y].ParentRoom._roomSystemRenderer.sprite = system.SystemSprite;
                        systemIcons.Add(RoomPosMatrix[x, y].ParentRoom._roomSystemRenderer);
                    }
                }
            }
        }
    }
    public void CreateWalls()
    {
        for (int y = 0; y < _vehicleData._savedYSize; y++)
        {
            for (int x = 0; x < _vehicleData._savedXSize; x++)
            {
                if (_vehicleData.VehicleMatrix.XArray[x].YStuff[y]._topWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_vehicleData.VehicleMatrix.XArray[x].YStuff[y]._rightWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_vehicleData.VehicleMatrix.XArray[x].YStuff[y]._bottomWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_vehicleData.VehicleMatrix.XArray[x].YStuff[y]._leftWallExists)
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
        for (int x = 0; x < _vehicleData._savedXSize; x++)
        {
            for (int y = 0; y < _vehicleData._savedYSize; y++)
            {
                if (!RoomPosMatrix[x, y]) continue;
                if (_vehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath != "")
                {
                    ASystem system = Resources.Load(_vehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemPrefabPath, typeof(ASystem)) as ASystem;
                    //Our object should either be a Weapon or a System, so check for both cases
                    if (system) system = Instantiate(system);
                    else continue;
                    system._direction = _vehicleData.VehicleMatrix.XArray[x].YStuff[y].SystemDirection;
                    system.SpawnInCorrectDirection();
                    if (system.TryGetComponent(out AWeapon wep))
                    {
                        wep.transform.parent = RoomPosMatrix[x, y].ParentRoom.transform;
                        wep.transform.localPosition = Vector3.zero;
                        PositionSystemInRoom(wep, wep.transform.parent.GetComponent<Room>());
                        wep.RoomPosForInteraction = RoomPosMatrix[x, y].ParentRoom.allRoomPositions[0];
                        twep.AWeaponArray.Add(wep);

                        //Set the reference to the rooms
                        RoomPosMatrix[x, y].ParentRoom._roomSystem = wep;
                    }
                    else
                    {
                        system.transform.parent = RoomPosMatrix[x, y].ParentRoom.transform;
                        system.transform.localPosition = Vector3.zero;
                        PositionSystemInRoom(system.GetComponent<ASystem>(), system.transform.parent.GetComponent<Room>());
                        twep.ASystemArray.Add(system);
                        system.RoomPosForInteraction = RoomPosMatrix[x, y].ParentRoom.allRoomPositions[0];

                        //Set the reference to the rooms
                        RoomPosMatrix[x, y].ParentRoom._roomSystem = system;
                    }
                }
            }
        }
    }
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
        //RoomsParent.transform.parent = RoofParent.transform.parent = FloorTilemap.transform.parent = TankGeometryParent.transform;
        RoomsParent.transform.parent = TankGeometryParent.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        TankGeometryParent.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * _vehicleData._savedXSize, 0.25f * _vehicleData._savedYSize, 0);

        //print("finished creating Tank Geometry");
    }
    public void ShowRoof(bool b)
    {
        foreach(Room r in AllRooms)
        {
            r.ShowRoof(b);
        }
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
    public Vector2Int FindRoomPositionWithSpace()
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
                    return new Vector2Int(tmpRoom.freeRoomPositions[j]._xPos, tmpRoom.freeRoomPositions[j]._yPos);
                }
            }
        }
        //if it has has found no free rooms, return
        print("no free rooms found");
        return new Vector2Int(-1, -1);
    }
    public void VisualizeMatrix()
    {
        string matrix = "";
        for (int y = 0; y < _vehicleData._savedYSize; y++)
        {
            matrix += "Y:" + y.ToString() + ": ";
            for (int x = 0; x < _vehicleData._savedXSize; x++)
            {
                if (RoomPosMatrix[x, y]) matrix += "(" + RoomPosMatrix[x, y].name + ") ";
                else matrix += "__NONE__, ";
            }
            matrix += "\n";
        }
        print(matrix);
    }
}