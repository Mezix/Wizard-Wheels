using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cannonball : AProjectile
{
    public Room _firstRoomHit = null;
    public override void OnEnable()
    {
        base.OnEnable();
        _firstRoomHit = null;
    }
    private void FixedUpdate()
    {
        if (!despawnAnimationPlaying) MoveProjectile();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!hasDoneDamage)
        {
            if(col.tag == "ProjectileObstacle")
            {
                StartCoroutine(DespawnAnimation());
                return;
            }
            if(col.gameObject.layer == LayerMask.NameToLayer("Room"))
            {
                TankController tank = col.transform.root.GetComponentInChildren<TankController>();
                if (tank)
                {
                    if(tank.Equals(wep.tMov.GetComponent<TankController>()))
                    {
                        //Debug.Log("Hit Self, returning");
                    }
                    else
                    {
                        Room r = col.GetComponent<Room>();
                        if (r)
                        {
                            if (_firstRoomHit == null)
                            {
                                _firstRoomHit = r;
                                //Debug.Log("Damaging Room");
                                r.DamageRoom(Damage);
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
    public override IEnumerator DespawnAnimation()
    {
        float pitchVariance = 0.1f;
        AudioManager.Singleton._cannonImpactSound.pitch = UnityEngine.Random.Range(0.5f - pitchVariance, 0.5f + pitchVariance);
        AudioManager.Singleton._cannonImpactSound.Play();

        despawnAnimationPlaying = true;
        _shadow.SetActive(false);
        _projectileSprite.gameObject.SetActive(false);
        GameObject explosion = Instantiate((GameObject) Resources.Load(GS.Effects("SingleExplosion")));
        explosion.transform.position = transform.position;
        yield return new WaitForFixedUpdate();
        DespawnBullet();
    }
}
