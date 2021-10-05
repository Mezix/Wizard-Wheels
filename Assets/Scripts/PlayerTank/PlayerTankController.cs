using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTankController : MonoBehaviour
{
    public static PlayerTankController instance;

    //  Important scripts

    public TankStats _tStats;
    public TankHealth _tHealth;
    public PlayerTankMovement _tMov;
    public PlayerTankRotation _tRot;
    public PlayerTankWeapons _tWep;

    public string _tankName;
    public List<TechWizard> _wizardList = new List<TechWizard>();

    //  Tank geometry and Layout

    public TankRoomConstellation _tankRoomConstellation;
    private GameObject tankGeometryParent;
    private GameObject rooms;
    private Tilemap floorTilemap;
    public Tile _defaultBGTile;

    private void Awake()
    {
        instance = this;
        _tHealth = GetComponentInChildren<TankHealth>();
        _tMov = GetComponentInChildren<PlayerTankMovement>();
        _tRot = GetComponentInChildren<PlayerTankRotation>();
        _tWep = GetComponentInChildren<PlayerTankWeapons>();
    }
    void Start()
    {
        InitTankStats();
        InitWizards();
        CreateTankFromRoomConstellation();
        CreateBG();
        PositionTankObjects();
    }

    

    private void InitTankStats()
    {
        if(_tStats)
        {
            _tHealth._maxHealth = _tStats._tankHealth;
        }
        else
        {
            _tHealth._maxHealth = 10;
        }
        _tHealth.InitHealth();
    }
    private void InitWizards()
    {
        foreach (TechWizard w in GetComponentsInChildren<TechWizard>())
        {
            _wizardList.Add(w);
        }
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
                    CreateBGRoom(x, y, sizeX, sizeY);
                }
            }
        }
    }
    private void CreateFloorTilemap()
    {
        GameObject floor = new GameObject("FloorTilemap");
        floor.transform.parent = gameObject.transform;

        //  Create Grid
        Grid g = floor.AddComponent<Grid>();
        g.cellSize = new Vector3(0.5f, 0.5f, 0);

        //  Create Tilemap
        floorTilemap = floor.AddComponent<Tilemap>();
        floorTilemap.tileAnchor = new Vector3(0,1,0);

        //  Create Renderer
        TilemapRenderer r = floor.AddComponent<TilemapRenderer>();
        r.sortingLayerName = "Vehicles";
    }
    private void CreateWallsTilemap()
    {
        GameObject tmp = new GameObject("WallsTilemap");

        //Create the Grid
        Grid g = tmp.AddComponent<Grid>();
        g.cellSize = new Vector3(0.5f, 0.5f, 0);

        //  Create Tilemap
        floorTilemap = tmp.AddComponent<Tilemap>();
        floorTilemap.tileAnchor = new Vector3(0, 1, 0);

        //  Create Renderer
        TilemapRenderer r = tmp.AddComponent<TilemapRenderer>();
        r.sortingLayerName = "Vehicles";

        //  Create Collider
        TilemapCollider2D c = tmp.AddComponent<TilemapCollider2D>();
    }

    private void CreateBGRoom(int startX, int startY, int sizeX, int sizeY)
    {
        for(int x = startX; x < startX + sizeX; x++)
        {
            for(int y = startY; y < startY + sizeY; y++)
            {
                floorTilemap.SetTile(new Vector3Int(x, -(y+1), 0), _defaultBGTile);
            }
        }
    }
    private void CreateTankFromRoomConstellation()
    {
        rooms = new GameObject("Tank Rooms");
        rooms.transform.parent = gameObject.transform;
        List<GameObject> tmpList = new List<GameObject>();

        for (int x = 0; x < _tankRoomConstellation.XTilesAmount; x++)
        {
            for (int y = 0; y < _tankRoomConstellation.YTilesAmount; y++)
            {
                if(_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y])
                {
                    GameObject go = Instantiate(_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y]);
                    go.transform.parent = rooms.transform;
                    go.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);

                    tmpList.Add(go);
                }
            }
        }
    }
    private void PositionTankObjects()
    {
        //create overarching object for all our spawned objects
        tankGeometryParent = new GameObject("Tank Geometry Parent");
        tankGeometryParent.transform.parent = gameObject.transform;

        // parent all spawnedObjects to this parent
        rooms.transform.parent = floorTilemap.transform.parent = tankGeometryParent.transform;

        //  Rooms have their transform origin point at the center of their rooms, so add a rooms x length, and subtract a rooms y length
        tankGeometryParent.transform.localPosition += new Vector3(0.25f, -0.25f, 0);

        //  Now move to the halfway point
        tankGeometryParent.transform.localPosition += new Vector3(-0.25f * _tankRoomConstellation.XTilesAmount, 0.25f * _tankRoomConstellation.YTilesAmount, 0);
    }
}
