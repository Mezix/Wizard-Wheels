using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankWeaponsAndSystems : TankWeaponsAndSystems
{
    public WeaponStats EnemyBasicCannon;

    public override void SelectWeapon(int weaponIndex)
    {
        if (weaponIndex < AWeaponArray.Count) AWeaponArray[weaponIndex].WeaponSelected = true;
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
                if (wep.AimAtTarget) continue; //no need to continue to search for targets if we already have one

                //find the closest room to aim at
                GameObject targetRoom = FindTarget(); //TODO: try to find the nearest room with a high priority!
                wep.TargetedRoom = targetRoom;

                if (wep.TargetRoomWithinLockOnRange())
                {
                    wep.AimAtTarget = true;
                    Ref.c.AddCrosshair(wep.TargetedRoom.GetComponent<Room>(), wep);
                }
                else
                {
                    wep.TargetedRoom = null;
                    wep.AimAtTarget = false;
                }
            }
        }
    }

    private GameObject FindTarget()
    {
        Room[] possibleTargets = Ref.PCon.TGeo.RoomsParent.GetComponentsInChildren<Room>();
        return possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Length-1)].gameObject;
    }
    public override void WeaponBehaviourInDeath()
    {
        if (AWeaponArray.Count > 0)
        {
            foreach (AWeapon wep in AWeaponArray)
            {
                Ref.c.RemoveCrosshair(wep);
                wep.AimAtTarget = false;
                wep.TargetedRoom = null;
                wep.ShouldNotRotate = true;
            }
        }
    }
    public void ResetAllWeapons()
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
