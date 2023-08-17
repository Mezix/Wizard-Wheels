using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankRotation : MonoBehaviour
{
    public List<GameObject> rotatableObjects = new List<GameObject>();
    [HideInInspector]
    public float rotationspeed;
    public float AngleToRotateTo;

    protected void RotateAllObjectsToRotation(float zRot)
    {/*
        if (rotatableObjects.Count == 0) return;
        foreach (GameObject rotatable in rotatableObjects)
        {
            HM.RotateTransformToAngle(rotatable.transform, new Vector3(0, 0, zRot));
        }*/
        HM.RotateTransformToAngle(transform, new Vector3(0, 0, zRot));
    }
    protected void RotateAllObjectsByRotation(float zRot)
    {/*
        if (rotatableObjects.Count == 0) return;
        foreach (GameObject rotatable in rotatableObjects)
        {
            rotatable.transform.Rotate(Vector3.forward * zRot);
        }*/
        transform.Rotate(Vector3.forward * zRot);
    }

    public void InitRotationSpeed()
    {
        if (!TryGetComponent(out TankController tc)) return;
        if (tc._tStats)
        {
            rotationspeed = tc._tStats._rotationSpeed;
        }
        else
        {
            rotationspeed = 50f;
        }
    }
}
