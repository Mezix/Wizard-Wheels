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
        for (int i = 1; i < IWeaponArray.Count; i++)
        {
            IWeaponArray[i].SetIndex(i);
        }
    }
    public void FireAllWeapons()
    {
        if(IWeaponArray.Count > 0)
        {
            foreach(IWeapon wep in IWeaponArray)
            {
                wep.AimAtTarget = true;
                wep.Room = PlayerTankController.instance.TGeo.rooms.GetComponentInChildren<Room>().gameObject;
            }
        }
    }
    public void FreezeWeaponsInDeath()
    {
        if (IWeaponArray.Count > 0)
        {
            foreach (IWeapon wep in IWeaponArray)
            {
                wep.AimAtTarget = false;
                wep.Room = null;
                wep.ShouldNotRotate = true;
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
