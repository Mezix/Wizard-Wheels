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
    public bool aimAtTarget;
    public Vector3 _aimRotation;

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
    }

    private void FixedUpdate()
    {
        //if(_target)
        //{
        //    //calculate position where enemy will be according to projectile speed etc.
        //    Vector3 pos = _target.transform.position;
        //    PointTurretAtPosSmoothly(Camera.main.ScreenToWorldPoint(pos));
        //}
        //else
        //{
        //    RotateToDesiredRotationSmoothly();
        //}
        PointTurretAtPosSmoothly(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
        SetWeaponUI();
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
    private void PointTurretAtPosSmoothly(Vector3 pos)
    {
        //  find the desired angle to face the mouse
        float zRotToMouse = Mathf.Rad2Deg * Mathf.Atan2(pos.y-transform.position.y, pos.x - transform.position.x); //TODO: make this a helper method!

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
        }

        //  rotate to this newly calculate angle
        Quaternion q = new Quaternion();
        q.eulerAngles = new Vector3(0, 0, zRotActual);
        transform.rotation = q;
    }
    private void RotateToAngleSmoothly()
    {
        throw new NotImplementedException();
    }
}
