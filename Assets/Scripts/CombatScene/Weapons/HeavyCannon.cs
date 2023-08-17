using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class HeavyCannon : AWeapon
{
    public override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        ProjectilePrefab = Resources.Load(GS.WeaponPrefabs("CannonballProjectilePrefab"), typeof(GameObject)) as GameObject;
        WeaponFireExplosion = Resources.Load(GS.WeaponPrefabs("SingleExplosion"), typeof(GameObject)) as GameObject;
        ManualLocalAimingAngle = 90;

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
            if (IsAimingAtTarget) PointTurretAtTarget();
            else if (!ShouldNotRotate) RotateTurretToAngle();
        }
        else
        {
            StopInteraction();
        }
        UpdateLaserLR();
    }  
}
