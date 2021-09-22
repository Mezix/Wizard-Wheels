﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class BasicCannon : MonoBehaviour, IWeapon
{
    public float weaponIndex = 0;

    //  Weapon stats

    public WeaponStats _weaponStats;
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; private set; }
    public float Damage { get; set; }
    public float RotationSpeed { get; set; }

    //  Aiming

    public GameObject _target { get; set; }
    public bool weaponSelected { get; set; }
    public bool aimAtTarget { get; set; }
    public float _aimRotationAngle { get; set; }

    //  Misc

    public GameObject _cannonballPrefab;
    public GameObject _crosshairPrefab;
    private GameObject spawnedCrosshair;
    public Transform _cannonballSpot;
    public Image _weaponChargeMeter;

    //  Audio
    public AudioSource weaponAudioSource = null;

    private void Start()
    {
        InitWeaponStats();
        _aimRotationAngle = 90;
    }
    private void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        SetWeaponUI();
        HandleWeaponSelected();
    }
    private void FixedUpdate()
    {
        if (aimAtTarget)PointTurretAtTarget();
        else RotateTurretToAngle();
    }
    public void InitWeaponStats()
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
        RotationSpeed = 5f;
    }
    public void SetIndex(int i)
    {
        weaponIndex = i;
    }

    /// <summary>
    /// This Method handles everything to do with the operation of a weapon!
    /// </summary>
    public void HandleWeaponSelected()
    {
        if (weaponSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Aim();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ResetAim();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelAim();
            }
        }
    }

    //  AIMING

    public void Aim()
    {
        if (spawnedCrosshair) DestroyCrosshairPrefab();
        RaycastHit2D hit = HM.RaycastToMouseCursor();
        if(hit.collider)
        {
            if (hit.collider.transform.root.tag == "Enemy")
            {
                _target = hit.collider.transform.parent.gameObject;
                SpawnCrosshairPrefab(hit.point, _target.transform);
                aimAtTarget = true;
            }
        }
        else
        {
            aimAtTarget = false;
            _aimRotationAngle = HM.Angle2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
        }
        weaponSelected = false;
    }
    public void CancelAim()
    {
        if (spawnedCrosshair) DestroyCrosshairPrefab();
        aimAtTarget = false;
        _target = null;
    }
    public void ResetAim()
    {
        if (spawnedCrosshair) DestroyCrosshairPrefab();
        _aimRotationAngle = 90;
        aimAtTarget = false;
        _target = null;
    }

    //  ROTATE

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

    //  FIRE

    public void Attack()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            PlayWeaponFireSoundEffect();
            WeaponFireParticles();
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

    //  UI
    private void SetWeaponUI()
    {
        _weaponChargeMeter.fillAmount = Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks);
    }
    private void SpawnCrosshairPrefab(Vector2 spawnPoint, Transform parent)
    {
        spawnedCrosshair = Instantiate(_crosshairPrefab);
        spawnedCrosshair.transform.position = spawnPoint;
        spawnedCrosshair.transform.parent = parent;
        spawnedCrosshair.GetComponentInChildren<Crosshair>().SetCrosshairWeaponIndex(weaponIndex.ToString());
    }
    private void DestroyCrosshairPrefab()
    {
         Destroy(spawnedCrosshair);
    }

    //  Misc
    private void WeaponFireParticles()
    {
    }

    private void PlayWeaponFireSoundEffect()
    {
        if (weaponAudioSource) weaponAudioSource.Play();
        else Debug.LogError("missing audio clip for weapon!");
    }
}
