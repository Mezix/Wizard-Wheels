using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaBeam : MonoBehaviour
{
    public List<Room> roomsToHit = new List<Room>();
    private void Start()
    {
        ClearRoomsToHit();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other)
        {
            if(other.GetComponentInChildren<Room>())
            {
                Room r = other.GetComponentInChildren<Room>();
                if(!r.tGeo.GetComponent<PlayerTankController>())
                {
                    if (!roomsToHit.Contains(r)) roomsToHit.Add(r);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other)
        {
            if (other.GetComponentInChildren<Room>())
            {
                Room r = other.GetComponentInChildren<Room>();
                if (roomsToHit.Contains(r)) roomsToHit.Remove(r);
            }
        }
    }
    public void ClearRoomsToHit()
    {
        roomsToHit = new List<Room>();
    }
}
