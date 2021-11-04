using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTankRotation : TankRotation
{
    void Start()
    {
        InitRotatableObjects();
    }
    public void RotateTankLeft()
    {
        RotateAllObjectsByRotation(rotationspeed * Time.deltaTime);
    }
    public void RotateTankRight()
    {
        RotateAllObjectsByRotation(-rotationspeed * Time.deltaTime);
    }

    //  Rotate Tank
    private void RotateTankToAngle()
    {
        float currentRot = rotatableObjects[0].transform.rotation.eulerAngles.z;
        if (currentRot > 180) currentRot -= 360;
        if (AngleToRotateTo > 180) AngleToRotateTo -= 360;

        float difference = currentRot - AngleToRotateTo;

        if (difference > 0)
        {
            if (Mathf.Abs(difference) < (rotationspeed * Time.deltaTime))
            {
                RotateAllObjectsToRotation(AngleToRotateTo);
                HM.RotateTransformToAngle(Ref.UI._steeringWheel.transform, new Vector3(0, 0, AngleToRotateTo));
            }
            else
            {
                RotateAllObjectsByRotation(-rotationspeed * Time.deltaTime);
            }
        }
        if (difference < 0)
        {
            if (Mathf.Abs(difference) < (rotationspeed * Time.deltaTime))
            {
                RotateAllObjectsToRotation(AngleToRotateTo);
                HM.RotateTransformToAngle(Ref.UI._steeringWheel.transform, new Vector3(0, 0, AngleToRotateTo));
            }
            else
            {
                RotateAllObjectsByRotation(rotationspeed * Time.deltaTime);
            }
        }
    }

}
