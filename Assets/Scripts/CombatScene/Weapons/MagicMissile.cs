using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : AWeapon
{
    public override void Start()
    {
        base.Start();
        ProjectilePrefab = Resources.Load(GS.WeaponPrefabs("MagicMissileProjectilePrefab"), typeof(GameObject)) as GameObject;
    }
    public override void SpawnProjectile()
    {
        foreach (Transform t in _projectileSpots)
        {
            GameObject proj = ObjectPool.Instance.GetProjectileFromPool(ProjectilePrefab.GetComponent<PoolableObject>()._poolableType);
            Debug.Log(proj);
            proj.GetComponent<MagicMissileProjectile>().SetBulletStatsAndTransformToWeaponStats(this, t);
            proj.GetComponent<MagicMissileProjectile>().HitPlayer = ShouldHitPlayer;
            proj.GetComponent<MagicMissileProjectile>().targetToSeek = TargetedRoom;
            proj.SetActive(true);
            TimeElapsedBetweenLastAttack = 0;
        }
    }
}
