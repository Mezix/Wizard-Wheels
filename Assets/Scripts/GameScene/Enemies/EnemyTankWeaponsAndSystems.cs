using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankWeaponsAndSystems : MonoBehaviour
{
    public List<IWeapon> IWeaponArray = new List<IWeapon>();
    public List<ISystem> ISystemArray = new List<ISystem>();

    private void SelectWeapon(int weaponIndex)
    {
        if (weaponIndex < IWeaponArray.Count) IWeaponArray[weaponIndex].WeaponSelected = true;
    }
    public void InitWeaponsAndSystems(TankRoomConstellation tr)
    {
        for (int x = 0; x < tr.XTilesAmount; x++)
        {
            for (int y = 0; y < tr.YTilesAmount; y++)
            {
                if (tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab)
                {
                    GameObject prefab = tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab;

                    //Our object should either be a Weapon or a System, so check for both cases
                    if (tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<IWeapon>() != null)
                    {
                        GameObject weaponObj = Instantiate(prefab);
                        weaponObj.transform.parent = tr.RoomPosMatrix[x, y].ParentRoom.transform;
                        weaponObj.transform.localPosition = Vector3.zero;
                        IWeapon wep = weaponObj.GetComponent<IWeapon>();
                        wep.InitSystem();
                        PositionSystemInRoom(weaponObj.GetComponent<ISystem>(), weaponObj.transform.parent.GetComponent<Room>());
                        wep.ShouldHitPlayer =  wep.WeaponSelected = wep.WeaponEnabled = true;
                        IWeaponArray.Add(wep);

                        //Set the reference to the rooms
                        tr.RoomPosMatrix[x, y].ParentRoom.roomSystem = wep;
                        tr.RoomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = wep.SystemSprite;

                    }
                    else if (tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<ISystem>() != null)
                    {
                        GameObject systemObj = Instantiate(prefab);
                        systemObj.transform.parent = tr.RoomPosMatrix[x, y].ParentRoom.transform;

                        systemObj.transform.localPosition = Vector3.zero;
                        ISystem sys = systemObj.GetComponent<ISystem>();
                        sys.InitSystem();
                        PositionSystemInRoom(systemObj.GetComponent<ISystem>(), systemObj.transform.parent.GetComponent<Room>());
                        ISystemArray.Add(sys);

                        //Set the reference to the rooms
                        tr.RoomPosMatrix[x, y].ParentRoom.roomSystem = sys;
                        tr.RoomPosMatrix[x, y].ParentRoom.roomSystemRenderer.sprite = sys.SystemSprite;
                    }
                }
            }
        }
    }

    private void PositionSystemInRoom(ISystem system, Room parentRoom)
    {
        print("Position of System set");
        system.SystemObj.transform.localPosition = Vector3.zero;
    }
    public void CreateWeaponsUI()
    {
        for (int i = 0; i < IWeaponArray.Count; i++)
        {
            IWeaponArray[i].SetIndex(i + 1);
        }
    }
    public void AcquireTargetsForAllWeapons()
    {
        if(IWeaponArray.Count > 0)
        {
            foreach (IWeapon wep in IWeaponArray)
            {
                if (wep.AimAtTarget) return; //no need to continue to search for targets if we already have one

                GameObject targetRoom = FindTarget();
                wep.AimAtTarget = true;
                wep.Room = targetRoom;
                Ref.c.AddCrosshair(wep.Room.GetComponent<Room>(), wep);
            }
        }
    }

    private GameObject FindTarget()
    {
        Room[] possibleTargets = PlayerTankController.instance.TGeo.RoomsParent.GetComponentsInChildren<Room>();
        return possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Length-1)].gameObject;
    }

    public void WeaponBehaviourInDeath()
    {
        if (IWeaponArray.Count > 0)
        {
            foreach (IWeapon wep in IWeaponArray)
            {
                wep.AimAtTarget = false;
                wep.Room = null;
                wep.ShouldNotRotate = true;
                Ref.c.RemoveCrosshair(wep);
            }
        }
    }
    internal void ResetAllWeapons()
    {
        if (IWeaponArray.Count > 0)
        {
            foreach (IWeapon wep in IWeaponArray)
            {
                if(wep != null) wep.ResetAim();
            }
        }
    }
}
