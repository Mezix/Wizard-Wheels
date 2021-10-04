using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class BasicCannon : MonoBehaviour, IWeapon
{

    //  Weapon stats

    public WeaponStats _weaponStats;
    public string WeaponName { get; set; }
    public Sprite WeaponSprite { get; set; }
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; private set; }
    public float Damage { get; set; }
    public float RotationSpeed { get; set; }

    //  Aiming

    public GameObject Target { get; set; }
    public bool WeaponSelected { get; set; }
    public bool AimAtTarget { get; set; }
    public float AimRotationAngle { get; set; }

    //  Misc

    public GameObject _cannonballPrefab;
    public Transform _cannonballSpot;
    public GameObject _crosshairPrefab;
    private GameObject spawnedCrosshair;
    private LineRenderer laserLR;

    //  UI
    public Image WeaponCharge { get; set; }
    public int WeaponIndex { get; set; }
    public Text _weaponIndexText;

    //  Audio
    public AudioSource _weaponAudioSource = null;

    private void Start()
    {
        InitWeaponStats();
        laserLR = _cannonballSpot.GetComponentInChildren<LineRenderer>();
        AimRotationAngle = 90;
    }
    private void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        UpdateWeaponUI();
        UpdateLaserLR();
        HandleWeaponSelected();
    }
    

    private void FixedUpdate()
    {
        if (AimAtTarget)PointTurretAtTarget();
        else RotateTurretToAngle();
    }
    public void InitWeaponStats()
    {
        if (_weaponStats)  //if we have a scriptableobject, use its stats
        {
            WeaponName = _weaponStats._weaponName;
            WeaponSprite = _weaponStats._weaponSprite;
            AttacksPerSecond = _weaponStats._attacksPerSecond;
            Damage = _weaponStats._damage;
            RotationSpeed = _weaponStats._rotationSpeed;
        }
        else  //set default stats
        {
            print("No Weapon Stats found, setting defaults!");
            Damage = 1;
            AttacksPerSecond = 1;
            RotationSpeed = 5f;
        }
        TimeBetweenAttacks = 1 / AttacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks; //make sure we can fire right away
    }
    public void SetIndex(int i)
    {
        WeaponIndex = i;
        _weaponIndexText.text = i.ToString();
    }

    /// <summary>
    /// This Method handles everything to do with the operation of a weapon!
    /// </summary>
    public void HandleWeaponSelected()
    {
        if (WeaponSelected)
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
                Target = hit.collider.transform.parent.gameObject;
                SpawnCrosshairPrefab(hit.point, Target.transform);
                AimAtTarget = true;
            }
        }
        else
        {
            AimAtTarget = false;
            AimRotationAngle = HM.Angle2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
        }
        WeaponSelected = false;
    }
    public void CancelAim()
    {
        if (spawnedCrosshair) DestroyCrosshairPrefab();
        AimAtTarget = false;
        Target = null;
    }
    public void ResetAim()
    {
        if (spawnedCrosshair) DestroyCrosshairPrefab();
        AimRotationAngle = 90;
        AimAtTarget = false;
        Target = null;
    }

    //  ROTATE

    public void RotateTurretToAngle()
    {
        float zRotActual = 0;
        float diff = AimRotationAngle - transform.rotation.eulerAngles.z;
        if (diff < -180) diff += 360;

        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual = transform.rotation.eulerAngles.z + Mathf.Sign(diff) * RotationSpeed;
        }
        else
        {
            zRotActual = AimRotationAngle;
        }
        //  rotate to this newly calculate angle
        HM.RotateTransformToAngle(transform, new Vector3(0, 0, zRotActual));

    }
    public void PointTurretAtTarget()
    {
        //TODO: calculate for targets movement

        //  find the desired angle to face the mouse
        float zRotToMouse = HM.Angle2D(Target.transform.position, transform.position);
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

    //  USE WEAPON

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
    private void UpdateWeaponUI()
    {
        if(WeaponCharge) WeaponCharge.fillAmount = Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks);
    }
    private void SpawnCrosshairPrefab(Vector2 spawnPoint, Transform parent)
    {
        spawnedCrosshair = Instantiate(_crosshairPrefab);
        spawnedCrosshair.transform.position = spawnPoint;
        spawnedCrosshair.transform.parent = parent;
        spawnedCrosshair.GetComponentInChildren<Crosshair>().SetCrosshairWeaponIndex(WeaponIndex.ToString());
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
        if (_weaponAudioSource) _weaponAudioSource.Play();
        else Debug.LogError("missing audio clip for weapon!");
    }
    private void UpdateLaserLR()
    {
        if (Target)
        {
            laserLR.gameObject.SetActive(true);
            float distance = Vector3.Distance(Target.transform.position, _cannonballSpot.transform.position);
            laserLR.SetPosition(1, Vector3.right * distance);
        }
        else laserLR.gameObject.SetActive(false);

        //  if we can fire at the target, turn the laser green
        //  else keep the laser red
    }
}
