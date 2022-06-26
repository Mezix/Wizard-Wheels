using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPosition : MonoBehaviour
{
    public Room ParentRoom;
    public int roomPosIndex; //index of roomPos in room

    //  Spawned Objects so we can delete at runtime
    public GameObject _spawnedTopWall = null;
    public GameObject _spawnedRightWall = null;
    public GameObject _spawnedBottomWall = null;
    public GameObject _spawnedLeftWall = null;
    public GameObject _spawnedTire = null;
    public GameObject _spawnedSystem = null;

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
