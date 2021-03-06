using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DottedLine;
using System;

public abstract class AWeapon : ISystem
{
    [SerializeField]
    private List<SpriteRenderer> _weaponSprites;

    //  Stats

    public WeaponStats _weaponStats;
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }
    public float Damage { get; set; }
    public float RotationSpeed { get; set; }
    public float MaxLockOnRange { get; set; }

    //  Aiming

    public bool WeaponSelected { get; set; }
    public bool WeaponEnabled { get; set; }
    public bool AimAtTarget { get; set; }
    public float AimRotationAngle { get; set; }
    public bool ShouldNotRotate { get; set; }
    public Transform RotatablePart;
    public GameObject TargetedRoom;
    private GameObject _targetingCircle;

    //  Misc

    public GameObject ProjectilePrefab { get; set; }
    public Transform _projectileSpot;
    public bool ShouldHitPlayer { get; set; }

    //  UI
    public PlayerWeaponUI PlayerUIWep { get; set; }

    public WeaponUI EnemyWepUI;
    public Color UIColor;
    public LineRenderer lr;

    //  Audio
    public AudioSource _weaponAudioSource = null;

    public void Awake()
    {
        ShouldHitPlayer = false;
        SystemObj = gameObject;
        InitLineRenderer();
    }
    public override void InitSystemStats()
    {
        if (_weaponStats)  //if we have a scriptableobject, use its stats
        {
            SystemName = _weaponStats._weaponName;
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
        InitCircle();
    }
    public void InitLineRenderer()
    {
        GameObject go = Instantiate((GameObject) Resources.Load("LineRendererPrefab"));
        go.transform.parent = transform;
        lr = go.GetComponent<LineRenderer>();
        lr.gameObject.SetActive(false);
    }
    private void InitCircle()
    {
        GameObject go = Instantiate((GameObject) Resources.Load("WeaponRangeCircle"));
        go.transform.localScale = Vector3.one * MaxLockOnRange;
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        _targetingCircle = go;
        _targetingCircle.SetActive(false);
    }
    public override void StartInteraction()
    {
        WeaponEnabled = true;
        IsBeingInteractedWith = true;
    }
    public override void StopInteraction()
    {
        WeaponEnabled = false;
        IsBeingInteractedWith = false;
    }

    public void SetOpacity(bool transparent)
    {
        if(_weaponSprites.Count == 0)
        {
            print("Sprites of weapon not initialized");
            return;
        }
        if(transparent)
        {
            foreach (SpriteRenderer sprite in _weaponSprites)
            {
                Color c = sprite.color;
                sprite.color = new Color(c.r, c.g, c.b, 0.5f);
            }
            //print("set weapon to transparent");
        }
        else
        {
            foreach (SpriteRenderer sprite in _weaponSprites)
            {
                Color c = sprite.color;
                sprite.color = new Color(c.r, c.g, c.b, 1);
            }
            //print("set weapon to solid");
        }
    }

    public void SetIndex(int i)
    {
        EnemyWepUI.WeaponIndex = i;
        EnemyWepUI._weaponIndexText.text = i.ToString();
    }
    /// <summary>
    /// This Method handles everything to do with the operation of a weapon!
    /// </summary>
    public void HandleWeaponSelected()
    {
        _targetingCircle.SetActive(false);
        if (WeaponSelected && WeaponEnabled && !ShouldHitPlayer)
        {
            _targetingCircle.SetActive(true);
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
        if (TargetedRoom) Ref.c.RemoveCrosshair(GetComponent<AWeapon>());

        RaycastHit2D hit = HM.RaycastToMouseCursor();
        if (hit.collider)
        {
            TankController targetTC = hit.collider.transform.root.GetComponentInChildren<TankController>();
            if (targetTC && hit.collider.transform.TryGetComponent(out Room targetRoom))
            {
                if (targetRoom.tGeo.GetComponent<TankController>().Equals(transform.root.GetComponentInChildren<TankController>()))
                {
                    print("Trying to target own Tank");
                    return;
                }
                TargetedRoom = targetRoom.gameObject;
                if (!(targetTC._dying || targetTC._dead))
                {
                    if (TargetRoomWithinLockOnRange())
                    {
                        Ref.c.AddCrosshair(TargetedRoom.GetComponentInChildren<Room>(), GetComponent<AWeapon>());
                        AimAtTarget = true;
                    }
                    else
                    {
                        StopCoroutine(Ref.UI.FlashWeaponOutOfRangeWarning());
                        StartCoroutine(Ref.UI.FlashWeaponOutOfRangeWarning());
                    }
                }
                
            }
        }
        else
        {
            AimAtTarget = false;
            AimRotationAngle = HM.GetAngle2DBetween(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
        }
        WeaponSelected = false;
        _targetingCircle.SetActive(false);
    }
    public void CancelAim()
    {
        TryGetComponent(out AWeapon iwep);
        if (iwep == null) return;
        Ref.c.RemoveCrosshair(iwep);
        AimAtTarget = false;
        TargetedRoom = null;
        _targetingCircle.SetActive(false);
    }
    public void ResetAim()
    {
        TryGetComponent(out AWeapon iwep);
        if (iwep == null) return;
        Ref.c.RemoveCrosshair(iwep);
        AimRotationAngle = 90;
        AimAtTarget = false;
        TargetedRoom = null;
        _targetingCircle.SetActive(false);
    }

    //  ROTATE

    public void RotateTurretToAngle()
    {
        TargetedRoom = null;
        float zRotActual = 0;
        float diff = AimRotationAngle - RotatablePart.rotation.eulerAngles.z;
        if (diff < -180) diff += 360;

        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual = RotatablePart.rotation.eulerAngles.z + Mathf.Sign(diff) * RotationSpeed;
        }
        else
        {
            zRotActual = AimRotationAngle;
        }
        //  rotate to this newly calculate angle
        HM.RotateTransformToAngle(RotatablePart, new Vector3(0, 0, zRotActual));

    }
    public void PointTurretAtTarget()
    {
        Vector3 TargetMoveVector = Vector3.zero;
        if (!TargetedRoom) return;
        float distance = Vector3.Distance(TargetedRoom.transform.position, _projectileSpot.transform.position);
        float TimeForProjectileToHitDistance = distance / (_weaponStats._projectileSpeed);

        //Calculate the position where our target will be
        if (TargetedRoom.transform.root.GetComponentInChildren<EnemyTankMovement>())
        {
            EnemyTankMovement mov = TargetedRoom.transform.root.GetComponentInChildren<EnemyTankMovement>();
            TargetMoveVector = mov.moveVector * mov.currentSpeed * TimeForProjectileToHitDistance;
        }

        //  find the desired angle to face the target
        float zRotToTarget = HM.GetAngle2DBetween(TargetedRoom.transform.position + TargetMoveVector, transform.position);
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
            Attack();
        }

        //  rotate to this newly calculate angle
        HM.RotateTransformToAngle(RotatablePart, new Vector3(0, 0, zRotActual));
    }

    //  USE WEAPON

    public void Attack()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            PlayWeaponFireSoundEffect();
            WeaponFireParticles();
            SpawnProjectile();
        }
    }
    private void SpawnProjectile()
    {
        GameObject proj = ProjectilePool.Instance.GetProjectileFromPool(ProjectilePrefab.tag);
        proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this);
        proj.GetComponent<AProjectile>().HitPlayer = ShouldHitPlayer;
        proj.SetActive(true);
        TimeElapsedBetweenLastAttack = 0;

    }

    //  UI
    protected void UpdateWeaponUI()
    {
        if(PlayerUIWep)
        {
            PlayerUIWep._UIWeaponCharge.fillAmount = Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks);
            PlayerUIWep.WeaponInteractable(WeaponEnabled);
        }
        if(ShouldHitPlayer)
        {
            EnemyWepUI.SetCharge(Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks));
        }
    }
    //  Misc

    private void WeaponFireParticles()
    {
        GameObject exp = Instantiate((GameObject)Resources.Load("SingleExplosion"));
        exp.transform.SetParent(_projectileSpot);
        exp.transform.localPosition = Vector3.zero;
    }
    private void PlayWeaponFireSoundEffect()
    {
        if (_weaponAudioSource) _weaponAudioSource.Play();
        else Debug.LogError("missing audio clip for weapon!");
    }
    protected void UpdateLaserLR()
    {
        lr.gameObject.SetActive(false);
        if (TargetedRoom && AimAtTarget && ShouldHitPlayer)
        {
            if (TargetRoomWithinLockOnRange())
            {
                //DottedLine.DottedLine.Instance.DrawDottedLine(transform.position, TargetedRoom.transform.position, UIColor);

                lr.gameObject.SetActive(true);
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, TargetedRoom.transform.position);
                lr.material.color = UIColor;
            }
        }
    }
    public void UpdateLockOn()
    {
        if (!TargetRoomWithinLockOnRange() && TargetedRoom)
        {
            CancelAim();
            //print("cancelling lock on");
        }
    }
    public bool TargetRoomWithinLockOnRange()
    {
        if (!TargetedRoom) return false;
        return Vector3.Distance(_projectileSpot.position, TargetedRoom.transform.position) < MaxLockOnRange;
    }
}
