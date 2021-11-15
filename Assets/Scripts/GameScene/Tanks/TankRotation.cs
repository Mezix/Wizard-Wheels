﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankRotation : MonoBehaviour
{
    public List<GameObject> rotatableObjects = new List<GameObject>();
    public float rotationspeed;
    public float AngleToRotateTo;
    protected void InitRotatableObjects()
    {
        foreach (Tire t in GetComponentsInChildren<Tire>()) rotatableObjects.Add(t.gameObject);
    }
    protected void RotateAllObjectsToRotation(float zRot)
    {
        foreach (GameObject rotatable in rotatableObjects)
        {
            HM.RotateTransformToAngle(rotatable.transform, new Vector3(0, 0, zRot));
        }
    }
    protected void RotateAllObjectsByRotation(float zRot)
    {
        foreach (GameObject rotatable in rotatableObjects)
        {
            rotatable.transform.Rotate(Vector3.forward * zRot);
        }
    }
}