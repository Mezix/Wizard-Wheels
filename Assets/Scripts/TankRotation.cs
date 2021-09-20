using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRotation : MonoBehaviour
{
    private float rotationspeed = 50f;
    public bool rotateBack = false;
    public bool rotationAngleSet = false;
    private float pointerAngle = 0;

    void Start()
    {
        rotateBack = false;
        rotationAngleSet = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) SetRotationSliderPositionPos();
        else if (rotateBack)
        {
            RotateBack();
        }
        else
        {
            HandleRotationInput();
        }
        SetRotationOfSteeringWheel();
    }
    private void HandleRotationInput()
    {
        if (rotationAngleSet) 
        {
            RotateToPointerAngle();
            if (Input.GetKey(KeyCode.A)); // Move The Pointer Left
            if (Input.GetKey(KeyCode.D)); // Move The Pointer Right
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) RotateSteeringWheelLeft();
            if (Input.GetKey(KeyCode.D)) RotateSteeringWheelRight();
        }
    }
    private void SetRotationOfSteeringWheel()
    { 
        Quaternion q = new Quaternion();
        q.eulerAngles = transform.rotation.eulerAngles;
        UIScript.instance.SteeringWheel.transform.rotation = q;
        rotateBack = false;
    }

    //  ROTATION
    public void ToggleRotateBack()
    {
        rotateBack = !rotateBack;
    }
    private void RotateBack()
    {
        float vec = transform.rotation.eulerAngles.z - 180;
        if (vec > 0)
        {
            RotateSteeringWheelLeft();
            if (Mathf.Abs(transform.rotation.eulerAngles.z) < 1)
            {
                transform.rotation = new Quaternion();
                rotateBack = false;
            }
        }
        if (vec < 0)
        {
            RotateSteeringWheelRight();
            if (Mathf.Abs(transform.rotation.eulerAngles.z) < 1)
            {
                transform.rotation = new Quaternion();
                rotateBack = false;
            }
        }
    }
    private void RotateSteeringWheelLeft()
    {
        transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
    }
    private void RotateSteeringWheelRight()
    {
        transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
    }
    private void SetRotationSliderPositionPos()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float rot = HM.Angle2D(transform.position, mouse);
        HM.RotateTransformToAngle(UIScript.instance.SteeringWheelPointer.transform, new Vector3 (0,0, rot + 90));
        pointerAngle = UIScript.instance.SteeringWheelPointer.transform.eulerAngles.z;
        rotationAngleSet = true;
    }
    private void RotateToPointerAngle()
    {
        float currentRot = transform.rotation.eulerAngles.z;
        float difference = currentRot - pointerAngle;

        if (difference > 0)
        {
            if (Mathf.Abs(difference) < 3)
            {
                rotateBack = false;
                rotationAngleSet = false;
                HM.RotateTransformToAngle(transform, new Vector3(0, 0, pointerAngle));
                HM.RotateTransformToAngle(UIScript.instance.SteeringWheel.transform, new Vector3(0, 0, pointerAngle));
            }
            else
            {
                RotateSteeringWheelRight();
                SetRotationOfSteeringWheel();
            }
        }
        if (difference < 0)
        {
            if (Mathf.Abs(difference) < 3)
            {
                rotateBack = false;
                rotationAngleSet = false;
                HM.RotateTransformToAngle(transform, new Vector3(0, 0, pointerAngle));
                HM.RotateTransformToAngle(UIScript.instance.SteeringWheel.transform, new Vector3(0, 0, pointerAngle));
            }
            else
            {
                RotateSteeringWheelLeft();
                SetRotationOfSteeringWheel();
            }
        }
    }
}
