using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : AWeapon
{
    public override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        ProjectilePrefab = (GameObject)Resources.Load("Weapons/MagicMissileProjectilePrefab");
        AimRotationAngle = 90;

        if (!ShouldHitPlayer) WeaponEnabled = false;
    }
    private void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        UpdateWeaponUI();
        UpdateLaserLR();
        UpdateLockOn();
        HandleWeaponSelected();
    }
    private void FixedUpdate()
    {
        if (WeaponEnabled)
        {
            if (IsAimingAtTarget) PointTurretAtTarget();
            else if (!ShouldNotRotate) RotateTurretToAngle();
        }
        else
        {
            StopInteraction();
        }
    }
}
