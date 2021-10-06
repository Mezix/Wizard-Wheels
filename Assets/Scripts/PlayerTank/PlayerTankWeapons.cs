using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankWeapons : MonoBehaviour
{
    public List<IWeapon> IWeaponArray = new List<IWeapon>();
    public List<UIWeapon> AllUIWeapons = new List<UIWeapon>();
    public bool multipleSelected = true;

    private void Update()
    {
        if (!PlayerTankController.instance._dying)
        {
            if (Input.GetKey(KeyCode.LeftShift)) multipleSelected = true;
            else multipleSelected = false;
            HandleWeaponSelection();
        }
    }

    private void HandleWeaponSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectWeapon(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectWeapon(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectWeapon(6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectWeapon(7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectWeapon(8);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SelectWeapon(9);
    }

    private void SelectWeapon(int weaponIndex)
    {
        if (!multipleSelected) DeselectAllWeapons();
        if (weaponIndex < IWeaponArray.Count) IWeaponArray[weaponIndex].WeaponSelected = true;
    }
    public void DeselectAllWeapons()
    {
        foreach(IWeapon wp in IWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
        }
    }

    public void InitWeapons()
    {
        //add a first Weapon as a dummy, so we can shoot weapons from 1-10 instead of 0-9
        IWeaponArray.Add(GetComponentInChildren<IWeapon>());
        //add all the weapons for real
        foreach (IWeapon wp in GetComponentsInChildren<IWeapon>())
        {
            IWeaponArray.Add(wp);
            wp.HitPlayer = false;
        }
    }
    public void ClearWeapons()
    {
        IWeaponArray.Clear();
    }
    public void CreateWeaponsUI()
    {
        //create ui for real
        for (int i = 1; i < IWeaponArray.Count; i++)
        {
            IWeaponArray[i].SetIndex(i);
            UIWeapon uw = UIScript.instance.CreateWeaponUI(IWeaponArray[i]);
            AllUIWeapons.Add(uw);
            if (i == 1) AllUIWeapons.Add(uw); //add a dummy first weapon ui to the list
            IWeaponArray[i].WeaponCharge = AllUIWeapons[i]._UIWeaponCharge;
        }
    }
}
