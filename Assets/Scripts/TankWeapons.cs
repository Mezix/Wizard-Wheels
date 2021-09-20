using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWeapons : MonoBehaviour
{
    public List<IWeapon> IWeaponArray = new List<IWeapon>();

    void Start()
    {
        InitWeapons();
        CreateWeaponsUI();
    }
    private void Update()
    {
        HandleWeaponInput();
    }

    private void HandleWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) FireWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) FireWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) FireWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) FireWeapon(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) FireWeapon(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) FireWeapon(6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) FireWeapon(7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) FireWeapon(8);
        if (Input.GetKeyDown(KeyCode.Alpha9)) FireWeapon(9);
    }

    private void FireWeapon(int weaponIndex)
    {
        if (IWeaponArray[weaponIndex] != null) IWeaponArray[weaponIndex].Attack();
        print("firing weapon from keybind");
    }

    public void InitWeapons()
    {
        //get the first Weapon as a dummy, so we can shoot weapons from 1-10 instead of 0-9
        IWeaponArray.Add(GetComponentInChildren<IWeapon>()); 
        //add all the weapons for real
        foreach (IWeapon wp in GetComponentsInChildren<IWeapon>()) IWeaponArray.Add(wp);
    }
    public void ClearWeapons()
    {
        IWeaponArray.Clear();
    }
    public void CreateWeaponsUI()
    {
        for (int i = 1; i < IWeaponArray.Count; i++)
        UIScript.instance.CreateWeaponUI(i, IWeaponArray[i]);
    }
}
