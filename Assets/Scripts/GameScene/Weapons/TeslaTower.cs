using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TeslaTower : AWeapon
{
    private bool firingDirectionLocked;
    public int TimeUntilFire;
    public TeslaBeam _teslaBeam;
    public override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        ProjectilePrefab = (GameObject) Resources.Load("Weapons/CannonballProjectilePrefab");
        WeaponFireExplosion = (GameObject) Resources.Load("SingleExplosion");
        AimRotationAngle = 90;
        _teslaBeam.gameObject.SetActive(false);

        if (!ShouldHitPlayer) WeaponEnabled = false;
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
    public override void PointTurretAtTarget()
    {
        if (!TargetedRoom || firingDirectionLocked) return;

        //  find the desired angle to face the target
        float zRotToTarget = HM.GetAngle2DBetween(TargetedRoom.transform.position, transform.position);
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
        _teslaBeam.gameObject.SetActive(true);
        //  Init the tesla indicator
        int i = 0;
        while(i < TimeUntilFire)
        {
            //  TODO:
            //  Add flashing to indicate when we will actually fire!
            //  Overwrite bar in weapon Ui in blue that ticks down!
            i++;
            yield return new WaitForFixedUpdate();
        }
        Attack();
        _teslaBeam.ClearRoomsToHit();
        _teslaBeam.gameObject.SetActive(false);
        firingDirectionLocked = false;
        yield return null;
    }
    public override void Attack()
    {
        PlayWeaponFireSoundEffect();
        WeaponFireParticles(WeaponFireExplosion);
        TimeElapsedBetweenLastAttack = 0;
        foreach (Room r in _teslaBeam.roomsToHit)
        {
            r.tGeo.GetComponent<TankController>().TakeDamage(Damage);
        }
        if (!ShouldHitPlayer)
        {
            WeaponFeedback();
            REF.Dialog.FireWeapon();
        }
    }
}
