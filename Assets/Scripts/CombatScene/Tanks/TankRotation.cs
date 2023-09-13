using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankRotation : MonoBehaviour
{
    [HideInInspector]
    public float rotationspeed;
    public float AngleToRotateTo;

    protected void RotateVehicle(float zRot)
    {
        HM.RotateTransformToAngle(transform, new Vector3(0, 0, zRot));
    }
    protected void RotateVehicleByRotation(float zRot)
    {
        transform.Rotate(Vector3.forward * zRot);
    }

    public void InitRotationSpeed()
    {
        if (!TryGetComponent(out TankController tc)) return;
        if (tc._vehicleInfo != null)
        {
            rotationspeed = tc._vehicleInfo.RotationSpeed;
        }
        else
        {
            rotationspeed = 50f;
        }
    }
}
