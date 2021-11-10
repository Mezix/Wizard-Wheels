using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : AWeapon
{
    private void Awake()
    {
        SystemObj = gameObject;
    }
    private void Start()
    {
        ProjectilePrefab = (GameObject)Resources.Load("Weapons\\MagicMissileProjectilePrefab");
        laserLR = _projectileSpot.GetComponentInChildren<LineRenderer>();
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
            if (AimAtTarget) PointTurretAtTarget();
            else if (!ShouldNotRotate) RotateTurretToAngle();
        }
        else
        {
            StopInteraction();
        }
    }
}
