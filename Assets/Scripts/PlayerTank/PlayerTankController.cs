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
    public GameObject _tankRooms;
    public Tilemap _floorTilemap;
    public Tile _defaultBGTile;
    Vector3 tileOffset;

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
        tileOffset = new Vector3(-0.25f, 0.25f, 0);
        InitTankStats();
        InitWizards();
        CreateTankFromRoomConstellation();
        CreateBG();
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
        _floorTilemap.transform.localPosition = _tankRooms.transform.localPosition;
    }
    private void CreateBGRoom(int startX, int startY, int sizeX, int sizeY)
    {
        for(int x = startX; x < startX + sizeX; x++)
        {
            for(int y = startY; y < startY + sizeY; y++)
            {
                _floorTilemap.SetTile(new Vector3Int(x, -(y+1), 0), _defaultBGTile);
            }
        }
    }
    private void CreateTankFromRoomConstellation()
    {
        for (int x = 0; x < _tankRoomConstellation.XTilesAmount; x++)
        {
            for (int y = 0; y < _tankRoomConstellation.YTilesAmount; y++)
            {
                if(_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y])
                {
                    GameObject go = Instantiate(_tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y]);
                    go.transform.parent = _tankRooms.transform;
                    go.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
                }
            }
        }
        _tankRooms.transform.localPosition += new Vector3(-0.25f * _tankRoomConstellation.XTilesAmount, 0.25f * _tankRoomConstellation.YTilesAmount, 0)
                                       + tileOffset;
    }
}
