using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRotation : MonoBehaviour
{
    private float rotationspeed = 50f;
    public bool pointerAngleSet = false;
    private float pointerAngle = 0;

    void Start()
    {
        pointerAngleSet = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) SetPointerRotation();
        else
        {
            HandleRotationInput();
        }
        SetRotationOfSteeringWheel();
    }


    private void HandleRotationInput()
    {
        if (pointerAngleSet) 
        {
            RotateToPointerAngle();
            if (Input.GetKey(KeyCode.A)) MovePointerLeft();  // Move The Pointer Left
            if (Input.GetKey(KeyCode.D)) MovePointerRight(); // Move The Pointer Right
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) RotateTankLeftManually();
            if (Input.GetKey(KeyCode.D)) RotateTankRightManually();
        }
    }

    //  Rotate Pointer when we are rotating towards it

    private void MovePointerLeft()
    {
        UIScript.instance.SteeringWheelPointer.transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
        pointerAngle += rotationspeed * Time.deltaTime;
    }
    private void MovePointerRight()
    {
        UIScript.instance.SteeringWheelPointer.transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
        pointerAngle -= rotationspeed * Time.deltaTime;
    }

    //  Rotate Steering Wheel
    private void SetPointerRotation(float angle = -100000)
    {
        if(angle > -100000) //if we entered an actual angle, then rotate to there
        {
            HM.RotateTransformToAngle(UIScript.instance.SteeringWheelPointer.transform, new Vector3(0, 0, angle));
        }
        else
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float rot = HM.Angle2D(transform.position, mouse);
            HM.RotateTransformToAngle(UIScript.instance.SteeringWheelPointer.transform, new Vector3(0, 0, rot + 90));
        }
        pointerAngle = UIScript.instance.SteeringWheelPointer.transform.eulerAngles.z;
        pointerAngleSet = true;
    }
    private void SetRotationOfSteeringWheel()
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = transform.rotation.eulerAngles;
        UIScript.instance.SteeringWheel.transform.rotation = q;
    }

    //  Rotate Tank Manually using the arrow keys
    private void RotateTankLeftManually()
    {
        transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
        UIScript.instance.SteeringWheelPointer.transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
        pointerAngle += rotationspeed * Time.deltaTime;
    }
    private void RotateTankRightManually()
    {
        transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
        UIScript.instance.SteeringWheelPointer.transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
        pointerAngle -= rotationspeed * Time.deltaTime;
    }
    private void RotateToPointerAngle()
    {
        float currentRot = transform.rotation.eulerAngles.z;
        float difference = currentRot - pointerAngle;

        if (difference > 0)
        {
            if (Mathf.Abs(difference) < 3)
            {
                pointerAngleSet = false;
                HM.RotateTransformToAngle(transform, new Vector3(0, 0, pointerAngle));
                HM.RotateTransformToAngle(UIScript.instance.SteeringWheel.transform, new Vector3(0, 0, pointerAngle));
            }
            else
            {
                transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
                SetRotationOfSteeringWheel();
            }
        }
        if (difference < 0)
        {
            if (Mathf.Abs(difference) < 3)
            {
                pointerAngleSet = false;
                HM.RotateTransformToAngle(transform, new Vector3(0, 0, pointerAngle));
                HM.RotateTransformToAngle(UIScript.instance.SteeringWheel.transform, new Vector3(0, 0, pointerAngle));
            }
            else
            {
                transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
                SetRotationOfSteeringWheel();
            }
        }
    }

    //  Turn Tank Back to Default Direction
    public void TurnTankUp()
    {
        SetPointerRotation(0); //set rotation of pointer to point upwards
    }
    private void RotateBack()
    {
        pointerAngleSet = true;
        pointerAngle = 0;
        /*float vec = transform.rotation.eulerAngles.z - 180;
        if (vec > 0)
        {
            transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
            if (Mathf.Abs(transform.rotation.eulerAngles.z) < 1)
            {
                transform.rotation = new Quaternion();
                rotateBack = false;
                print("rotateback finished");
            }
        }
        if (vec < 0)
        {
            transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
            if (Mathf.Abs(transform.rotation.eulerAngles.z) < 1)
            {
                transform.rotation = new Quaternion();
                rotateBack = false;
                print("rotateback finished");
            }
        }*/
    }
}
