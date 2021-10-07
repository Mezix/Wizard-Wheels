using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public ISystem roomSystem;
    public int sizeX;
    public int sizeY;
    public List<Transform> allRoomPositions = new List<Transform>();
    public List<Transform> freeRoomPositions = new List<Transform>();

    private void Awake()
    {
        freeRoomPositions = allRoomPositions;
    }
    public void TakeUpRoom(Transform t)
    {
        if(allRoomPositions.Contains(t))
        {
            if (freeRoomPositions.Contains(t))
            {
                freeRoomPositions.Remove(t);
            }
        }
    }
    public void FreeUpRoom(Transform t)
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
