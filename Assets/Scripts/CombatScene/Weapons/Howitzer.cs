using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Howitzer : AWeapon
{
    public GroundTarget TargetedPositionObject;
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
        if(TargetedPositionObject)
        {
            Vector2 targetedPos = TargetedPositionObject.transform.position;
            float angle = 180 + HM.GetEulerAngle2DBetween(transform.position, targetedPos);

            if (Vector2.Distance(transform.position, targetedPos) > MaxLockOnRange)
            {
                TargetedPositionObject.transform.position = transform.position + HM.Get2DCartesianFromPolar(angle, MaxLockOnRange);
            }
        }
    }
    public override void AimWithMouse()
    {
        WeaponSelected = false;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = 180 + HM.GetEulerAngle2DBetween(transform.position, worldPos);

        if (TargetedPositionObject) Destroy(TargetedPositionObject.gameObject);

        if (Vector2.Distance(transform.position, worldPos) > MaxLockOnRange)
        {
            worldPos = HM.Get2DCartesianFromPolar(angle, MaxLockOnRange);
        }

        TargetedPositionObject = Instantiate(Resources.Load(GS.WeaponPrefabs("GroundTarget"), typeof (GroundTarget)) as GroundTarget, worldPos, Quaternion.identity);
        TargetedPositionObject._assignedWeapon = this;
        //HM.RotateLocalTransformToAngle(RotatablePart, new Vector3(0, 0, AngleToAimAt));
    }
    public override void RotateTurretToAngle()
    {
        if (!TargetedPositionObject) return;
        HM.RotateTransformToAngle(RotatablePart, new Vector3(0, 0, 180 + HM.GetEulerAngle2DBetween(transform.position, TargetedPositionObject.transform.position)));
    }
}
