using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IWeapon
{
    //WeaponStats weaponStats { get; set; }
    int WeaponIndex { get; set; }
    string WeaponName { get; set; }
    Sprite WeaponSprite { get; set; }
    float AttacksPerSecond { get; set; }
    float TimeBetweenAttacks { get; }
    float TimeElapsedBetweenLastAttack { get; }
    float Damage { get; set; }
    float RotationSpeed { get; set; }
    GameObject Room { get; set; }
    bool WeaponSelected { get; set; }
    bool AimAtTarget { get; set; }
    float AimRotationAngle { get; set; }
    Image WeaponCharge { get; set; }

    void InitWeaponStats();
    void SetIndex(int i);
    void HandleWeaponSelected();
    void Aim();
    void CancelAim();
    void ResetAim();
    void RotateTurretToAngle();
    void PointTurretAtTarget();
    void Attack();
}
