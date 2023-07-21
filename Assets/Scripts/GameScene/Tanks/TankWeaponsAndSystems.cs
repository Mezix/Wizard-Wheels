using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWeaponsAndSystems : MonoBehaviour
{
    public List<AWeapon> AWeaponArray = new List<AWeapon>();
    public List<ASystem> ASystemArray = new List<ASystem>();

    public virtual void SelectWeapon(int weaponIndex)
    {
    }
    public virtual void WeaponBehaviourInDeath()
    {
        foreach (AWeapon wp in AWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
            wp.ShouldNotRotate = true;
            REF.c.RemoveCrosshair(wp);
        }
    }
    public void SetUpWeapons(bool player, Color color)
    {
        if (player)
        {
            foreach(AWeapon wep in AWeaponArray)
            {
                wep.ShouldHitPlayer = false;
                wep.WeaponUI.ShowWeaponUI(player);
                wep.InitSystemStats();
                wep.tMov = GetComponent<TankMovement>();
            }
        }
        else
        {
            foreach (AWeapon wep in AWeaponArray)
            {
                wep._weaponStats = GetComponent<EnemyTankWeaponsAndSystems>().EnemyBasicCannon;
                wep.ShouldHitPlayer = wep.WeaponSelected = wep.WeaponEnabled = true;
                wep.WeaponUI.ShowWeaponUI(player);
                wep.InitSystemStats();
                wep.UIColor = color;
                wep.tMov = GetComponent<TankMovement>();
            }
        }
    }
    public void SetUpSystems(bool player)
    {
        if (player)
        {
            foreach (ASystem sys in ASystemArray)
            {
                sys.InitSystemStats();
            }
        }
        else
        {
            foreach (ASystem sys in ASystemArray)
            {
                sys.InitSystemStats();
            }
        }
    }
}
