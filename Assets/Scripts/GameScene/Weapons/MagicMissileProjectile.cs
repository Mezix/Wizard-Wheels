using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileProjectile : AProjectile
{
    private GameObject target;
    private void FixedUpdate()
    {
        if (!despawnAnimationPlaying) MoveProjectile();
        //UpdateShadowPosition();
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
        rb.MovePosition(transform.position - transform.right * (Mathf.Max(3, ProjectileSpeed * CurrentLifeTime/MaxLifetime)) * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!hasDoneDamage)
        {
            TankController tank = col.transform.root.GetComponentInChildren<TankController>();
            if (tank)
            {
                if (HitPlayer)
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
        yield return new WaitForSeconds(0.05f);
        DespawnBullet();
    }
    public override void SetBulletStatsAndTransformToWeaponStats(AWeapon weapon, Transform t)
    {
        target = weapon.TargetedRoom;
        Damage = weapon._weaponStats._damage;
        ProjectileSpeed = weapon._weaponStats._projectileSpeed;
        transform.position = t.position;
        transform.rotation = t.rotation;
        trail.Clear();
    }
}
