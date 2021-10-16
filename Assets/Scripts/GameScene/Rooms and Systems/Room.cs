﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public ISystem roomSystem;
    public int sizeX;
    public int sizeY;
    public List<RoomPosition> allRoomPositions;
    public List<RoomPosition> freeRoomPositions;

    //Pathfinding
    public int _gCost; // gCost represents the Distance from our Starting Point to our current Tile.
    public int _hCost; // hCost represents the Distance from our Target Point to our Current Tile.
    public int FCost
    {
        get
        {
            return _gCost + _hCost;
        }
    }
    public Room _pathfindParent; //important for the pathfinding process
    public int _xPos;
    public int _yPos;

    private void Awake()
    {
        HM.CopyListOfRoomPositions(allRoomPositions, freeRoomPositions);
    }
    public void OccupyRoomPos(RoomPosition t)
    {
        if (allRoomPositions.Contains(t))
        {
            if (freeRoomPositions.Contains(t))
            {
                freeRoomPositions.Remove(t);
            }
        }
    }
    public void FreeUpRoomPos(RoomPosition t)
    {
        if (allRoomPositions.Contains(t))
        {
            freeRoomPositions.Add(t);
        }
    }
}