using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowitzerProjectile : AProjectile
{
    public List<Room> _allRoomsInExplosion = new List<Room>();
    public DetonationStatus detonationStatus;
    public bool detonationTriggerStarted = false;
    public float detonationTriggerLength = 0.2f;
    public GroundTarget _groundTarget;
    public enum DetonationStatus
    {
        Flying,
        DetonationReady
    }

    public override void OnEnable()
    {
        base.OnEnable();

        detonationStatus = DetonationStatus.Flying;
        _allRoomsInExplosion.Clear();
        MaxLifetime = 2f; // = Travel Time!
        detonationTriggerStarted = false;
        _projectileSprite.transform.localScale = Vector3.one;
    }
    public override void FixedUpdate()
    {
        if (!despawnAnimationPlaying) MoveProjectile();
    }
    public override void MoveProjectile()
    {
        transform.position = Vector3.Lerp(transform.position, _groundTarget.transform.position, Mathf.Min(1, Time.deltaTime * CurrentLifeTime/MaxLifetime));
        UpdateShadowPosition();
    }
    public override void CheckLifetime() //a function that checks if our projectile has reached the end of its lifespan, and then decides what to do now
    {
        float opacity;
        float opacityFactor = 0.5f;
        float scaleFactor = 5;
        if (CurrentLifeTime < MaxLifetime/2f)
        {
            opacity = Mathf.Max(0.5f, 1 - CurrentLifeTime / MaxLifetime);
            _projectileSprite.color = new Color(1,1,1, opacity * opacityFactor);
            _projectileSprite.transform.localScale = Vector3.one * (1 + scaleFactor * (CurrentLifeTime / MaxLifetime));
        }
        else if(CurrentLifeTime < MaxLifetime)
        {
            opacity = Mathf.Min(1, CurrentLifeTime / MaxLifetime);
            _projectileSprite.color = new Color(1, 1, 1, opacity * opacityFactor);
            _projectileSprite.transform.localScale = Vector3.one * (1 + (scaleFactor * (1 - CurrentLifeTime / MaxLifetime)));
        }
        else
        {
            opacity = 1;
            _projectileSprite.color = new Color(1, 1, 1, opacity);
            _projectileSprite.transform.localScale = Vector3.one;
            detonationStatus = DetonationStatus.DetonationReady;
        }
        if(CurrentLifeTime > 60)
        {
            StartCoroutine(Detonate());
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Room"))
        {
            TankController tank = collision.transform.root.GetComponentInChildren<TankController>();
            if (tank)
            {
                if (tank.Equals(wep.tMov.GetComponent<TankController>()))
                {
                    //Debug.Log("Hit Self, returning");
                }
                else
                {
                    if (detonationStatus.Equals(DetonationStatus.DetonationReady))
                    {
                        if(!detonationTriggerStarted) StartCoroutine(Detonate());
                    }
                    //  Add all rooms to the list for later detonation
                    if(!hasDoneDamage)
                    {
                        Room r = collision.GetComponent<Room>();
                        if (HitPlayer)
                        {
                            tank = tank.GetComponent<PlayerTankController>();
                            if (tank)
                            {
                                _allRoomsInExplosion.Add(r);
                                Debug.Log(r);
                            }
                        }
                        else
                        {
                            tank = tank.GetComponent<EnemyTankController>();
                            if (tank)
                            {
                                _allRoomsInExplosion.Add(r);
                                Debug.Log(r);
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator Detonate()
    {
        detonationTriggerStarted = true;
        yield return new WaitForSeconds(detonationTriggerLength);
        hasDoneDamage = true;
        Debug.Log(_allRoomsInExplosion.Count);
        foreach (Room r in _allRoomsInExplosion)
        {
            r.DamageRoom(Damage);
            DamageVehicle(r._tGeo.GetComponent<TankController>());
        }
        StartCoroutine(DespawnAnimation());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Room"))
        {
            TankController tank = collision.transform.root.GetComponentInChildren<TankController>();
            if (tank)
            {
                Room r = collision.GetComponent<Room>();
                if(_allRoomsInExplosion.Contains(r)) _allRoomsInExplosion.Remove(r);
            }
        }
    }
    public override void SetBulletStatsAndTransformToWeaponStats(AWeapon weapon, Transform startPositionTransform)
    {
        wep = weapon;
        Damage = weapon._weaponStats._damage;
        ProjectileSpeed = weapon._weaponStats._projectileSpeed;
        if (weapon.tMov) ProjectileSpeed += weapon.tankSpeedProjectileModifier * weapon.tMov.currentSpeed;
        transform.position = startPositionTransform.transform.position;
        transform.rotation = startPositionTransform.transform.rotation;
        if (trail) trail.Clear();
    }
    public override IEnumerator DespawnAnimation()
    {
        Destroy(_groundTarget);
        float pitchVariance = 0.1f;
        AudioManager.Singleton._cannonImpactSound.pitch = UnityEngine.Random.Range(0.5f - pitchVariance, 0.5f + pitchVariance);
        AudioManager.Singleton._cannonImpactSound.Play();

        despawnAnimationPlaying = true;
        if (_shadow) _shadow.SetActive(false);
        _projectileSprite.gameObject.SetActive(false);
        GameObject explosionObject = ObjectPool.Instance.GetPoolableFromPool(_projectileExplosionType);
        explosionObject.GetComponent<AExplosion>().InitExplosion(transform.position, _explosionDiameter);
        yield return new WaitForFixedUpdate();
        DespawnProjectile();
    }
}
