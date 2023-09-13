using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileProjectile : AProjectile
{
    private GameObject targetToSeek;
    private void Start()
    {
        MaxLifetime = 6f;
    }

    public override void MoveProjectile()
    {
        if(targetToSeek)
        {
            Vector3 angleTowardsRoom = new Vector3(0, 0, HM.GetEulerAngle2DBetween(transform.position, targetToSeek.transform.position));

            //  TODO: Rotate towards target slowly instead of facing it immediately
            HM.RotateTransformToAngle(transform, angleTowardsRoom);
        }
        rb.MovePosition(transform.position - transform.right * (Mathf.Max(3, ProjectileSpeed * CurrentLifeTime/MaxLifetime)) * Time.deltaTime);
    }
}
