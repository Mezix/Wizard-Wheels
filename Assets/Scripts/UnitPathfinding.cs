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
    //  Change FindPath to use wizards position instead of
    public List<RoomPosition> FindPath(RoomPosition startRoom, RoomPosition targetRoom, TankRoomConstellation tank) //for clarification watch Sebastian Lagues Video on A* Pathfinding (Part 1 & 3)
    {
        List<RoomPosition> Path = new List<RoomPosition>();

        List<RoomPosition> OpenSet = new List<RoomPosition>(); //The List of all Tiles, we could check in the Future
        HashSet<RoomPosition> ClosedSet = new HashSet<RoomPosition>(); // The Set of all Tiles we already Checked.

        OpenSet.Add(startPos);

        while (OpenSet.Count > 0) //This Loop is running, as long as there are still Tiles we could Check.
        {
            RoomPosition currentRoomPosition = OpenSet[0];
            for (int i = 1; i < OpenSet.Count; i++) //Determines the best RoomPos for our Path, from all the Neighbors of our RoomPos we already checked.
            {
                if (OpenSet[i].FCost < currentRoomPosition.FCost || OpenSet[i].FCost == currentRoomPosition.FCost && OpenSet[i]._hCost < currentRoomPosition._hCost)
                {
                    currentRoomPosition = OpenSet[i];
                }
            }

            OpenSet.Remove(currentRoomPosition);
            ClosedSet.Add(currentRoomPosition);

<<<<<<< HEAD
            if (currentRoomPosition == targetRoom) //Breaks the Loop if we found our Target.
=======
            if (currentTile == targetPos) //Breaks the Loop if we found our Target.
>>>>>>> parent of 25861bf (added a slightly buggy version of pathfinding for wizards)
            {
                Path = RetracePath(startPos, targetPos);
                break;
            }

            foreach (RoomPosition neighbour in GetNeighbouringRoomPositions(currentRoomPosition, tank)) //Adds new Tiles to our Openset List, based on the Neighbors of our already checked Tiles.
            {
                if (ClosedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentRoomPosition._gCost + GetDistance(currentRoomPosition, neighbour);
                if (newMovementCostToNeighbour < neighbour._gCost || !OpenSet.Contains(neighbour)) //Determines the Costs of the Tiles we add to our Openset List.
                {
                    neighbour._gCost = newMovementCostToNeighbour;
<<<<<<< HEAD
                    neighbour._hCost = GetDistance(neighbour, targetRoom);
                    neighbour._parent = currentRoomPosition;
=======
                    neighbour._hCost = GetDistance(neighbour, targetPos);
                    neighbour._parent = currentTile;
>>>>>>> parent of 25861bf (added a slightly buggy version of pathfinding for wizards)

                    if (!OpenSet.Contains(neighbour))
                    {
                        OpenSet.Add(neighbour);
                    }
                }
            }
        }
        return Path;
    }
    List<RoomPosition> RetracePath(RoomPosition startRoom, RoomPosition targetRoom) //Highlights the Path between two Tiles, and adds all Tiles of that Path to a List.
    {
        List<RoomPosition> path = new List<RoomPosition>();
        RoomPosition currentRoom = targetRoom;

        while (currentRoom != startRoom)
        {
            path.Add(currentRoom);
            currentRoom = currentRoom._parent;
        }

        path.Reverse();
        return path;
    }
    List<RoomPosition> ReturnPath(RoomPosition startRoom, RoomPosition targetRoom) //same as retrace path except it returns it instead of overwriting the one in the grid
    {
        List<RoomPosition> path = new List<RoomPosition>();
        RoomPosition currentTile = targetRoom;

        while (currentTile != startRoom)
        {
            path.Add(currentTile);
            currentTile = currentTile._parent;
        }

        path.Reverse();
        return path;
    }
<<<<<<< HEAD
    public int GetDistance(RoomPosition roomA, RoomPosition roomB) //Returns the Distance between two Tiles.
=======
    public int GetDistance(Room tileA, Room tileB) //Returns the Distance between two Tiles.
>>>>>>> parent of 25861bf (added a slightly buggy version of pathfinding for wizards)
    {
        int dstx = Mathf.FloorToInt(Mathf.Abs(tileA._xPos - tileB._xPos));
        int dsty = Mathf.FloorToInt(Mathf.Abs(tileA._yPos - tileB._yPos));

        if (dstx > dsty)
        {
            return 20 * dsty + 10 * (dstx - dsty);
        }
        return 20 * dstx + 10 * (dsty - dstx);
    }
<<<<<<< HEAD
    public List<RoomPosition> GetNeighbouringRoomPositions(RoomPosition roomToCheck, TankRoomConstellation tank)
=======
    public List<Room> GetNeighbours(Room room, TankRoomConstellation tank)
>>>>>>> parent of 25861bf (added a slightly buggy version of pathfinding for wizards)
    {
        List<RoomPosition> neighbours = new List<RoomPosition>();

        //TODO: ecken aus nachbargruppe ausschließen, damit wir nicht schräg laufen können

        for (int x = -1; x <= 1; x++)
        {
            if (x != 0)
            {
                int checkX = room._xPos + x;
                if (checkX >= 0 && checkX < tank.XTilesAmount)
                {
<<<<<<< HEAD
                    if (tank.AllRoomPositions[checkX, roomToCheck._yPos])
                    {
                        neighbours.Add(tank.AllRoomPositions[checkX, roomToCheck._yPos].GetComponent<RoomPosition>());
                    }
=======
                    if (!tank.AllObjectsInRoom[checkX, room._yPos]) continue;
                    neighbours.Add(tank.AllObjectsInRoom[checkX, room._yPos].GetComponent<Room>());
>>>>>>> parent of 25861bf (added a slightly buggy version of pathfinding for wizards)
                }
            }
        }

        for (int y = -1; y <= 1; y++)
        {
            if (y != 0)
            {
                int checkY = room._yPos + y;
                if (checkY >= 0 && checkY < tank.YTilesAmount)
                {
<<<<<<< HEAD
                    if (tank.AllRoomPositions[roomToCheck._xPos, checkY])
                    {
                        neighbours.Add(tank.AllRoomPositions[roomToCheck._xPos, checkY].GetComponent<RoomPosition>());
                    }
=======
                    if (!tank.AllObjectsInRoom[room._xPos, checkY]) continue;
                    neighbours.Add(tank.AllObjectsInRoom[room._xPos, checkY].GetComponent<Room>());
>>>>>>> parent of 25861bf (added a slightly buggy version of pathfinding for wizards)
                }
            }
        }
        return neighbours;
    }
}
