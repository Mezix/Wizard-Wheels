using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTankRotation : MonoBehaviour
{
    public float rotationspeed = 50f;

    public Transform tankRotation;
    private List<GameObject> rotatableObjects = new List<GameObject>();
    float AngleToRotateTo;

    void Start()
    {
        InitRotatableObjects();
        rotatableObjects.Add(tankRotation.gameObject);
    }
    private void InitRotatableObjects()
    {
        foreach (Tire t in GetComponentsInChildren<Tire>()) rotatableObjects.Add(t.gameObject);
    }

    //  Rotate Tank Manually using the arrow keys
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
        float currentRot = tankRotation.rotation.eulerAngles.z;
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

    private void RotateAllObjectsToRotation(float zRot)
    {
        foreach (GameObject rotatable in rotatableObjects)
        {
            HM.RotateTransformToAngle(rotatable.transform, new Vector3(0, 0, AngleToRotateTo));
        }
    }
    private void RotateAllObjectsByRotation(float zRot)
    {
        foreach (GameObject rotatable in rotatableObjects)
        {
            rotatable.transform.Rotate(Vector3.forward * zRot);
        }
    }
}
