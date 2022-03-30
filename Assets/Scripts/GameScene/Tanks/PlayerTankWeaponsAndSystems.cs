using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankWeaponsAndSystems : TankWeaponsAndSystems
{
    public List<PlayerWeaponUI> AllUIWeapons = new List<PlayerWeaponUI>();
    public bool multipleSelected = true;

    public float _weaponRotationSpeedMultiplier;

    private void Awake()
    {
        _weaponRotationSpeedMultiplier = 1;
    }
    private void Update()
    {
        if (!Ref.PCon._dying)
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
        if (weaponIndex < AWeaponArray.Count) AWeaponArray[weaponIndex].WeaponSelected = true;
        Ref.PCon.DeselectAllWizards();
    }
    public void DeselectAllWeapons()
    {
        foreach (AWeapon wp in AWeaponArray)
        {
            if (wp != null)
            {
                wp.WeaponSelected = false;
                wp.PlayerUIWep.WeaponUISelected(false);
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
            PlayerWeaponUI uw = Ref.UI.CreateWeaponUI(AWeaponArray[i]);
            AllUIWeapons.Add(uw);
            AWeaponArray[i].PlayerUIWep = uw;
        }
    }
    public override void WeaponBehaviourInDeath()
    {
        DeselectAllWeapons();
        foreach (AWeapon wp in AWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
            wp.ShouldNotRotate = true;
            Ref.c.RemoveCrosshair(wp);
        }
    }

    public void UpdateWeaponRotationSpeed()
    {
        foreach(PlayerWeaponUI wep in AllUIWeapons)
        {
            wep._weapon.RotationSpeed = wep._weapon._weaponStats._rotationSpeed * _weaponRotationSpeedMultiplier;
        }
    }
}
