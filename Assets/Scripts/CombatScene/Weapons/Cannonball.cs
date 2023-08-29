﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cannonball : AProjectile
{
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
            TankController tank = col.transform.root.GetComponentInChildren<TankController>();
            if(tank)
            {
                if(col.TryGetComponent(out Room r))
                {
                    r.DamageRoom(Damage);
                }
                if(HitPlayer)
                {
                    if (tank.GetComponent<PlayerTankController>())
                    {
                        DamageTank(tank);
                        hasDoneDamage = true;
                    }
                }
                else
                {
                    if (tank.GetComponent<EnemyTankController>())
                    {
                        DamageTank(tank);
                        hasDoneDamage = true;
                    }
                }
            }
        }
    }
    public override IEnumerator DespawnAnimation()
    {
        despawnAnimationPlaying = true;
        _shadow.SetActive(false);
        _projectileSprite.gameObject.SetActive(false);
        GameObject explosion = Instantiate((GameObject)Resources.Load(GS.Effects("SingleExplosion")));
        explosion.transform.position = transform.position;
        yield return new WaitForFixedUpdate();
        DespawnBullet();
    }
}
