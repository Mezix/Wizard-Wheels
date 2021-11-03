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

    public GameObject SystemObj { get; set; }
    public string SystemName { get; set; }
    public RoomPosition RoomPosForInteraction { get; set; }
    public bool IsBeingInteractedWith { get; set; }
    public Sprite SystemSprite { get; set; }
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; private set; }
    public float Damage { get; set; }
    public float RotationSpeed { get; set; }
    public float MaxLockOnRange { get; set; }


    //  Aiming

    public GameObject Room { get; set; }
    public bool WeaponSelected { get; set; }
    public bool WeaponEnabled { get; set; }
    public bool AimAtTarget { get; set; }
    public float AimRotationAngle { get; set; }
    public bool ShouldNotRotate { get; set; }

    //  Misc

    public GameObject ProjectilePrefab { get; set; }
    public Transform _cannonballSpot;
    private LineRenderer laserLR;
    public bool ShouldHitPlayer { get; set; }

    //  UI
    public UIWeapon UIWep { get; set; }
    public int WeaponIndex { get; set; }

    public Text _weaponIndexText;

    //  Audio
    public AudioSource _weaponAudioSource = null;

    private void Awake()
    {
        SystemObj = gameObject;
    }
    private void Start()
    {
        ProjectilePrefab = (GameObject) Resources.Load("Weapons\\cannonball");
        laserLR = _cannonballSpot.GetComponentInChildren<LineRenderer>();
        AimRotationAngle = 90;

        if(!ShouldHitPlayer) WeaponEnabled = false;
    }
    private void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        UpdateWeaponUI();
        UpdateLaserLR();
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
    }
    public void InitSystem()
    {
        if (_weaponStats)  //if we have a scriptableobject, use its stats
        {
            SystemName = _weaponStats._weaponName;
            SystemSprite = _weaponStats._weaponSprite;
            AttacksPerSecond = _weaponStats._attacksPerSecond;
            Damage = _weaponStats._damage;
            RotationSpeed = _weaponStats._rotationSpeed;
            MaxLockOnRange = _weaponStats._lockOnRange;
        }
        else  //set default stats
        {
            print("No Weapon Stats found, setting defaults!");
            Damage = 1;
            AttacksPerSecond = 1;
            RotationSpeed = 5f;
            MaxLockOnRange = 100f;
        }
        TimeBetweenAttacks = 1 / AttacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks; //make sure we can fire right away
    }

    public void StartInteraction()
    {
        WeaponEnabled = true;
        IsBeingInteractedWith = true;
    }
    public void StopInteraction()
    {
        WeaponEnabled = false;
        IsBeingInteractedWith = false;
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
        if (WeaponSelected && WeaponEnabled && !ShouldHitPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                AimWithMouse();
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

    public void AimWithMouse()
    {
        if (Room) Ref.c.RemoveCrosshair(GetComponent<IWeapon>());

        RaycastHit2D hit = HM.RaycastToMouseCursor();
        if(hit.collider)
        {
            if (hit.collider.transform.root.GetComponentInChildren<IEnemy>() != null
                && hit.collider.transform.GetComponent<Room>())
            {
                Room = hit.collider.gameObject;
                if (TargetRoomWithinLockOnRange())
                {
                    Ref.c.AddCrosshair(Room.GetComponentInChildren<Room>(), GetComponent<IWeapon>());
                    AimAtTarget = true;
                }
                else
                {
                    //TODO: give a warning that the target is out of range on the screen!
                }
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
        Ref.c.RemoveCrosshair(GetComponent<IWeapon>());
        AimAtTarget = false;
        Room = null;
    }
    public void ResetAim()
    {
        Ref.c.RemoveCrosshair(GetComponent<IWeapon>());
        AimRotationAngle = 90;
        AimAtTarget = false;
        Room = null;
    }

    //  ROTATE

    public void RotateTurretToAngle()
    {
        Room = null;
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
        Vector3 TargetMoveVector = Vector3.zero;
        float distance = Vector3.Distance(Room.transform.position, _cannonballSpot.transform.position);
        float TimeForProjectileToHitDistance = distance / (_weaponStats._projectileSpeed);

        //Calculate the position where our target will be
        if (Room.transform.root.GetComponentInChildren<EnemyTankMovement>())
        {
            EnemyTankMovement mov = Room.transform.root.GetComponentInChildren<EnemyTankMovement>();
            TargetMoveVector = mov._movementVector * mov.velocity * TimeForProjectileToHitDistance;
        }

        //  find the desired angle to face the target
        float zRotToTarget = HM.Angle2D(Room.transform.position + TargetMoveVector, transform.position);
        //  get closer to the angle with our max rotationspeed
        float zRotActual;
        float diff = zRotToTarget - transform.rotation.eulerAngles.z;
        if (diff < -180) diff += 360;

        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual = transform.rotation.eulerAngles.z + Mathf.Sign(diff) * RotationSpeed;
        }
        else
        {
            zRotActual = zRotToTarget;
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
        GameObject cannonball = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
        cannonball.GetComponent<IProjectile>().SetBulletStatsAndTransform(_weaponStats._damage, _cannonballSpot.transform.position, transform.rotation);
        cannonball.GetComponent<IProjectile>().HitPlayer = ShouldHitPlayer;
        cannonball.SetActive(true);
        TimeElapsedBetweenLastAttack = 0;

    }

    //  UI
    private void UpdateWeaponUI()
    {
        if (UIWep)
        {
            UIWep._UIWeaponCharge.fillAmount = Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks);
            UIWep.WeaponInteractable(WeaponEnabled);
        }
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
        if (Room && AimAtTarget)
        {
            if(TargetRoomWithinLockOnRange())
            {
                laserLR.gameObject.SetActive(true);
                //float distance = Vector3.Distance(Room.transform.position, _cannonballSpot.transform.position);
                laserLR.SetPosition(0, _cannonballSpot.transform.position);
                laserLR.SetPosition(1, Room.transform.position);
            }
        }
        else laserLR.gameObject.SetActive(false);

        //  if we can fire at the target, turn the laser green
        //  else keep the laser red
    }
    public void UpdateLockOn()
    {
        if(!TargetRoomWithinLockOnRange() && Room)
        {
            CancelAim();
            print("cancelling lock on");
        }
    }
    public bool TargetRoomWithinLockOnRange()
    {
        if (!Room) return false;
        return Vector3.Distance(_cannonballSpot.position, Room.transform.position) < MaxLockOnRange;
    }
}
