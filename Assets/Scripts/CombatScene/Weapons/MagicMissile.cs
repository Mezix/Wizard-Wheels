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
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = 180 + HM.GetEulerAngle2DBetween(transform.position, worldPos);
        worldVectorToAimAt = angle;

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            WeaponSelected = false;
            if (PlayerWepUI) PlayerWepUI.DeselectWeapon();
        }
    }
    public override void AttemptAttack()
    {
        if (TimeElapsedBetweenLastAttack >= TimeBetweenAttacks)
        {
            FireProjectiles();
        }
    }
    public override void RotateTurretToAngle()
    {
        //Does nothing, but must override the method because this weapon functions differently 
    }
    public override void WeaponFireParticles()
    {
        //Does nothing, but must override the method because this weapon functions differently 
    }
    public override void FireProjectiles()
    {
        StartCoroutine(FireAllMissiles());
    }
    public IEnumerator FireAllMissiles()
    {
        isFiring = true;
        TimeElapsedBetweenLastAttack = 0;
        foreach (Transform projectileSpot in _projectileSpots)
        {
            PlayWeaponFireSoundEffect();
            GameObject proj = ObjectPool.Instance.GetPoolableFromPool(ProjectilePrefab.GetComponent<PoolableObject>()._poolableType);
            proj.GetComponent<MagicMissileProjectile>().SetBulletStatsAndTransformToWeaponStats(this, projectileSpot);
            proj.GetComponent<MagicMissileProjectile>().HitPlayer = ShouldHitPlayer;
            proj.SetActive(true);

            GameObject explosionObject = ObjectPool.Instance.GetPoolableFromPool(PoolableObject.PoolableType.MagicExplosion);
            explosionObject.GetComponent<AExplosion>().InitExplosion(projectileSpot.position, 0.25f);

            if (!ShouldHitPlayer)
            {
                WeaponFeedback();
                //REF.Dialog.FireWeapon();
            }

            yield return new WaitForSeconds(1/3f); // fire all 6 rockets in 2 seconds
        }
        isFiring = false;
    }
}
