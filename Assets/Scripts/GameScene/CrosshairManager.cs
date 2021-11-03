using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    private GameObject crosshairPrefab;
    private Dictionary<Room, Crosshair> RoomsCrosshairsDictionary;

    private void Awake()
    {
        Ref.c = this;
        crosshairPrefab = (GameObject)Resources.Load("Crosshair");
    }
    private void Start()
    {
        RoomsCrosshairsDictionary = new Dictionary<Room, Crosshair>();
    }
    public void AddCrosshair(Room room, IWeapon weapon)
    {
        if (RoomsCrosshairsDictionary.ContainsKey(room)) //  Check if weve spawned a crosshair for that specific room
        {
            //update the crosshair at that location
            RoomsCrosshairsDictionary[room].AddAttacker(weapon);
        }
        else //otherwise spawn a crosshair there
        {
            //Spawn Crosshair
            GameObject spawnedCrosshair = Instantiate((GameObject)Resources.Load("Crosshair"));
            Crosshair c = spawnedCrosshair.GetComponentInChildren<Crosshair>();
            spawnedCrosshair.transform.parent = room.transform;
            spawnedCrosshair.transform.localPosition = Vector3.zero + new Vector3(0, 0, 10);

            RoomsCrosshairsDictionary.Add(room, c);
            c.SetCrosshairSizeAndPosition(room.sizeX, room.sizeY);
            c.AddAttacker(weapon);
        }
    }

    public void RemoveCrosshair(IWeapon weapon)
    {
        if (weapon == null) return;
        if (!weapon.Room) return;
        //Remove the weapon from the crosshair
        Room r = weapon.Room.GetComponentInChildren<Room>();
        if (RoomsCrosshairsDictionary.ContainsKey(r))
        {
            bool destroyCrosshair = RoomsCrosshairsDictionary[r].RemoveAttacker(weapon);

            //if the crosshair isnt being used by any weapon anymore, destroy it
            if (destroyCrosshair)
            {
                Destroy(RoomsCrosshairsDictionary[weapon.Room.GetComponentInChildren<Room>()].gameObject);
                RoomsCrosshairsDictionary.Remove(weapon.Room.GetComponentInChildren<Room>());
            }
        }
    }
}
