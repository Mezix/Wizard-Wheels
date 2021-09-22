using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IWeapon
{
    //WeaponStats weaponStats { get; set; }
    int weaponIndex { get; set; }
    string weaponName { get; set; }
    Sprite weaponSprite { get; set; }
    float AttacksPerSecond { get; set; }
    float TimeBetweenAttacks { get; }
    float TimeElapsedBetweenLastAttack { get; }
    float Damage { get; set; }
    float RotationSpeed { get; set; }
    GameObject _target { get; set; }
    bool weaponSelected { get; set; }
    bool aimAtTarget { get; set; }
    float _aimRotationAngle { get; set; }
    Image weaponCharge { get; set; }

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
