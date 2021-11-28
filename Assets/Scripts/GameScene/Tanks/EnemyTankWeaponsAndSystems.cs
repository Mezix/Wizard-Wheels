using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankWeaponsAndSystems : MonoBehaviour
{
    public List<AWeapon> AWeaponArray = new List<AWeapon>();
    public List<ISystem> ISystemArray = new List<ISystem>();

    public WeaponStats EnemyBasicCannon;

    private void SelectWeapon(int weaponIndex)
    {
        if (weaponIndex < AWeaponArray.Count) AWeaponArray[weaponIndex].WeaponSelected = true;
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
                    if (tr.SavedPrefabRefMatrix.XArray[x].YStuff[y].RoomSystemPrefab.GetComponent<AWeapon>() != null)
                    {
                        GameObject weaponObj = Instantiate(prefab);
                        weaponObj.transform.parent = tr.RoomPosMatrix[x, y].ParentRoom.transform;
                        weaponObj.transform.localPosition = Vector3.zero;
                        AWeapon wep = weaponObj.GetComponent<AWeapon>();
                        wep._weaponStats = EnemyBasicCannon;
                        wep.InitSystem();
                        PositionSystemInRoom(weaponObj.GetComponent<ISystem>(), weaponObj.transform.parent.GetComponent<Room>());
                        wep.ShouldHitPlayer =  wep.WeaponSelected = wep.WeaponEnabled = true;
                        wep.EnemyWepUI.ShowWeaponUI(true);
                        AWeaponArray.Add(wep);

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
        system.SystemObj.transform.localPosition = Vector2.zero;
        if (parentRoom.sizeX > 1) system.SystemObj.transform.localPosition += new Vector3(0.25f, 0);
        if (parentRoom.sizeY > 1) system.SystemObj.transform.localPosition += new Vector3(0, -0.25f);
    }
    public void CreateWeaponsUI()
    {
        for (int i = 0; i < AWeaponArray.Count; i++)
        {
            AWeaponArray[i].SetIndex(i + 1);
        }
    }
    public void AcquireTargetsForAllWeapons()
    {
        if(AWeaponArray.Count > 0)
        {
            foreach (AWeapon wep in AWeaponArray)
            {
                if (wep.AimAtTarget) return; //no need to continue to search for targets if we already have one

                //find the closest room to aim at
                GameObject targetRoom = FindTarget(); //TODO: try to find the nearest room with a high priority!
                wep.Room = targetRoom;

                if (wep.TargetRoomWithinLockOnRange())
                {
                    wep.AimAtTarget = true;
                    Ref.c.AddCrosshair(wep.Room.GetComponent<Room>(), wep);
                }
                else
                {
                    wep.Room = null;
                    wep.AimAtTarget = false;
                }
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
        if (AWeaponArray.Count > 0)
        {
            foreach (AWeapon wep in AWeaponArray)
            {
                Ref.c.RemoveCrosshair(wep);
                wep.AimAtTarget = false;
                wep.Room = null;
                wep.ShouldNotRotate = true;
            }
        }
    }
    internal void ResetAllWeapons()
    {
        if (AWeaponArray.Count > 0)
        {
            foreach (AWeapon wep in AWeaponArray)
            {
                if(wep != null) wep.ResetAim();
            }
        }
    }
}
