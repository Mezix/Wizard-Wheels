using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    float AttacksPerSecond { get; set; }
    float TimeBetweenAttacks { get; }
    float TimeElapsedBetweenLastAttack { get; }
    float Damage { get; set; }
    float RotationSpeed { get; set; }
    GameObject _target { get; set; }
    bool weaponSelected { get; set; }
    bool aimAtTarget { get; set; }
    float _aimRotationAngle { get; set; }

    void InitWeaponStats();
    void HandleWeaponSelected();
    void Aim();
    void CancelAim();
    void ResetAim();
    void RotateTurretToAngle();
    void PointTurretAtTarget();
    void Attack();
}
