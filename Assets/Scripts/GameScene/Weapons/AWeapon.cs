using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AWeapon : MonoBehaviour, ISystem
{
    [SerializeField]
    private List<SpriteRenderer> _weaponSprites;

    public GameObject SystemObj { get; set; }
    public string SystemName { get; set; }
    public RoomPosition RoomPosForInteraction { get; set; }
    public bool IsBeingInteractedWith { get; set; }
    public Sprite SystemSprite { get; set; }

    //  Stats

    public WeaponStats _weaponStats;
    public float AttacksPerSecond { get; set; }
    public float TimeBetweenAttacks { get; private set; }
    public float TimeElapsedBetweenLastAttack { get; protected set; }
    public float Damage { get; set; }
    public float RotationSpeed { get; set; }
    public float MaxLockOnRange { get; set; }

    //  Aiming

    public GameObject Room;
    public bool WeaponSelected { get; set; }
    public bool WeaponEnabled { get; set; }
    public bool AimAtTarget { get; set; }
    public float AimRotationAngle { get; set; }
    public bool ShouldNotRotate { get; set; }

    //  Misc

    public GameObject ProjectilePrefab { get; set; }
    public Transform _projectileSpot;
    protected LineRenderer laserLR;
    public bool ShouldHitPlayer { get; set; }

    //  UI
    public UIWeapon PlayerUIWep { get; set; }
    public WeaponUI EnemyWepUI;

    //  Audio
    public AudioSource _weaponAudioSource = null;

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
                sprite.color = new Color(1, 1, 1, 0.5f);
            }
            //print("set weapon to transparent");
        }
        else
        {
            foreach (SpriteRenderer sprite in _weaponSprites)
            {
                sprite.color = new Color(1, 1, 1, 1);
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
        if (Room) Ref.c.RemoveCrosshair(GetComponent<AWeapon>());

        RaycastHit2D hit = HM.RaycastToMouseCursor();
        if (hit.collider)
        {
            TankController tc = hit.collider.transform.root.GetComponentInChildren<TankController>();
            if (tc && hit.collider.transform.GetComponent<Room>())
            {
                Room = hit.collider.gameObject;
                if (!(tc._dying || tc._dead))
                {
                    if (TargetRoomWithinLockOnRange())
                    {
                        Ref.c.AddCrosshair(Room.GetComponentInChildren<Room>(), GetComponent<AWeapon>());
                        AimAtTarget = true;
                    }
                    else
                    {
                        //TODO: give a warning that the target is out of range on the screen!
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
    }
    public void CancelAim()
    {
        TryGetComponent(out AWeapon iwep);
        if (iwep == null) return;
        Ref.c.RemoveCrosshair(iwep);
        AimAtTarget = false;
        Room = null;
    }
    public void ResetAim()
    {
        TryGetComponent(out AWeapon iwep);
        if (iwep == null) return;
        Ref.c.RemoveCrosshair(iwep);
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
        float distance = Vector3.Distance(Room.transform.position, _projectileSpot.transform.position);
        float TimeForProjectileToHitDistance = distance / (_weaponStats._projectileSpeed);

        //Calculate the position where our target will be
        if (Room.transform.root.GetComponentInChildren<EnemyTankMovement>())
        {
            EnemyTankMovement mov = Room.transform.root.GetComponentInChildren<EnemyTankMovement>();
            TargetMoveVector = mov.moveVector * mov.currentSpeed * TimeForProjectileToHitDistance;
        }

        //  find the desired angle to face the target
        float zRotToTarget = HM.GetAngle2DBetween(Room.transform.position + TargetMoveVector, transform.position);
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
        CounterRotateUI();
    }
    public void CounterRotateUI()
    {
        HM.RotateLocalTransformToAngle(EnemyWepUI.transform, new Vector3(0, 0, -transform.localRotation.eulerAngles.z));
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
        if (Room && AimAtTarget && ShouldHitPlayer)
        {
            if (TargetRoomWithinLockOnRange())
            {
                laserLR.gameObject.SetActive(true);
                //float distance = Vector3.Distance(Room.transform.position, _cannonballSpot.transform.position);
                laserLR.SetPosition(0, _projectileSpot.transform.position);
                laserLR.SetPosition(1, Room.transform.position);
            }
        }
        else laserLR.gameObject.SetActive(false);

        //  if we can fire at the target, turn the laser green
        //  else keep the laser red
    }
    public void UpdateLockOn()
    {
        if (!TargetRoomWithinLockOnRange() && Room)
        {
            CancelAim();
            //print("cancelling lock on");
        }
    }
    public bool TargetRoomWithinLockOnRange()
    {
        if (!Room) return false;
        return Vector3.Distance(_projectileSpot.position, Room.transform.position) < MaxLockOnRange;
    }
}
