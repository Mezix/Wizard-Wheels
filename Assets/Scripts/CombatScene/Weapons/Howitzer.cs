using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Howitzer : AWeapon
{
    public GroundTarget _groundTarget;
    public override void Start()
    {
        base.Start();
        ProjectilePrefab = Resources.Load(GS.WeaponPrefabs("HowitzerBallPrefab"), typeof(GameObject)) as GameObject;
    }
    public override void Update()
    {
        TimeElapsedBetweenLastAttack += Time.deltaTime;
        UpdateWeaponUI();
        UpdateLockOn();
        HandleWeaponSelected();
        if(_groundTarget)
        {
            Vector2 targetedPos = _groundTarget.transform.position;
            float angle = 180 + HM.GetEulerAngle2DBetween(transform.position, targetedPos);

            if (Vector2.Distance(transform.position, targetedPos) > MaxLockOnRange)
            {
                _groundTarget.transform.position = transform.position + HM.Get2DCartesianFromPolar(angle, MaxLockOnRange);
            }
        }
    }
    public override void AimWithMouse()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = 180 + HM.GetEulerAngle2DBetween(transform.position, worldPos);

        if (_groundTarget) Destroy(_groundTarget.gameObject);

        if (Vector2.Distance(transform.position, worldPos) > MaxLockOnRange)
        {
            worldPos = HM.Get2DCartesianFromPolar(angle, MaxLockOnRange);
        }

        _groundTarget = Instantiate(Resources.Load(GS.WeaponPrefabs("GroundTarget"), typeof (GroundTarget)) as GroundTarget, worldPos, Quaternion.identity);
        _groundTarget._assignedWeapon = this;

        WeaponSelected = false;
        if (PlayerWepUI) PlayerWepUI.DeselectWeapon();
    }
    public override void RotateTurretToAngle()
    {
        if (!_groundTarget) return;
        HM.RotateTransformToAngle(RotatablePart, new Vector3(0, 0, 180 + HM.GetEulerAngle2DBetween(transform.position, _groundTarget.transform.position)));
    }
    public override void FireProjectiles()
    {
        foreach (Transform t in _projectileSpots)
        {
            GameObject proj = ObjectPool.Instance.GetPoolableFromPool(ProjectilePrefab.GetComponent<PoolableObject>()._poolableType);
            proj.GetComponent<HowitzerProjectile>().SetBulletStatsAndTransformToWeaponStats(this, t);
            proj.GetComponent<HowitzerProjectile>().HitPlayer = ShouldHitPlayer;
            proj.GetComponent<HowitzerProjectile>()._groundTarget = _groundTarget;
            _groundTarget = null;
            proj.SetActive(true);
            TimeElapsedBetweenLastAttack = 0;
        }
    }
}
