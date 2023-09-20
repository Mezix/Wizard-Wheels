using System;
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
            WeaponUI.SetCharge(Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks), _firingStatus);
        }
        HM.RotateTransformToAngle(WeaponUI._weaponIndexText.transform, new Vector3(0, 0, 0));
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
                firingDirectionLocked = true;
                StartCoroutine(StartAttack());
            }
        }

        //  rotate to this newly calculate angle
        HM.RotateTransformToAngle(RotatablePart, new Vector3(0, 0, zRotActual));
    }
    public IEnumerator StartAttack()
    {
        _firingStatus = FiringStatus.Charging;

        float animLength = 1.4f;

        //  Init the tesla indicator
        _weaponFireAnimator.speed = 1/(windupLengthInSeconds * animLength);
        WeaponFireParticles(); // start firing anim

        while(_timeWindingUp < windupLengthInSeconds)
        {
            _timeWindingUp = Mathf.Min(windupLengthInSeconds, _timeWindingUp + Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        //  Fire after the windup is done
        AttemptAttack();

        //  Wait out the animation
        yield return new WaitForSeconds(windupLengthInSeconds * 1 - animLength);
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
