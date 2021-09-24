using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTankController : MonoBehaviour
{
    public static PlayerTankController instance;

    public TankMovement tMov;
    public TankRotation tRot;
    public TankWeapons tWep;
    public List<TechWizard> wizardList = new List<TechWizard>();

    public TankRoomConstellation tankRoomConstellation;
    public GameObject parent;
    public Tilemap BGTilemap;
    public Tile bgTile;
    Vector3 offset;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        offset = new Vector3(-0.25f, 0.25f, 0);
        InitTank();
        InitWizards();
        CreateTankFromRoomConstellation();
        CreateBGTilemap();
    }

    private void CreateBGTilemap()
    {
        for (int x = 0; x < tankRoomConstellation.XTilesAmount; x++)
        {
            for (int y = 0; y < tankRoomConstellation.YTilesAmount; y++)
            {
                if (tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y])
                {
                    int sizeX = tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y].GetComponent<Room>().sizeX;
                    int sizeY = tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y].GetComponent<Room>().sizeY;
                    CreateBGRoom(x, y, sizeX, sizeY);
                }
            }
        }
        BGTilemap.transform.localPosition = parent.transform.localPosition;
    }

    private void CreateBGRoom(int startX, int startY, int sizeX, int sizeY)
    {
        for(int x = startX; x < startX + sizeX; x++)
        {
            for(int y = startY; y < startY + sizeY; y++)
            {
                BGTilemap.SetTile(new Vector3Int(x, -(y+1), 0), bgTile);
            }
        }
    }

    private void CreateTankFromRoomConstellation()
    {
        for (int x = 0; x < tankRoomConstellation.XTilesAmount; x++)
        {
            for (int y = 0; y < tankRoomConstellation.YTilesAmount; y++)
            {
                if(tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y])
                {
                    GameObject go = Instantiate(tankRoomConstellation.SavedPrefabRefMatrix.XArray[x].YRooms[y]);
                    go.transform.parent = parent.transform;
                    go.transform.localPosition = new Vector2(x * 0.5f, y * -0.5f);
                }
            }
        }
        parent.transform.localPosition += new Vector3(-0.25f * tankRoomConstellation.XTilesAmount, 0.25f * tankRoomConstellation.YTilesAmount, 0)
                                       + offset;
    }

    private void InitWizards()
    {
        foreach (TechWizard w in GetComponentsInChildren<TechWizard>())
        {
            wizardList.Add(w);
        }
    }

    private void InitTank()
    {
        tMov = GetComponentInChildren<TankMovement>();
        tRot = GetComponentInChildren<TankRotation>();
        tWep = GetComponentInChildren<TankWeapons>();
    }
}
