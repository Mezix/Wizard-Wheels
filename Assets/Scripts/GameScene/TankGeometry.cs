using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TankGeometry : MonoBehaviour
{
    public TankRoomConstellation _tankRoomConstellation;
    public GameObject TankGeometryParent { get; private set; }
    public GameObject RoomsParent { get; private set; }
    public List<Room> AllRooms { get; private set; }
    public Tilemap FloorTilemap { get; private set; }
    public void SpawnTank()
    {
        CreateTankFromRoomConstellation();
        CreateBG();
        PositionTankObjects();
    }
    private void CreateBG()
    {
        CreateFloorTilemap();
        for (int x = 0; x < _tankRoomConstellation.XTilesAmount; x++)
        {
            for (int y = 0; y < _tankRoomConstellation.YTilesAmount; y++)
            {
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y])
                {
                    int sizeX = _tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y].GetComponent<Room>().sizeX;
                    int sizeY = _tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y].GetComponent<Room>().sizeY;
                    CreateBGAtPos(x, y, sizeX, sizeY);
                }
            }
        }
    }
    private void CreateFloorTilemap()
    {
        GameObject floor = new GameObject("FloorTilemap");
        floor.transform.parent = gameObject.transform;
        floor.transform.localPosition = Vector3.zero;

        //  Create Grid
        Grid g = floor.AddComponent<Grid>();
        g.cellSize = new Vector3(0.5f, 0.5f, 0);

        //  Create Tilemap
        FloorTilemap = floor.AddComponent<Tilemap>();
        FloorTilemap.tileAnchor = new Vector3(0, 1, 0);

        //  Create Renderer
        TilemapRenderer r = floor.AddComponent<TilemapRenderer>();
        r.sortingLayerName = "Vehicles";
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
    private void CreateBGAtPos(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                FloorTilemap.SetTile(new Vector3Int(x, -(y + 1), 0), (Tile)Resources.Load("DefaultTile"));
            }
        }
    }
    private void CreateTankFromRoomConstellation()
    {
        _tankRoomConstellation.RoomPosMatrix = new RoomPosition[_tankRoomConstellation.XTilesAmount, _tankRoomConstellation.YTilesAmount];
        AllRooms = new List<Room>();

        RoomsParent = new GameObject("All Tank Rooms");
        RoomsParent.transform.parent = gameObject.transform;
        RoomsParent.transform.localPosition = Vector3.zero;

        for (int x = 0; x < _tankRoomConstellation.XTilesAmount; x++)
        {
            for (int y = 0; y < _tankRoomConstellation.YTilesAmount; y++)
            {
                if (_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y])
                {
                    GameObject rGO = Instantiate(_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y]);
                    Room r = rGO.GetComponent<Room>();
                    rGO.transform.parent = RoomsParent.transform;
                    rGO.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
                    AllRooms.Add(r);

                    // Set the Room Positions
                    for (int roomY = 0; roomY < r.sizeY; roomY++)
                    {
                        for (int roomX = 0; roomX < r.sizeX; roomX++)
                        {
                            //print(roomX + " " + roomY);
                            _tankRoomConstellation.RoomPosMatrix[x + roomX, y + roomY] = r.allRoomPositions[roomX + roomY];
                            _tankRoomConstellation.RoomPosMatrix[x + roomX, y + roomY]._xPos = x + _tankRoomConstellation.RoomPosMatrix[x + roomX, y + roomY]._xRel;
                            _tankRoomConstellation.RoomPosMatrix[x + roomX, y + roomY]._yPos = y + _tankRoomConstellation.RoomPosMatrix[x + roomX, y + roomY]._yRel;
                        }
                    }

                    //sets the corner of the room that doesnt get caught with the matrix
                    _tankRoomConstellation.RoomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1] = r.allRoomPositions[r.sizeX * r.sizeY-1];
                    _tankRoomConstellation.RoomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1]._xPos = x + r.sizeX - 1;
                    _tankRoomConstellation.RoomPosMatrix[x + r.sizeX - 1, y + r.sizeY - 1]._yPos = y + r.sizeY - 1;
                }
            }
        }
    }
    private void PositionTankObjects()
    {
        //create overarching object for all our spawned objects
        TankGeometryParent = new GameObject("Tank Geometry Parent");
        TankGeometryParent.transform.parent = gameObject.transform;
        TankGeometryParent.transform.localPosition = Vector3.zero;

        // parent all spawnedObjects to this parent
        RoomsParent.transform.parent = FloorTilemap.transform.parent = TankGeometryParent.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        TankGeometryParent.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        TankGeometryParent.transform.localPosition += new Vector3(-0.25f * _tankRoomConstellation.XTilesAmount, 0.25f * _tankRoomConstellation.YTilesAmount, 0);
    }
    public Room FindRandomRoomWithSpace()
    {
        List<Room> allRoomsTMP = AllRooms;
        // searches through all possible rooms until it finds one it can occupy
        for(int i = 0; i < AllRooms.Count; i++)
        {
            Room tmpRoom = allRoomsTMP[Random.Range(0, allRoomsTMP.Count - 1)];
            for(int j = 0; j < tmpRoom.freeRoomPositions.Length; j++)
            {
                if(tmpRoom.freeRoomPositions[j] != null)
                {
                    print("found a random free room");
                    return tmpRoom;
                }
            }
        }
        //if it has has found no free rooms, return
        print("no free rooms found");
        return null;
    }
}