using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathfinding : MonoBehaviour
{
    private void Awake()
    {
        Ref.Path = this;
    }
    public List<RoomPosition> FindPath(RoomPosition startRoomPos, RoomPosition targetRoomPos, TankGeometry tank) //for clarification watch Sebastian Lagues Video on A* Pathfinding (Part 1 & 3)
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
    public List<RoomPosition> GetNeighbours(RoomPosition roomPosToCheck, TankGeometry tank)
    {
        List<RoomPosition> neighbouredRooms = new List<RoomPosition>();

        //  Neighbours in X Direction
        for (int x = -1; x <= 1; x++)
        {
            if (x != 0)
            {
                int checkX = roomPosToCheck._xPos + x;
                if (checkX >= 0 && checkX < tank._tankRoomConstellation._savedXSize)
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
                if (checkY >= 0 && checkY < tank._tankRoomConstellation._savedYSize)
                {
                    if (tank.RoomPosMatrix[roomPosToCheck._xPos, checkY] != null)
                    {
                        neighbouredRooms.Add(tank.RoomPosMatrix[roomPosToCheck._xPos, checkY]);
                    }
                }
            }
        }
        return neighbouredRooms;
    }
    public void SetPathToRoomWithMouse(AUnit unit)
    {
        Room CurrentRoom = unit.CurrentRoom;
        RoomPosition CurrentRoomPos = unit.CurrentRoomPos;

        //  Attempt to find a valid room, if we dont find one, deselect units instead
        RaycastHit2D hit = HM.RaycastToMouseCursor(LayerMask.GetMask("Room"));
        if (!hit.collider || !hit.collider.transform.TryGetComponent(out Room roomToGetTo) || roomToGetTo.GetNextFreeRoomPos() == null)
        {
            Ref.mouse.DeselectAllUnits();
            print("no valid room found, deselecting unit and aborting pathfinding");
            return;
        }
        if (!CurrentRoom.tGeo.Equals(roomToGetTo.tGeo))
        {
            Ref.mouse.DeselectAllUnits();
            print("Trying to get to a different Tank than the one we are in, Returning and deselecting Unit!");
            return;
        }

        //  Check if the room has a system
        RoomPosition roomPosToGetTo;
        bool tryingToGetToInteractionPos = false;
        if (roomToGetTo.roomSystem != null)
        {
            //check if the rooms designated room for interaction is free
            if (roomToGetTo.freeRoomPositions[roomToGetTo.roomSystem.RoomPosForInteraction.roomPosIndex])
            {
                roomPosToGetTo = roomToGetTo.roomSystem.RoomPosForInteraction;
                tryingToGetToInteractionPos = true;
            }
            else roomPosToGetTo = roomToGetTo.GetNextFreeRoomPos();
        }
        else
        {
            roomPosToGetTo = roomToGetTo.GetNextFreeRoomPos();
        }

        if (roomToGetTo.Equals(CurrentRoom))
        {
            if(!tryingToGetToInteractionPos)
            {
                Ref.mouse.DeselectAllUnits();
                print("Trying to go to the same position we are already in, Deselecting unit!");
                return;
            }
        }

        
        List<RoomPosition> path = FindPath(CurrentRoomPos, roomPosToGetTo, unit.transform.parent.GetComponentInChildren<TankGeometry>());

        //  Check if the path is valid!
        if (path.Count == 0)
        {
            Ref.mouse.DeselectAllUnits();
            print("Path is empty, aborting!");
            return;
        }

        //if we were already moving somewhere, free up the space we were last moving to and find our current room
        if (unit.PathToRoom.Count > 0)
        {
            //print("Diverting path!");
            unit.DesiredRoom.FreeUpRoomPos(unit.DesiredRoomPos, unit);
        }

        //  free up the room we are currently in so someone else can go there
        if (CurrentRoom && CurrentRoomPos) CurrentRoom.FreeUpRoomPos(CurrentRoomPos, unit);

        //  reserve the spot we are going to for ourselves
        unit.DesiredRoom = roomToGetTo;
        unit.DesiredRoomPos = unit.DesiredRoom.GetNextFreeRoomPos();
        roomToGetTo.OccupyRoomPos(unit.DesiredRoomPos, unit);

        //Assign the valid path
        unit.ClearUnitPath();
        unit.CurrentWaypoint = 0;
        unit.PathToRoom = path;
        //PrintPath(unit.PathToRoom);


        // stop interacting with the system in our room if we have one
        if (CurrentRoom.roomSystem != null) unit.StopInteraction();

        //start the movement
        unit.UnitIsMoving = true;
        unit.UnitSelected = false;

        //Set the indicator of the unit
        unit.SetNextPosIndicator(unit.DesiredRoomPos);
    }
    public void SetPathToRoom(AUnit unit, RoomPosition roomPosToGetTo)
    {
        Room CurrentRoom = unit.CurrentRoom;
        RoomPosition CurrentRoomPos = unit.CurrentRoomPos;
        Room roomToGetTo = roomPosToGetTo.ParentRoom;

        if (!unit || !roomPosToGetTo || !CurrentRoom || !CurrentRoomPos || !roomToGetTo)
        {
            print("Some Room is invalid");
            return;
        }
        if (!CurrentRoom.tr.Equals(roomToGetTo.tr))
        {
            Ref.mouse.DeselectAllUnits();
            print("Trying to get to a different Tank than the one we are in, Returning and deselecting Unit!");
            return;
        }
        if (roomPosToGetTo.Equals(CurrentRoomPos))
        {
            Ref.mouse.DeselectAllUnits();
            print("Trying to go to the same roomPos we are already in, Deselecting unit!");
            return;
        }

        List<RoomPosition> path = FindPath(CurrentRoomPos, roomPosToGetTo, unit.transform.parent.GetComponentInChildren<TankGeometry>());

        //  Check if the path is valid!
        if (path.Count == 0)
        {
            Ref.mouse.DeselectAllUnits();
            print("Path not possible, aborting!");
            return;
        }

        //if we were already moving somewhere, free up the space we were last moving to and find our current room
        if (unit.PathToRoom.Count > 0)
        {
            print("Diverting path!");
            unit.DesiredRoom.FreeUpRoomPos(unit.DesiredRoomPos, unit);
        }

        //  free up the room we are currently in so someone else can go there
        if (CurrentRoom && CurrentRoomPos) CurrentRoom.FreeUpRoomPos(CurrentRoomPos, unit);

        //  reserve the spot we are going to for ourselves
        unit.DesiredRoom = roomToGetTo;
        unit.DesiredRoomPos = unit.DesiredRoom.GetNextFreeRoomPos();
        roomToGetTo.OccupyRoomPos(unit.DesiredRoomPos, unit);

        //Assign the valid path
        unit.ClearUnitPath();
        unit.CurrentWaypoint = 0;
        unit.PathToRoom = path;
        //PrintPath(unit.PathToRoom);


        // stop interacting with the system in our room if we have one
        if (CurrentRoom.roomSystem != null) unit.StopInteraction();

        //start the movement
        unit.UnitIsMoving = true;
        unit.UnitSelected = false;
    }
    public void PrintPath(List<RoomPosition> path)
    {
        string p = "";
        foreach (RoomPosition r in path) p += r.name + ", ";
        //print(p);
        print(path.Count);
    }
}