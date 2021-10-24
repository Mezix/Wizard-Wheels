﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankRotation : MonoBehaviour
{
    public float rotationspeed = 50f;
    public bool pointerAngleSet = false;
    [SerializeField]
    private float pointerAngle = 0;
    public bool steeringWheelSelectedByMouse = false;

    public Transform tankRotation;
    private List<GameObject> rotatableObjects = new List<GameObject>();

    void Update()
    {
        if(!PlayerTankController.instance._dying)
        {
            if (steeringWheelSelectedByMouse)
            {
                SetPointerRotationRelativeToSteeringWheel();
            }
            else if (Input.GetKeyDown(KeyCode.Z)) SetPointerRotationRelativeToTank();
            else
            {
                HandleRotationInput();
            }
            SetRotationOfSteeringWheel();
        }
    }
    public void InitTankRotation()
    {
        pointerAngleSet = false;
        InitRotatableObjects();
        rotatableObjects.Add(tankRotation.gameObject);
    }
    private void InitRotatableObjects()
    {
        foreach (Tire t in GetComponentsInChildren<Tire>()) rotatableObjects.Add(t.gameObject);
    }

    private void HandleRotationInput()
    {
        if (pointerAngleSet) 
        {
            RotateTankToPointerAngle();
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
        Ref.UI._steeringWheelPointer.transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
        pointerAngle += rotationspeed * Time.deltaTime;
    }
    private void MovePointerRight()
    {
        Ref.UI._steeringWheelPointer.transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
        pointerAngle -= rotationspeed * Time.deltaTime;
    }

    //  Rotate Steering Wheel
    private void SetPointerRotationRelativeToTank()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float rot = HM.Angle2D(transform.position, mouse);
        HM.RotateTransformToAngle(Ref.UI._steeringWheelPointer.transform, new Vector3(0, 0, rot + 90));
        pointerAngle = Ref.UI._steeringWheelPointer.transform.eulerAngles.z;
        pointerAngleSet = true;
    }
    private void SetPointerRotationToAngle(float angle)
    {
        HM.RotateTransformToAngle(Ref.UI._steeringWheelPointer.transform, new Vector3(0, 0, angle));
        pointerAngle = Ref.UI._steeringWheelPointer.transform.eulerAngles.z;
        pointerAngleSet = true;
    }
    public void SetPointerRotationRelativeToSteeringWheel()
    {
        float rot = HM.Angle2D(Ref.UI._steeringWheel.transform.position, Input.mousePosition);
        HM.RotateTransformToAngle(Ref.UI._steeringWheelPointer.transform, new Vector3(0, 0, rot + 90));
        pointerAngle = Ref.UI._steeringWheelPointer.transform.eulerAngles.z;
        pointerAngleSet = true;
    }

    private void SetRotationOfSteeringWheel()
    {
        HM.RotateTransformToAngle(Ref.UI._steeringWheel.transform, tankRotation.rotation.eulerAngles);
    }

    //  Rotate Tank Manually using the arrow keys
    private void RotateTankLeftManually()
    {
        RotateAllObjectsByRotation(rotationspeed * Time.deltaTime);
        Ref.UI._steeringWheelPointer.transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
        pointerAngle += rotationspeed * Time.deltaTime;
    }
    private void RotateTankRightManually()
    {
        RotateAllObjectsByRotation(-rotationspeed * Time.deltaTime);
        Ref.UI._steeringWheelPointer.transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
        pointerAngle -= rotationspeed * Time.deltaTime;
    }

    //  Rotate Tank
    private void RotateTankToPointerAngle()
    {
        float currentRot = tankRotation.rotation.eulerAngles.z;
        if (currentRot > 180) currentRot -= 360;
        if (pointerAngle > 180) pointerAngle -= 360;

        float difference = currentRot - pointerAngle;

        if (difference > 0)
        {
            if (Mathf.Abs(difference) < (rotationspeed * Time.deltaTime))
            {
                pointerAngleSet = false;
                RotateAllObjectsToRotation(pointerAngle);
                HM.RotateTransformToAngle(Ref.UI._steeringWheel.transform, new Vector3(0, 0, pointerAngle));
            }
            else
            {
                RotateAllObjectsByRotation(-rotationspeed * Time.deltaTime);
                SetRotationOfSteeringWheel();
            }
        }
        if (difference < 0)
        {
            if (Mathf.Abs(difference) < (rotationspeed * Time.deltaTime))
            {
                pointerAngleSet = false;
                RotateAllObjectsToRotation(pointerAngle);
                HM.RotateTransformToAngle(Ref.UI._steeringWheel.transform, new Vector3(0, 0, pointerAngle));
            }
            else
            {
                RotateAllObjectsByRotation(rotationspeed * Time.deltaTime);
                SetRotationOfSteeringWheel();
            }
        }
    }

    private void RotateAllObjectsToRotation(float zRot)
    {
        foreach (GameObject rotatable in rotatableObjects)
        {
            HM.RotateTransformToAngle(rotatable.transform, new Vector3(0, 0, pointerAngle));
        }
    }
    private void RotateAllObjectsByRotation(float zRot)
    {
        foreach (GameObject rotatable in rotatableObjects)
        {
            rotatable.transform.Rotate(Vector3.forward * zRot);
        }
    }

    //  Turn Tank Back to Default Direction
    public void TurnTankUp()
    {
        SetPointerRotationToAngle(0); //set rotation of pointer to point upwards
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
    } //TODO: use this code to fix the auto turning!

    //  (De)Select SteeringWheel to drag it around
    public void SelectSteeringWheel()
    {
        steeringWheelSelectedByMouse = true;
    }
    public void DeselectSteeringWheel()
    {
        steeringWheelSelectedByMouse = false;
    }
}
