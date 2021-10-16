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
    /// <param name="startRoom"></param>
    /// <param name="targetRoom"></param>
    /// <param name="tank"></param>
    /// <returns></returns>
    public List<Room> FindPath(Room startRoom, Room targetRoom, TankRoomConstellation tank) //for clarification watch Sebastian Lagues Video on A* Pathfinding (Part 1 & 3)
    {
        List<Room> Path = new List<Room>();

        List<Room> OpenSet = new List<Room>(); //The List of all Tiles, we could check in the Future
        HashSet<Room> ClosedSet = new HashSet<Room>(); // The Set of all Tiles we already Checked.

        OpenSet.Add(startRoom);

        while (OpenSet.Count > 0) //This Loop is running, as long as there are still Tiles we could Check.
        {
            Room currentTile = OpenSet[0];
            for (int i = 1; i < OpenSet.Count; i++) //Determines the best Tile for our Path, from all the Neighbors of our Tiles we already checked.
            {
                if (OpenSet[i].FCost < currentTile.FCost || OpenSet[i].FCost == currentTile.FCost && OpenSet[i]._hCost < currentTile._hCost)
                {
                    currentTile = OpenSet[i];
                }
            }

            OpenSet.Remove(currentTile);
            ClosedSet.Add(currentTile);

            if (currentTile == targetRoom) //Breaks the Loop if we found our Target.
            {
                Path = RetracePath(startRoom, targetRoom);
                break;
            }

            foreach (Room neighbour in GetNeighbours(currentTile, tank)) //Adds new Tiles to our Openset List, based on the Neighbors of our already checked Tiles.
            {
                if (ClosedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentTile._gCost + GetDistance(currentTile, neighbour);
                if (newMovementCostToNeighbour < neighbour._gCost || !OpenSet.Contains(neighbour)) //Determines the Costs of the Tiles we add to our Openset List.
                {
                    neighbour._gCost = newMovementCostToNeighbour;
                    neighbour._hCost = GetDistance(neighbour, targetRoom);
                    neighbour._pathfindParent = currentTile;

                    if (!OpenSet.Contains(neighbour))
                    {
                        OpenSet.Add(neighbour);
                    }
                }
            }
        }
        return Path;
    }
    List<Room> RetracePath(Room startRoom, Room targetRoom) //Highlights the Path between two Tiles, and adds all Tiles of that Path to a List.
    {
        List<Room> path = new List<Room>();
        Room currentRoom = targetRoom;

        while (currentRoom != startRoom)
        {
            path.Add(currentRoom);
            currentRoom = currentRoom._pathfindParent;
        }

        path.Reverse();
        return path;
    }
    List<Room> ReturnPath(Room startRoom, Room targetRoom) //same as retrace path except it returns it instead of overwriting the one in the grid
    {
        List<Room> path = new List<Room>();
        Room currentTile = targetRoom;

        while (currentTile != startRoom)
        {
            path.Add(currentTile);
            currentTile = currentTile._pathfindParent;
        }

        path.Reverse();
        return path;
    }
    public int GetDistance(Room roomA, Room roomB) //Returns the Distance between two Tiles.
    {
        int dstx = Mathf.FloorToInt(Mathf.Abs(roomA._xPos - roomB._xPos));
        int dsty = Mathf.FloorToInt(Mathf.Abs(roomA._yPos - roomB._yPos));

        if (dstx > dsty)
        {
            return 20 * dsty + 10 * (dstx - dsty);
        }
        return 20 * dstx + 10 * (dsty - dstx);
    }
    public List<Room> GetNeighbours(Room roomToCheck, TankRoomConstellation tank)
    {
        List<Room> neighbours = new List<Room>();

        //  Neighbours in X Direction
        for (int x = -1; x <= 1; x++)
        {
            if (x != 0)
            {
                int checkX = roomToCheck._xPos + x;
                if (checkX >= 0 && checkX < tank.XTilesAmount)
                {
                    if (tank.AllObjectsInRoom[checkX, roomToCheck._yPos])
                    {
                        neighbours.Add(tank.AllObjectsInRoom[checkX, roomToCheck._yPos].GetComponent<Room>());
                    }
                }
            }
        }

        //  Neighbours in Y Direction
        for (int y = -1; y <= 1; y++)
        {
            if (y != 0)
            {
                int checkY = roomToCheck._yPos + y;
                if (checkY >= 0 && checkY < tank.YTilesAmount)
                {
                    if (tank.AllObjectsInRoom[roomToCheck._xPos, checkY])
                    {
                        neighbours.Add(tank.AllObjectsInRoom[roomToCheck._xPos, checkY].GetComponent<Room>());
                    }
                }
            }
        }
        return neighbours;
    }
}