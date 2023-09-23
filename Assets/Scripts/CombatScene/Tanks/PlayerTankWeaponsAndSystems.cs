using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankWeaponsAndSystems : TankWeaponsAndSystems
{
    public bool multipleSelected = true;
    public float _weaponRotationSpeedMultiplier;

    private void Awake()
    {
        _weaponRotationSpeedMultiplier = 1;
    }
    private void Update()
    {
        if (!REF.PCon._dying)
        {
            if (Input.GetKey(KeyCode.LeftShift)) multipleSelected = true;
            else multipleSelected = false;

            if (!(Input.GetKey(KeyCode.LeftControl))) HandleWeaponSelection();
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
    public override void SelectWeapon(int weaponIndex)
    {
        if (!multipleSelected) DeselectAllWeapons();
        if (weaponIndex < AWeaponArray.Count)
        {
            if(AWeaponArray[weaponIndex].WeaponEnabled)
            {
                AWeaponArray[weaponIndex].WeaponSelected = true;
                AWeaponArray[weaponIndex].PlayerWepUI.SelectWeapon();
                AWeaponArray[weaponIndex]._weaponSelectedUI.UpdateWeaponSelectedLR();
            }
        }
        REF.PCon.DeselectAllWizards();
    }
    public void DeselectAllWeapons()
    {
        foreach (AWeapon wp in AWeaponArray)
        {
            if (wp != null)
            {
                wp.WeaponSelected = false;
                wp.PlayerWepUI.WeaponUISelected(false);
                wp._weaponSelectedUI.UpdateWeaponSelectedLR();
            }
        }
    }
    public void ClearWeapons()
    {
        AWeaponArray.Clear();
    }
    public void CreateWeaponsUI()
    {
        for (int i = 0; i < AWeaponArray.Count; i++)
        {
            AWeaponArray[i].SetIndex(i+1);
            REF.CombatUI.CreateWeaponUI(AWeaponArray[i]);
        }
    }
    public override void WeaponBehaviourInDeath()
    {
        DeselectAllWeapons();
        foreach (AWeapon wp in AWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
            wp.ShouldNotRotate = true;
            REF.c.RemoveCrosshair(wp);
        }
    }

    public void UpdateWeaponRotationSpeed()
    {
        foreach(AWeapon wep in AWeaponArray)
        {
            wep.RotationSpeed = wep._weaponStats._rotationSpeed * _weaponRotationSpeedMultiplier;
        }
    }
}
