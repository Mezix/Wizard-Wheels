using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class BasicCannon : AWeapon
{
    private void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        ProjectilePrefab = (GameObject) Resources.Load("Weapons\\CannonballProjectilePrefab");
        AimRotationAngle = 90;

        if(!ShouldHitPlayer) WeaponEnabled = false;
    }
    private void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        UpdateWeaponUI();
        UpdateLockOn();
        HandleWeaponSelected();
    }
    private void FixedUpdate()
    {
        if(WeaponEnabled)
        {
            if (AimAtTarget) PointTurretAtTarget();
            else if (!ShouldNotRotate) RotateTurretToAngle();
        }
        else
        {
            StopInteraction();
        }
        UpdateLaserLR();
    }  
}
