using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DottedLine;
using System;

public abstract class AWeapon : ASystem
{
    //  Stats

    public WeaponStats _weaponStats;
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }
    public int Damage { get; set; }
    public float RotationSpeed { get; set; }
    public float MaxLockOnRange { get; set; }
    public float _maxAllowedAngleToTurn { get; set; }
    public float Recoil { get; set; }
    public float RecoilDuration { get; set; }

    public bool WeaponSelected { get; set; }
    public bool WeaponEnabled { get; set; }
    public bool IsAimingAtTarget { get; set; }
    public float AngleToAimAt;
    public bool ShouldNotRotate { get; set; }
    public Transform RotatablePart;

    [HideInInspector]
    public GameObject TargetedRoom;

    //  Misc

    public GameObject ProjectilePrefab { get; set; }
    public List<Transform> _projectileSpots = new List<Transform>();
    public bool ShouldHitPlayer { get; set; }

    //  UI
    public PlayerWeaponUI PlayerWepUI { get; set; }
    public WeaponUI WeaponUI;
    public Color UIColor;
    public LineRenderer lr;

    //  Feedback
    public Animator _weaponAnimator;
    public Animator _weaponFireAnimator;
    public AudioSource _weaponAudioSource = null;
    private float pitchVariance = 0.1f;
    public float _volumePct;

    //  TankMovement

    [HideInInspector]
    public TankMovement tMov;
    [HideInInspector]
    public float tankSpeedProjectileModifier;

    //  Selected
    [HideInInspector]
    public WeaponSelectedUI _weaponSelectedUI;
    public WeaponHoveringUI _weaponHoveringUI;
    public FiringStatus _firingStatus;
    public enum FiringStatus
    {
        Reloading,
        Charging
    }

    public override void Awake()
    {
        pitchVariance = 0.1f;
        SystemObj = gameObject;
        ShouldHitPlayer = false;
        tankSpeedProjectileModifier = 0;
    }
    public virtual void Start()
    {
        ProjectilePrefab = Resources.Load(GS.WeaponPrefabs("CannonballProjectilePrefab"), typeof(GameObject)) as GameObject;
        AngleToAimAt = 90;

        if (!ShouldHitPlayer) WeaponEnabled = false;
    }
    public virtual void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        UpdateWeaponUI();
        UpdateLockOn();
        HandleWeaponSelected();
    }
    public virtual void FixedUpdate()
    {
        if (WeaponEnabled)
        {
            if (IsAimingAtTarget) PointTurretAtTarget();
            else if (!ShouldNotRotate) RotateTurretToAngle();
        }
        else
        {
            StopInteraction();
        }
        UpdateLaserLR();
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
            _maxAllowedAngleToTurn = _weaponStats._maxSwivel / 2f;
            Recoil = _weaponStats._recoil;
            RecoilDuration = _weaponStats._recoilDuration;
        }
        else  //set default stats
        {
            print("No Weapon Stats found, setting defaults!");
            SystemName = "No System Found";
            AttacksPerSecond = 1;
            Damage = 1;
            RotationSpeed = 5f;
            MaxLockOnRange = 100f;
            _maxAllowedAngleToTurn = 180;
            Recoil = 0.05f;
            RecoilDuration = 0.05f;
        }
        TimeBetweenAttacks = 1 / AttacksPerSecond;
        TimeElapsedBetweenLastAttack = TimeBetweenAttacks; //make sure we can fire right away
        InitWeaponSelectedUI();
        _weaponSelectedUI.UpdateWeaponSelected();
        InitLineRenderer();
    }

    public virtual void ManualFire()
    {
        AttemptAttack();
        WeaponSelected = false;
        if (PlayerWepUI) PlayerWepUI.DeselectWeapon();
    }

    public void InitLineRenderer()
    {
        lr = Instantiate(Resources.Load(GS.WeaponPrefabs("WeaponLineRenderer"), typeof(LineRenderer)) as LineRenderer);
        lr.transform.SetParent(transform, false);
        lr.gameObject.SetActive(false);
    }
    private void InitWeaponSelectedUI()
    {
        _weaponSelectedUI = Instantiate(Resources.Load(GS.WeaponPrefabs("WeaponSelectedUI"), typeof(WeaponSelectedUI)) as WeaponSelectedUI);
        _weaponSelectedUI.transform.localPosition = RotatablePart.transform.localPosition;
        _weaponSelectedUI.transform.SetParent(transform, false);
        _weaponSelectedUI.InitWeaponSelectedUI(this);

        //if (PlayerWepUI)
        //{
        //    _weaponHoveringUI = Instantiate(Resources.Load(GS.WeaponPrefabs("WeaponHoveringUI"), typeof(WeaponHoveringUI)) as WeaponHoveringUI);
        //    _weaponHoveringUI.transform.localPosition = RotatablePart.transform.localPosition;
        //    _weaponHoveringUI.transform.SetParent(transform, false);
        //    HM.RotateLocalTransformToAngle(_weaponHoveringUI.transform, new Vector3(0, 0, -90));
        //}
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
    public void SetIndex(int i)
    {
        WeaponUI.WeaponIndex = i;
        WeaponUI._weaponIndexText.text = i.ToString();
    }
    /// <summary>
    /// This Method handles everything to do with the operation of a weapon!
    /// </summary>
    public virtual void HandleWeaponSelected()
    {
        if (WeaponSelected && WeaponEnabled && !ShouldHitPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                AimWithMouse();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                ManualFire();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ResetAim();
            }
            if (_weaponSelectedUI)
            {
                _weaponSelectedUI.UpdateWeaponSelected();
            }
        }
    }

    //  AIMING

    public virtual void AimWithMouse()
    {
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
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            WeaponSelected = false;
            if (PlayerWepUI) PlayerWepUI.DeselectWeapon();
        }
    }
    public virtual void CancelLockOn()
    {
        REF.c.RemoveCrosshair(this);
        IsAimingAtTarget = false;

        TargetedRoom = null;
    }
    public virtual void ResetAim()
    {
        WeaponSelected = false;
        if (PlayerWepUI) PlayerWepUI.DeselectWeapon();
        REF.c.RemoveCrosshair(this);
        IsAimingAtTarget = false;
        TargetedRoom = null;
    }

    //  ROTATE

    public virtual void RotateTurretToAngle()
    {
        TargetedRoom = null;
        float zRotActual;
        float diff = AngleToAimAt - RotatablePart.localRotation.eulerAngles.z;
        if (diff < -180) diff += 360;

        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual = RotatablePart.localRotation.eulerAngles.z + Mathf.Sign(diff) * RotationSpeed;
        }
        else
        {
            zRotActual = AngleToAimAt;
        }
        HM.RotateLocalTransformToAngle(RotatablePart, new Vector3(0, 0, zRotActual));
    }
    public virtual void PointTurretAtTarget()
    {
        if (!TargetedRoom) return;
        Vector3 TargetMoveVector = Vector3.zero;
        UpdateTankSpeedProjectileModifier();

        float distance = Vector3.Distance(TargetedRoom.transform.position, _projectileSpots[0].transform.position);
        float TimeForProjectileToHitDistance = distance / (_weaponStats._projectileSpeed + tankSpeedProjectileModifier * tMov.currentSpeed);

        if (ShouldHitPlayer)
        {
            TargetMoveVector = REF.PCon.TMov.moveVector * REF.PCon.TMov.currentSpeed * TimeForProjectileToHitDistance;
        }
        else
        {
            //Calculate the position where our target will be
            if (TargetedRoom.transform.root.GetComponentInChildren<EnemyTankMovement>())
            {
                EnemyTankMovement mov = TargetedRoom.transform.root.GetComponentInChildren<EnemyTankMovement>();
                TargetMoveVector = mov.moveVector * mov.currentSpeed * TimeForProjectileToHitDistance;
            }
        }

        //  find the desired angle to face the target
        float zRotToTarget = HM.GetEulerAngle2DBetween(transform.InverseTransformPoint(TargetedRoom.transform.position + TargetMoveVector), transform.localPosition);
        float zRotActual = HM.WrapAngle(RotatablePart.localRotation.eulerAngles.z);
        float diff = zRotToTarget - zRotActual;
        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual += Mathf.Sign(diff) * RotationSpeed;
            if (zRotActual > _maxAllowedAngleToTurn) zRotActual = _maxAllowedAngleToTurn;
            else if (zRotActual < -_maxAllowedAngleToTurn) zRotActual = -_maxAllowedAngleToTurn;
        }
        else
        {
            zRotActual = zRotToTarget;
            AttemptAttack();
        }
        HM.RotateLocalTransformToAngle(RotatablePart, new Vector3(0, 0, zRotActual));
        AngleToAimAt = zRotActual;
    }

    //  USE WEAPON

    public virtual void AttemptAttack()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            PlayWeaponFireSoundEffect();
            WeaponFireParticles();
            FireProjectiles();
            if (!ShouldHitPlayer)
            {
                WeaponFeedback();
                //REF.Dialog.FireWeapon();
            }
        }
    }

    public virtual void WeaponFeedback()
    {
        REF.Cam.StartShake(RecoilDuration, Recoil);
    }

    public virtual void FireProjectiles()
    {
        foreach (Transform t in _projectileSpots)
        {
            GameObject proj = ObjectPool.Instance.GetPoolableFromPool(ProjectilePrefab.GetComponent<PoolableObject>()._poolableType);
            proj.GetComponent<AProjectile>().SetBulletStatsAndTransformToWeaponStats(this, t);
            proj.GetComponent<AProjectile>().HitPlayer = ShouldHitPlayer;
            proj.SetActive(true);
            TimeElapsedBetweenLastAttack = 0;
        }
    }

    //  UI
    public virtual void UpdateWeaponUI()
    {
        if (PlayerWepUI)
        {
            PlayerWepUI.SetCharge(Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks), _firingStatus);
            PlayerWepUI.WeaponIsBeingInteractedWith(WeaponEnabled);
        }
        if (ShouldHitPlayer)
        {
            if (WeaponUI) WeaponUI.SetCharge(Mathf.Min(1, TimeElapsedBetweenLastAttack / TimeBetweenAttacks), _firingStatus);
        }
        if (WeaponUI) HM.RotateTransformToAngle(WeaponUI._weaponIndexText.transform, new Vector3(0, 0, 0));
    }
    //  Misc

    public virtual void WeaponFireParticles()
    {
        if (_weaponAnimator) _weaponAnimator.SetTrigger("Fire");

        if (_weaponFireAnimator) _weaponFireAnimator.SetTrigger("Fire");
        else
        {
            GameObject explosionObject = ObjectPool.Instance.GetPoolableFromPool(PoolableObject.PoolableType.DeathExplosion);
            explosionObject.GetComponent<AExplosion>().InitExplosion(transform.position, 1);
            foreach (Transform t in _projectileSpots) explosionObject.transform.SetParent(t);
            explosionObject.transform.localPosition = Vector3.zero;
        }
    }
    public virtual void PlayWeaponFireSoundEffect()
    {
        _weaponAudioSource.pitch = UnityEngine.Random.Range(1 - pitchVariance, 1 + pitchVariance);
        if (_weaponAudioSource) _weaponAudioSource.Play();
        else Debug.LogError("Missing audio clip for weapon!");
    }
    public void UpdateLaserLR()
    {
        if (!lr) return;
        lr.gameObject.SetActive(false);
        if (TargetedRoom && IsAimingAtTarget && ShouldHitPlayer)
        {
            if (TargetRoomIsWithinLockOnRange())
            {
                //DottedLine.DottedLine.Instance.DrawDottedLine(transform.position, TargetedRoom.transform.position, UIColor);

                lr.gameObject.SetActive(true);
                lr.SetPosition(0, RotatablePart.position);
                lr.SetPosition(1, TargetedRoom.transform.position);
                lr.material.color = UIColor;
            }
        }
    }
    public void UpdateLockOn()
    {
        if (!TargetRoomIsWithinLockOnRange() && TargetedRoom)
        {
            CancelLockOn();
            //print("cancelling lock on");
        }
    }
    public bool TargetRoomIsWithinLockOnRange()
    {
        if (!TargetedRoom) return false;
        return Vector3.Distance(_projectileSpots[0].position, TargetedRoom.transform.position) < MaxLockOnRange;
    }
    public void UpdateTankSpeedProjectileModifier()
    {
        float tankRotation = tMov.GetComponent<TankRotation>().transform.rotation.eulerAngles.z + 90;
        tankSpeedProjectileModifier = 0;
        if (tankRotation > 180) tankRotation -= 360;
        if (AngleToAimAt > 180) AngleToAimAt -= 360;
        float angle = tankRotation - AngleToAimAt;
        angle = Mathf.Abs(angle);
        angle = Mathf.Min(90, angle);
        tankSpeedProjectileModifier = angle / 90;
        tankSpeedProjectileModifier = 1 - tankSpeedProjectileModifier;

        //  Minimum of 0
        tankSpeedProjectileModifier = Mathf.Max(0, tankSpeedProjectileModifier);
    }
}