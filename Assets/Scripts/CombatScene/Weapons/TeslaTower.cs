﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TeslaTower : AWeapon
{
    private bool firingDirectionLocked;
    public float _timeWindingUp;
    public float windupLengthInSeconds;
    public TeslaBeam _teslaBeam;

    public override void Start()
    {
        AngleToAimAt = 90;
        _timeWindingUp = 0;
        windupLengthInSeconds = 1.0f;
        _teslaBeam.SetTeslaBeamSize(_weaponStats._lockOnRange);

        if (!ShouldHitPlayer) WeaponEnabled = false;
    }

    public override void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        UpdateWeaponUI();
        UpdateLockOn();
        HandleWeaponSelected();
    }
    public override void UpdateWeaponUI()
    {
        if (PlayerWepUI)
        {
            PlayerWepUI.WeaponIsBeingInteractedWith(WeaponEnabled);

            if (_firingStatus.Equals(FiringStatus.Reloading))
            {
                PlayerWepUI.SetCharge(Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks), _firingStatus);
            }
            else
            {
                PlayerWepUI.SetCharge(Mathf.Min(1, _timeWindingUp / windupLengthInSeconds), _firingStatus);
            }
        }
        if (ShouldHitPlayer)
        {
            if (_firingStatus.Equals(FiringStatus.Reloading))
            {
                WeaponUI.SetCharge(Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks), _firingStatus);
            }
            else
            {
                WeaponUI.SetCharge(Mathf.Min(1, _timeWindingUp / windupLengthInSeconds), _firingStatus);
            }
        }
        if (WeaponUI) HM.RotateTransformToAngle(WeaponUI._weaponIndexText.transform, new Vector3(0, 0, 0));
    }
    public override void PointTurretAtTarget()
    {
        if (!TargetedRoom || firingDirectionLocked) return;

        //  find the desired angle to face the target
        float zRotToTarget = HM.GetEulerAngle2DBetween(TargetedRoom.transform.position, transform.position);
        //  get closer to the angle with our max rotationspeed
        float zRotActual;
        float diff = zRotToTarget - RotatablePart.rotation.eulerAngles.z;
        if (diff < -180) diff += 360;

        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual = RotatablePart.rotation.eulerAngles.z + Mathf.Sign(diff) * RotationSpeed;
        }
        else
        {
            zRotActual = zRotToTarget;
            if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
            {
                StartCoroutine(StartAttack());
            }
        }

        //  rotate to this newly calculate angle
        HM.RotateTransformToAngle(RotatablePart, new Vector3(0, 0, zRotActual));
    }
    public override void AimWithMouse()
    {
        if (firingDirectionLocked) return;
        if (TargetedRoom) REF.c.RemoveCrosshair(GetComponent<AWeapon>());

        RaycastHit2D hit = HM.RaycastToMouseCursor(LayerMask.GetMask("Room"));
        if (hit.collider && hit.collider.tag != "Level")
        {
            TankController targetTC = hit.collider.transform.root.GetComponentInChildren<TankController>();
            if (targetTC && hit.collider.transform.TryGetComponent(out Room targetRoom))
            {
                //Debug.Log(targetTC.name);
                if (targetRoom._tGeo.GetComponent<TankController>().Equals(transform.root.GetComponentInChildren<TankController>()))
                {
                    //Debug.Log("Trying to target own Tank");
                    return;
                }
                TargetedRoom = targetRoom.gameObject;
                if (!(targetTC._dying || targetTC._dead))
                {
                    if (TargetRoomIsWithinLockOnRange())
                    {
                        REF.c.AddCrosshair(TargetedRoom.GetComponentInChildren<Room>(), GetComponent<AWeapon>());
                        IsAimingAtTarget = true;
                    }
                    else
                    {
                        StopCoroutine(REF.CombatUI.FlashWeaponOutOfRangeWarning());
                        StartCoroutine(REF.CombatUI.FlashWeaponOutOfRangeWarning());
                    }
                }

            }
        }
        else
        {
            IsAimingAtTarget = false;
            AngleToAimAt = HM.GetEulerAngle2DBetween(transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)), transform.localPosition);

            if (AngleToAimAt > _maxAllowedAngleToTurn) AngleToAimAt = _maxAllowedAngleToTurn;
            else if (AngleToAimAt < -_maxAllowedAngleToTurn) AngleToAimAt = -_maxAllowedAngleToTurn;
        }
        WeaponSelected = false;
    }
    public IEnumerator StartAttack()
    {
        firingDirectionLocked = true;
        _firingStatus = FiringStatus.Charging;

        float timeUntilChargeAnimationIsOver = 0.714f;

        //  Init the tesla indicator
        _weaponFireAnimator.speed = 1/(windupLengthInSeconds * timeUntilChargeAnimationIsOver);
        WeaponFireParticles(); // start firing anim

        while(_timeWindingUp < windupLengthInSeconds)
        {
            _timeWindingUp = Mathf.Min(windupLengthInSeconds, _timeWindingUp + Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        //  Fire after the windup is done
        AttemptAttack();

        //  Wait out the animation
        yield return new WaitForSeconds(windupLengthInSeconds * 1 - timeUntilChargeAnimationIsOver);
        firingDirectionLocked = false;
    }
    public override void AttemptAttack()
    {
        _firingStatus = FiringStatus.Reloading;
        PlayWeaponFireSoundEffect();
        TimeElapsedBetweenLastAttack = 0;
        _timeWindingUp = 0;
        foreach (Room r in _teslaBeam.roomsToHit)
        {
            r._tGeo.GetComponent<TankController>().TakeDamage(Damage);
        }
        if (!ShouldHitPlayer)
        {
            WeaponFeedback();
            //REF.Dialog.FireWeapon();
        }
    }
}
