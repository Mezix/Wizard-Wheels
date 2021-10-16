using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public ISystem roomSystem;
    public int sizeX;
    public int sizeY;
    
    public RoomPosition[] allRoomPositions;
    public RoomPosition[] freeRoomPositions;

    private void Awake()
    {
        InitRoomPositions();
    }

    private void InitRoomPositions()
    {
        freeRoomPositions = new RoomPosition[allRoomPositions.Length];
        for (int i = 0; i < allRoomPositions.Length; i++)
        {
            freeRoomPositions[i] = allRoomPositions[i];
            freeRoomPositions[i].roomPosIndex = i;
            freeRoomPositions[i].ParentRoom = this;
        }
        // copy all available room positions to our freeRoomPositions

        //HM.CopyListOfRoomPositions(allRoomPositions, freeRoomPositions);
    }

    public void OccupyRoomPos(RoomPosition rPos)
    {
        freeRoomPositions[rPos.roomPosIndex] = null;
        //if (allRoomPositions.Contains(t))
        //{
        //    if (freeRoomPositions.Contains(t))
        //    {
        //        freeRoomPositions.Remove(t);
        //    }
        //}
    }
    public void FreeUpRoomPos(RoomPosition rPos)
    {
        freeRoomPositions[rPos.roomPosIndex] = rPos;
        //if (allRoomPositions.Contains(t))
        //{
        //    freeRoomPositions.Add(t);
        //}
    }
    public RoomPosition GetNextRoomPos()
    {
        for(int i = 0; i < freeRoomPositions.Length; i++)
        {
            if (freeRoomPositions[i]) return freeRoomPositions[i];
            //try get the next free room
        }
        return null;
    }
}