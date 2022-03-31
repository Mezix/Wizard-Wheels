using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankWeaponsAndSystems : MonoBehaviour
{
    public List<AWeapon> AWeaponArray = new List<AWeapon>();
    public List<ISystem> ISystemArray = new List<ISystem>();

    public virtual void SelectWeapon(int weaponIndex)
    {
    }
    public virtual void WeaponBehaviourInDeath()
    {
        foreach (AWeapon wp in AWeaponArray)
        {
            if (wp != null) wp.WeaponSelected = false;
            wp.ShouldNotRotate = true;
            Ref.c.RemoveCrosshair(wp);
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
            }
        }
    }
    public void SetUpSystems(bool player)
    {
        if (player)
        {
            foreach (ISystem sys in ISystemArray)
            {
                sys.InitSystemStats();
            }
        }
        else
        {
            foreach (ISystem sys in ISystemArray)
            {
                sys.InitSystemStats();
            }
        }
    }
}
