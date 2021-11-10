using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (col.transform.root.tag == "Enemy" && !HitPlayer)
            {
                DamageEnemy(col.transform.root.GetComponentInChildren<IEnemy>());
                hasDoneDamage = true;
            }
            if (col.transform.root.tag == "Player" && HitPlayer)
            {
                DamagePlayer();
                hasDoneDamage = true;
            }
        }
    }
    public override IEnumerator DespawnAnimation()
    {
        despawnAnimationPlaying = true;
        _shadow.SetActive(false);
        _projectileSprite.gameObject.SetActive(false);
        GameObject explosion = Instantiate((GameObject)Resources.Load("SingleExplosion"));
        explosion.transform.position = transform.position;
        yield return new WaitForSeconds(0.43f);
        Destroy(explosion);
        DespawnBullet();
    }
}
