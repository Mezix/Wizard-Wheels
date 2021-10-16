using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankWeapons : MonoBehaviour
{
    public List<IWeapon> IWeaponArray = new List<IWeapon>();

    private void SelectWeapon(int weaponIndex)
    {
        if (weaponIndex < IWeaponArray.Count) IWeaponArray[weaponIndex].WeaponSelected = true;
    }
    public void InitWeapons()
    {
        foreach (IWeapon wp in GetComponentsInChildren<IWeapon>())
        {
            IWeaponArray.Add(wp);
            wp.ShouldHitPlayer = true;
        }
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
            foreach(IWeapon wep in IWeaponArray)
            {
                if (wep.AimAtTarget) return; //no need to continue to search for targets if we already have one

                GameObject targetRoom = FindTarget();
                wep.AimAtTarget = true;
                wep.Room = targetRoom;
                wep.SpawnCrosshair(wep.Room.transform);
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
                wep.DestroyCrosshair();
            }
        }
    }
    internal void ResetAllWeapons()
    {
        if (IWeaponArray.Count > 0)
        {
            foreach (IWeapon wep in IWeaponArray)
            {
                wep.ResetAim();
            }
        }
    }
}
