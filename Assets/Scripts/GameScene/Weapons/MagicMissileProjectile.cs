using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileProjectile : AProjectile
{
    private GameObject target;
    private void FixedUpdate()
    {
        if (!despawnAnimationPlaying) MoveProjectile();
        UpdateShadowPosition();
    }
    private void Start()
    {
        MaxLifetime = 6f;
    }

    public override void MoveProjectile()
    {
        if(target)
        {
            Vector3 angleTowardsRoom = new Vector3(0, 0, HM.GetAngle2DBetween(transform.position, target.transform.position));

            //  TODO: Rotate towards target slowly instead of facing it immediately
            HM.RotateTransformToAngle(transform, angleTowardsRoom);
        }
        rb.MovePosition(transform.position - transform.right * ProjectileSpeed * Time.deltaTime);
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
        //GameObject explosion = Instantiate((GameObject)Resources.Load("SingleExplosion"));
        //explosion.transform.position = transform.position;
        yield return new WaitForSeconds(0.05f);
        //Destroy(explosion);
        DespawnBullet();
    }
    public override void SetBulletStatsAndTransformToWeaponStats(AWeapon weapon)
    {
        target = weapon.Room;
        Damage = weapon._weaponStats._damage;
        ProjectileSpeed = weapon._weaponStats._projectileSpeed;
        transform.position = weapon.transform.position;
        transform.rotation = weapon.transform.rotation;
    }
}
