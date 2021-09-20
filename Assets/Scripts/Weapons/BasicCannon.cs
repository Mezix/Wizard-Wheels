using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicCannon : MonoBehaviour, IWeapon
{
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; private set; }
    public float Damage { get; set; }
    public float RotationSpeed = 5f;

    public GameObject _cannonballPrefab;
    public Transform _cannonballSpot;
    public WeaponStats _weaponStats;
    public Image _weaponChargeMeter;

    public GameObject _target;
    public bool weaponSelected { get; set; }
    public bool aimAtTarget;
    public float _aimRotationAngle;

    private void Start()
    {
        if (_weaponStats)  //if we have a scriptableobject, use its stats
        {
            AttacksPerSecond = _weaponStats.AttacksPerSecond;
            Damage = _weaponStats.Damage;
        }
        else //default stats
        {
            print("No Weapon Stats found");
            Damage = 4;
            AttacksPerSecond = 2;
        }
        TimeBetweenAttacks = 1 / AttacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks; //make sure we can fire right away
        _aimRotationAngle = 90;
    }

    private void FixedUpdate()
    {
        if (aimAtTarget)
        {
            PointTurretAtTarget();
        }
        else
        {
            RotateTurretToAngle();
        }
    }

    private void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        SetWeaponUI();
        if(weaponSelected)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                Aim();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CancelAim();
            }
        }
    }
    private void SetWeaponUI()
    {
        _weaponChargeMeter.fillAmount = Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks);
    }

    public void Attack()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            SpawnCannonball();
        }
    }
    private void SpawnCannonball()
    {
        GameObject cannonball = ProjectilePool.Instance.GetProjectileFromPool("cannonball");
        cannonball.GetComponent<IProjectile>().SetBulletStatsAndTransform(5, _cannonballSpot.transform.position, transform.rotation);
        cannonball.SetActive(true);
        TimeElapsedBetweenLastAttack = 0;
    }
    public void Aim()
    {
        RaycastHit2D hit = HM.RaycastToMouseCursor();
        if (hit.collider.transform.root.tag == "Enemy")
        {
            _target = hit.collider.transform.root.gameObject;
            aimAtTarget = true;
            print("found enemy, aiming!");
        }
        else
        {
            aimAtTarget = false;
            _aimRotationAngle = HM.Angle2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
            print("aim at direction");
        }
        weaponSelected = false;
    }
    public void CancelAim()
    {
        aimAtTarget = false;
        _target = null;
    }
    public void RotateTurretToAngle()
    {
        float zRotActual = 0;
        float diff = _aimRotationAngle - transform.rotation.eulerAngles.z;
        if (diff < -180) diff += 360;

        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual = transform.rotation.eulerAngles.z + Mathf.Sign(diff) * RotationSpeed;
        }
        else
        {
            zRotActual = _aimRotationAngle;
        }
        //  rotate to this newly calculate angle
        HM.RotateTransformToAngle(transform, new Vector3(0, 0, zRotActual));

    }
    public void PointTurretAtTarget()
    {
        //TODO: calculate for targets movement

        //  find the desired angle to face the mouse
        float zRotToMouse = HM.Angle2D(_target.transform.position, transform.position);
        //  get closer to the angle with our max rotationspeed
        float zRotActual = 0;
        float diff = zRotToMouse - transform.rotation.eulerAngles.z;
        if (diff < -180) diff += 360;

        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual = transform.rotation.eulerAngles.z + Mathf.Sign(diff) * RotationSpeed;
        }
        else
        {
            zRotActual = zRotToMouse;
            Attack();
        }

        //  rotate to this newly calculate angle
        HM.RotateTransformToAngle(transform, new Vector3(0, 0, zRotActual));
    }
}
