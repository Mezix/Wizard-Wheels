using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TeslaTower : AWeapon
{
    private bool firingDirectionLocked;
    public float windupLengthInSeconds;
    public TeslaBeam _teslaBeam;

    public override void Start()
    {
        AngleToAimAt = 90;
        _teslaBeam.SetTeslaBeamSize(_weaponStats._lockOnRange);

        if (!ShouldHitPlayer) WeaponEnabled = false;
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
        //  Init the tesla indicator
        _weaponFireAnimator.speed = 1/(windupLengthInSeconds * 1.4f);
        WeaponFireParticles(); // start firing anim

        //TODO: charge up in blue in the UI

        //  Fire after the windup is done
        yield return new WaitForSeconds(windupLengthInSeconds);
        Attack();

        //  Wait out the animation
        yield return new WaitForSeconds(windupLengthInSeconds * 0.4f);
        firingDirectionLocked = false;
    }
    public override void Attack()
    {
        PlayWeaponFireSoundEffect();
        TimeElapsedBetweenLastAttack = 0;
        foreach (Room r in _teslaBeam.roomsToHit)
        {
            r._tGeo.GetComponent<TankController>().TakeDamage(Damage);
        }
        if (!ShouldHitPlayer)
        {
            WeaponFeedback();
            REF.Dialog.FireWeapon();
        }
    }
}
