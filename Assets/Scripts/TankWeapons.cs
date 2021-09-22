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
        if (IWeaponArray[weaponIndex] != null) IWeaponArray[weaponIndex].weaponSelected = true;
    }

    public void InitWeapons()
    {
        //add a first Weapon as a dummy, so we can shoot weapons from 1-10 instead of 0-9
        IWeaponArray.Add(GetComponentInChildren<IWeapon>()); 
        //add all the weapons for real
        foreach (IWeapon wp in GetComponentsInChildren<IWeapon>()) IWeaponArray.Add(wp);
        for (int i = 1; i < IWeaponArray.Count - 1; i++) IWeaponArray[i].SetIndex(i);
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
