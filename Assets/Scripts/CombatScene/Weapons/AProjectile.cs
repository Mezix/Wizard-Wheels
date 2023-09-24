using System;
using System.Collections;
using UnityEngine;

public abstract class AProjectile : MonoBehaviour //the interface for all projectiles fired from ranged Weapons
{
    public float CurrentLifeTime { get; protected set; } //Check IProjectile for explanations
    public float MaxLifetime { get; set; }
    public int Damage { get; set; }
    public float ProjectileSpeed { get; set; }
    public bool HitPlayer { get; set; }
    public TrailRenderer trail;

    protected Rigidbody2D rb;
    [SerializeField]
    protected SpriteRenderer _projectileSprite;
    protected bool despawnAnimationPlaying;
    protected bool hasDoneDamage;

     //  Shadow
     [SerializeField]
    protected GameObject _shadow;
    [SerializeField]
    protected Transform _shadowRotation;
    [SerializeField]
    protected float maxShadowHeight;

    public AWeapon wep;
    public const int IgnoreProjectilesLayer = 12;
    public Room _firstRoomHit = null;

    public PoolableObject.PoolableType _projectileExplosionType;
    public float _explosionDiameter = 1;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        MaxLifetime = 3;
    }
    public virtual void OnEnable()
    {
        CurrentLifeTime = 0;
        despawnAnimationPlaying = false;
        hasDoneDamage = false;
        if(_shadow)
        {
            _shadow.transform.localPosition = new Vector2(0, -maxShadowHeight);
            _shadow.SetActive(true);
        }
        _projectileSprite.gameObject.SetActive(true);
        _firstRoomHit = null;
    }
    public virtual void Update()
    {
        CurrentLifeTime += Time.deltaTime;
        CheckLifetime();
    }
    public virtual void FixedUpdate()
    {
        if (!despawnAnimationPlaying) MoveProjectile();
    }
    public virtual void MoveProjectile()
    {
        rb.MovePosition(transform.position + transform.right * ProjectileSpeed * Time.deltaTime);
        UpdateShadowPosition();
    }
    public virtual void UpdateShadowPosition()
    {
        if (!_shadow) return;
        HM.RotateLocalTransformToAngle(_shadowRotation, -transform.rotation.eulerAngles);
        _shadow.transform.localPosition = new Vector2(0, -maxShadowHeight * (1f - CurrentLifeTime / MaxLifetime));
        HM.RotateLocalTransformToAngle(_shadowRotation.GetChild(0), transform.rotation.eulerAngles - new Vector3(0,0,90));
    }
    public virtual void CheckLifetime() //a function that checks if our projectile has reached the end of its lifespan, and then decides what to do now
    {
        if (CurrentLifeTime >= MaxLifetime && !despawnAnimationPlaying)
        {
            StartCoroutine(DespawnAnimation());
        }
    }
    protected void DespawnProjectile()
    {
        ObjectPool.Instance.AddToPool(GetComponent<PoolableObject>());
    }
    public virtual void SetBulletStatsAndTransformToWeaponStats(AWeapon weapon, Transform t)
    {
        wep = weapon;
        Damage = weapon._weaponStats._damage;
        ProjectileSpeed = weapon._weaponStats._projectileSpeed;
        if (weapon.tMov) ProjectileSpeed += weapon.tankSpeedProjectileModifier * weapon.tMov.currentSpeed;
        transform.position = t.transform.position;
        transform.rotation = t.transform.rotation;
        if (trail) trail.Clear();
    }
    public virtual void DamageVehicle(TankController vehicle)
    {
        REF.TM.TriggerHitStop(0.5f, 0.05f, 0.1f, 0.1f);
        vehicle.TakeDamage(Damage);
        StartCoroutine(DespawnAnimation());
    }
    public virtual IEnumerator DespawnAnimation()
    {
        float pitchVariance = 0.1f;
        AudioManager.Singleton._cannonImpactSound.pitch = UnityEngine.Random.Range(0.5f - pitchVariance, 0.5f + pitchVariance);
        AudioManager.Singleton._cannonImpactSound.Play();

        despawnAnimationPlaying = true;
        if(_shadow)_shadow.SetActive(false);
        _projectileSprite.gameObject.SetActive(false);
        GameObject explosionObject = ObjectPool.Instance.GetPoolableFromPool(_projectileExplosionType);
        explosionObject.GetComponent<AExplosion>().InitExplosion(transform.position, _explosionDiameter);
        yield return new WaitForFixedUpdate();
        DespawnProjectile();
    }
    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (!hasDoneDamage)
        {
            if (col.tag == "ProjectileObstacle")
            {
                StartCoroutine(DespawnAnimation());
                return;
            }
            if (col.gameObject.layer == LayerMask.NameToLayer("Room"))
            {
                TankController tank = col.transform.root.GetComponentInChildren<TankController>();
                if (tank)
                {
                    if (tank.Equals(wep.tMov.GetComponent<TankController>()))
                    {
                        //Debug.Log("Hit Self, returning");
                    }
                    else
                    {
                        Room roomToHit = col.GetComponent<Room>();
                        if (roomToHit)
                        {
                            if (_firstRoomHit == null)
                            {
                                _firstRoomHit = roomToHit;
                                Vector3 ProjectileRotationInRoomLocalSpace = roomToHit.transform.InverseTransformDirection(transform.rotation.eulerAngles);
                                ProjectileRotationInRoomLocalSpace = new Vector3(0, 0, HM.WrapAngle(90 + ProjectileRotationInRoomLocalSpace.z));
                                RoomPosition.DamageDirection dir = GetDamageDirection(ProjectileRotationInRoomLocalSpace);
                                roomToHit.DamageRoom(Damage, dir);
                            }
                        }
                        if (HitPlayer)
                        {
                            tank = tank.GetComponent<PlayerTankController>();
                            if (tank)
                            {
                                //Debug.Log("Damaging Player");
                                DamageVehicle(tank);
                                hasDoneDamage = true;
                            }
                        }
                        else
                        {
                            tank = tank.GetComponent<EnemyTankController>();
                            if (tank)
                            {
                                //Debug.Log("Damaging Enemy");
                                DamageVehicle(tank);
                                hasDoneDamage = true;
                            }
                        }
                    }
                }
            }
        }
    }

    private RoomPosition.DamageDirection GetDamageDirection(Vector3 vectorBetweenRoomAndProjectile)
    {
        if (vectorBetweenRoomAndProjectile.z >= 0 && vectorBetweenRoomAndProjectile.z < 90)
        {
            return RoomPosition.DamageDirection.Up;
        }
        else if(vectorBetweenRoomAndProjectile.z >= 90 && vectorBetweenRoomAndProjectile.z < 180)
        {
            return RoomPosition.DamageDirection.Right;
        }
        else if(vectorBetweenRoomAndProjectile.z < 0 && vectorBetweenRoomAndProjectile.z > -90)
        {
            return RoomPosition.DamageDirection.Left;
        }
        else
        {
            return RoomPosition.DamageDirection.Down;
        }
    }
}
