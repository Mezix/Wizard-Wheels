using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : AWeapon
{
    public bool isFiring = false;
    public float worldVectorToAimAt;
    public override void Start()
    {
        base.Start();
        ProjectilePrefab = Resources.Load(GS.WeaponPrefabs("MagicMissileProjectilePrefab"), typeof(GameObject)) as GameObject;
    }
    public override void Update()
    {
        if(!isFiring) TimeElapsedBetweenLastAttack += Time.deltaTime;
        UpdateWeaponUI();
        UpdateLockOn();
        HandleWeaponSelected();
        foreach (Transform t in _projectileSpots)
        {
            HM.RotateTransformToAngle(t, new Vector3(0, 0, worldVectorToAimAt));
        }
    }
    public override void AimWithMouse()
    {
        WeaponSelected = false;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = 180 + HM.GetEulerAngle2DBetween(transform.position, worldPos);
        worldVectorToAimAt = angle;
    }
    public override void RotateTurretToAngle()
    {
    }
    public override void FireProjectiles()
    {
        StartCoroutine(FireAllMissiles());
    }
    public IEnumerator FireAllMissiles()
    {
        isFiring = true;
        TimeElapsedBetweenLastAttack = 0;
        foreach (Transform t in _projectileSpots)
        {
            GameObject proj = ObjectPool.Instance.GetProjectileFromPool(ProjectilePrefab.GetComponent<PoolableObject>()._poolableType);
            proj.GetComponent<MagicMissileProjectile>().SetBulletStatsAndTransformToWeaponStats(this, t);
            proj.GetComponent<MagicMissileProjectile>().HitPlayer = ShouldHitPlayer;
            proj.SetActive(true);
            yield return new WaitForSeconds(1/3f); // fire all 6 rockets in 2 seconds
        }
        isFiring = false;
    }
}
