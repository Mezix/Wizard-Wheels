using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static PlayerData;

public class TankGeometry : MonoBehaviour
{
    public VehicleGeometry _vehicleData;
    public RoomPosition[,] RoomPosMatrix;
    public GameObject _tankGeometryParent;
    public GameObject _roomsParent;
    public List<Room> _allRooms;

    [SerializeField]
    private List<SpriteRenderer> systemIcons = new List<SpriteRenderer>();
    public Color FloorColor;
    public Color RoofColor;
    public void CreateTankGeometry()
    {
        LoadRooms();
        PositionTankObjects();
        InitWeaponsAndSystems();
        CreateSystemIcons();
        CreateWalls();

        if (REF.CombatUI) REF.CombatUI.TurnOnXRay(REF.CombatUI._xrayOn);
    }
    private void LoadRooms()
    {
        RoomPosMatrix = new RoomPosition[_vehicleData.SavedXSize, _vehicleData.SavedYSize];
        _allRooms = new List<Room>();

        _roomsParent = new GameObject("All Tank Rooms");
        _roomsParent.transform.parent = gameObject.transform;
        _roomsParent.transform.localPosition = Vector3.zero;

        for (int y = 0; y < _vehicleData.SavedYSize; y++)
        {
            for (int x = 0; x < _vehicleData.SavedXSize; x++)
            {
                if (_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].Equals(new RoomInfo()) 
                    || _vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath == null
                    || _vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath == "") continue;
                Room room = Instantiate(Resources.Load(_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].RoomPrefabPath, typeof(Room))) as Room;
                room._tGeo = this;
                room.ID = _vehicleData.GetHashCode();
                room.gameObject.transform.parent = _roomsParent.transform;
                room.gameObject.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
                _allRooms.Add(room);

                room._floorType = _vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].FloorType;
                room._roofType = _vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].RoofType;
                room.InitHP(_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].RoomCurrentHP, _vehicleData.VehicleRoomMaxHP);

                if(GetComponent<PlayerTankController>())
                {
                    BoxCollider2D bc2d = room.GetComponent<BoxCollider2D>();

                    BoxCollider2D usedByCompositeBoxCollider = room.gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                    usedByCompositeBoxCollider.size = bc2d.size;
                    usedByCompositeBoxCollider.offset = bc2d.offset;
                    usedByCompositeBoxCollider.usedByComposite = true;
                }

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
        for (int y = 0; y < _vehicleData.SavedYSize; y++)
        {
            for (int x = 0; x < _vehicleData.SavedXSize; x++)
            {
                if (_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath != "")
                {
                    ASystem system = Resources.Load(_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath, typeof(ASystem)) as ASystem;
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
        for (int y = 0; y < _vehicleData.SavedYSize; y++)
        {
            for (int x = 0; x < _vehicleData.SavedXSize; x++)
            {
                if (_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y]._topWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallUp"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y]._rightWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallRight"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y]._bottomWallExists)
                {
                    GameObject wall = Instantiate(Resources.Load(GS.WallPrefabs("WallDown"), typeof(GameObject)) as GameObject);
                    wall.transform.SetParent(RoomPosMatrix[x, y].transform);
                    wall.transform.localPosition = Vector3.zero;
                }
                if (_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y]._leftWallExists)
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
        for (int y = 0; y < _vehicleData.SavedYSize; y++)
        {
            for (int x = 0; x < _vehicleData.SavedXSize; x++)
            {
                if (!RoomPosMatrix[x, y]) continue;
                if (_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath != "")
                {
                    ASystem system = Resources.Load(_vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].SystemPrefabPath, typeof(ASystem)) as ASystem;
                    //Our object should either be a Weapon or a System, so check for both cases
                    if (system) system = Instantiate(system);
                    else continue;
                    system._direction = _vehicleData.VehicleMatrix.Columns[x].ColumnContent[y].SystemDirection;
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
        _tankGeometryParent = new GameObject("Tank Geometry Parent");
        _tankGeometryParent.transform.parent = gameObject.transform;
        _tankGeometryParent.transform.localPosition = Vector3.zero;

        // parent all spawnedObjects to this parent
        //RoomsParent.transform.parent = RoofParent.transform.parent = FloorTilemap.transform.parent = TankGeometryParent.transform;
        _roomsParent.transform.parent = _tankGeometryParent.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        _tankGeometryParent.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        _tankGeometryParent.transform.localPosition += new Vector3(-0.25f * _vehicleData.SavedXSize, 0.25f * _vehicleData.SavedYSize, 0);

        //print("finished creating Tank Geometry");
    }
    public void ShowRoof(bool b)
    {
        foreach(Room r in _allRooms)
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
        List<Room> allRoomsTMP = _allRooms;
        // searches through all possible rooms until it finds one it can occupy
        for (int i = 0; i < _allRooms.Count; i++)
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
        for (int y = 0; y < _vehicleData.SavedYSize; y++)
        {
            matrix += "Y:" + y.ToString() + ": ";
            for (int x = 0; x < _vehicleData.SavedXSize; x++)
            {
                if (RoomPosMatrix[x, y]) matrix += "(" + RoomPosMatrix[x, y].name + ") ";
                else matrix += "__NONE__, ";
            }
            matrix += "\n";
        }
        print(matrix);
    }
}