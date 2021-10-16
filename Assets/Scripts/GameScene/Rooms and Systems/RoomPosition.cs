using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPosition : MonoBehaviour
{
    public Room ParentRoom;

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
    public RoomPosition _pathfindParent; //important for the pathfinding process
    public int _xPos;
    public int _yPos;
    //relative coordinates of the rooms positions compared to the start of the room, because Im too stupid to figure out how to do it in a matrix
    public int _xRel;
    public int _yRel;
}
