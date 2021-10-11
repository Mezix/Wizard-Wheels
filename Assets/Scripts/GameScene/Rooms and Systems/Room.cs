using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public ISystem roomSystem;
    public int sizeX;
    public int sizeY;
    public List<RoomPosition> allRoomPositions = new List<RoomPosition>();
    public List<RoomPosition> freeRoomPositions = new List<RoomPosition>();
    public RoomPosition[,] RoomPosGrid;
    private void Awake()
    {
        freeRoomPositions = allRoomPositions;
        RoomPosGrid = new RoomPosition[sizeX, sizeY];

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                RoomPosGrid[x, y] = allRoomPositions[x + y];
            }
        }
        //Bottom Right Corner room must be added manually
        RoomPosGrid[sizeX-1, sizeY-1] = allRoomPositions[(sizeX * sizeY) - 1];
    }
    public void TakeUpRoom(RoomPosition t)
    {
        if(allRoomPositions.Contains(t))
        {
            if (freeRoomPositions.Contains(t))
            {
                freeRoomPositions.Remove(t);
            }
        }
    }
    public void FreeUpRoom(RoomPosition t)
    {
        if (allRoomPositions.Contains(t))
        {
            if (freeRoomPositions.Contains(t))
            {
                freeRoomPositions.Add(t);
            }
        }
    }
}
