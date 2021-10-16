using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathfinding : MonoBehaviour
{
    public static UnitPathfinding instance;
    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// trying to merge findpath
    /// </summary>
    /// <param name="startRoomPos"></param>
    /// <param name="targetRoomPos"></param>
    /// <param name="tank"></param>
    /// <returns></returns>
    public List<RoomPosition> FindPath(RoomPosition startRoomPos, RoomPosition targetRoomPos, TankRoomConstellation tank) //for clarification watch Sebastian Lagues Video on A* Pathfinding (Part 1 & 3)
    {
        List<RoomPosition> Path = new List<RoomPosition>();

        List<RoomPosition> OpenSet = new List<RoomPosition>(); //The List of all Tiles, we could check in the Future
        HashSet<RoomPosition> ClosedSet = new HashSet<RoomPosition>(); // The Set of all Tiles we already Checked.

        OpenSet.Add(startRoomPos);

        while (OpenSet.Count > 0) //This Loop is running, as long as there are still Tiles we could Check.
        {
            RoomPosition currentRoomPos = OpenSet[0];
            for (int i = 1; i < OpenSet.Count; i++) //Determines the best Tile for our Path, from all the Neighbors of our Tiles we already checked.
            {
                if (OpenSet[i].FCost < currentRoomPos.FCost || OpenSet[i].FCost == currentRoomPos.FCost && OpenSet[i]._hCost < currentRoomPos._hCost)
                {
                    currentRoomPos = OpenSet[i];
                }
            }

            OpenSet.Remove(currentRoomPos);
            ClosedSet.Add(currentRoomPos);

            if (currentRoomPos == targetRoomPos) //Breaks the Loop if we found our Target.
            {
                Path = RetracePath(startRoomPos, targetRoomPos);
                break;
            }

            foreach (RoomPosition neighbour in GetNeighbours(currentRoomPos, tank)) //Adds new Tiles to our Openset List, based on the Neighbors of our already checked Tiles.
            {
                if (ClosedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentRoomPos._gCost + GetDistance(currentRoomPos, neighbour);
                if (newMovementCostToNeighbour < neighbour._gCost || !OpenSet.Contains(neighbour)) //Determines the Costs of the Tiles we add to our Openset List.
                {
                    neighbour._gCost = newMovementCostToNeighbour;
                    neighbour._hCost = GetDistance(neighbour, targetRoomPos);
                    neighbour._pathfindParent = currentRoomPos;

                    if (!OpenSet.Contains(neighbour))
                    {
                        OpenSet.Add(neighbour);
                    }
                }
            }
        }
        return Path;
    }
    List<RoomPosition> RetracePath(RoomPosition startingRoomPos, RoomPosition targetRoomPos) //Highlights the Path between two Tiles, and adds all Tiles of that Path to a List.
    {
        List<RoomPosition> path = new List<RoomPosition>();
        RoomPosition currentRoom = targetRoomPos;

        while (currentRoom != startingRoomPos)
        {
            path.Add(currentRoom);
            currentRoom = currentRoom._pathfindParent;
        }

        path.Reverse();
        return path;
    }
    List<RoomPosition> ReturnPath(RoomPosition startRoomPos, RoomPosition targetRoomPos) //same as retrace path except it returns it instead of overwriting the one in the grid
    {
        List<RoomPosition> Path = new List<RoomPosition>();
        RoomPosition currentRoomPos = targetRoomPos;

        while (currentRoomPos != startRoomPos)
        {
            Path.Add(currentRoomPos);
            currentRoomPos = currentRoomPos._pathfindParent;
        }

        Path.Reverse();
        return Path;
    }
    public int GetDistance(RoomPosition roomPosA, RoomPosition roomPosB) //Returns the Distance between two Tiles.
    {
        int dstx = Mathf.FloorToInt(Mathf.Abs(roomPosA._xPos - roomPosB._xPos));
        int dsty = Mathf.FloorToInt(Mathf.Abs(roomPosA._yPos - roomPosB._yPos));

        if (dstx > dsty)
        {
            return 20 * dsty + 10 * (dstx - dsty);
        }
        return 20 * dstx + 10 * (dsty - dstx);
    }
    public List<RoomPosition> GetNeighbours(RoomPosition roomPosToCheck, TankRoomConstellation tank)
    {
        List<RoomPosition> neighbouredRooms = new List<RoomPosition>();

        //  Neighbours in X Direction
        for (int x = -1; x <= 1; x++)
        {
            if (x != 0)
            {
                int checkX = roomPosToCheck._xPos + x;
                if (checkX >= 0 && checkX < tank.XTilesAmount)
                {
                    if (tank.RoomPosMatrix[checkX, roomPosToCheck._yPos])
                    {
                        neighbouredRooms.Add(tank.RoomPosMatrix[checkX, roomPosToCheck._yPos]);
                    }
                }
            }
        }

        //  Neighbours in Y Direction
        for (int y = -1; y <= 1; y++)
        {
            if (y != 0)
            {
                int checkY = roomPosToCheck._yPos + y;
                print(checkY);
                if (checkY >= 0 && checkY < tank.YTilesAmount)
                {
                    if (tank.RoomPosMatrix[roomPosToCheck._xPos, checkY])
                    {
                        neighbouredRooms.Add(tank.RoomPosMatrix[roomPosToCheck._xPos, checkY]);
                    }
                }
            }
        }
        return neighbouredRooms;
    }
}