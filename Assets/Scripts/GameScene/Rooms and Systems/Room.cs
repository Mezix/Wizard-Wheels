using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public ISystem roomSystem;
    public int sizeX;
    public int sizeY;
    public List<RoomPosition> allRoomPositions;
    public List<RoomPosition> freeRoomPositions;

    private void Awake()
    {
        InitRoomPositions();
    }

    private void InitRoomPositions()
    {
        foreach (RoomPosition r in allRoomPositions) r.ParentRoom = this;
        // copy all available room positions to our freeRoomPositions
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