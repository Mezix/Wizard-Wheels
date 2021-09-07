using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float RotationSpeed = 5f;

    public GameObject cannonballPrefab;
    public Transform cannonballSpot;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootTurret();
        }
    }
    private void ShootTurret()
    {
        GameObject obj = Instantiate(cannonballPrefab);
        obj.transform.position = cannonballSpot.position;
        obj.transform.rotation = transform.rotation;
    }

    void FixedUpdate()
    {
        PointTurretAtPosSmoothly(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    private void PointTurretAtPosSmoothly(Vector3 pos)
    {
        //  find the desired angle to face the mouse
        float zRotToMouse = Mathf.Rad2Deg * Mathf.Atan2(pos.y-transform.position.y, pos.x - transform.position.x); //TODO: make this a helper method!

        //  get closer to the angle with our max rotationspeed
        float zRotActual = 0;
        float diff = zRotToMouse - transform.rotation.eulerAngles.z;
        if (diff < -180) diff += 360;

        if (Mathf.Abs(diff) > RotationSpeed)
        {
            zRotActual = transform.rotation.eulerAngles.z + Mathf.Sign(diff) * RotationSpeed;
        }
        else
        {
            zRotActual = zRotToMouse;
        }

        //  rotate to this newly calculate angle
        Quaternion q = new Quaternion();
        q.eulerAngles = new Vector3(0, 0, zRotActual);
        transform.rotation = q;
    }
}
