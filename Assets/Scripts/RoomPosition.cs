using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPosition : MonoBehaviour
{
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
    public RoomPosition _parent; //important for the pathfinding process
    public int _xPos;
    public int _yPos;

    //used for changing the relative xpos and ypos in a room that is larger than 1x1
    public int xRel;
    public int yRel;
}
