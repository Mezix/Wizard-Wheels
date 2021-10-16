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
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectWeapon(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectWeapon(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectWeapon(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectWeapon(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SelectWeapon(8);
    }

    private void SelectWeapon(int weaponIndex)
    {
        if (!multipleSelected) DeselectAllWeapons();
        if (weaponIndex < IWeaponArray.Count) IWeaponArray[weaponIndex].WeaponSelected = true;
    }
    public void DeselectAllWeapons()
    {
        foreach (IWeapon wp in IWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
        }
    }

    public void InitWeapons()
    {
        //add all the weapons for real
        foreach (IWeapon wp in GetComponentsInChildren<IWeapon>())
        {
            IWeaponArray.Add(wp);
            wp.ShouldHitPlayer = false;
        }
    }
    public void ClearWeapons()
    {
        IWeaponArray.Clear();
    }
    public void CreateWeaponsUI()
    {
        for (int i = 0; i < IWeaponArray.Count; i++)
        {
            IWeaponArray[i].SetIndex(i+1);
            UIWeapon uw = References.UI.CreateWeaponUI(IWeaponArray[i]);
            AllUIWeapons.Add(uw);
            IWeaponArray[i].WeaponCharge = AllUIWeapons[i]._UIWeaponCharge;
        }
    }
    public void WeaponBehaviourInDeath()
    {
        DeselectAllWeapons();
        foreach (IWeapon wp in IWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
            wp.ShouldNotRotate = true;
            wp.DestroyCrosshair();
        }
    }
}
