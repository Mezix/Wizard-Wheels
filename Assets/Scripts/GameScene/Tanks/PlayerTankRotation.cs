using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankRotation : TankRotation
{
    public bool pointerAngleSet = false;
    public bool steeringWheelSelectedByMouse = false;

    public float _rotationSpeedMultiplier;
    private void Awake()
    {
        _rotationSpeedMultiplier = 1;
    }
    void Update()
    {
        if(!Ref.PCon._dying)
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
        //InitRotatableObjects();
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
        Ref.UI._steeringWheelScript._steeringWheelPointer.transform.Rotate(Vector3.forward * rotationspeed * _rotationSpeedMultiplier *  Time.deltaTime);
        AngleToRotateTo += rotationspeed * _rotationSpeedMultiplier * Time.deltaTime;
    }
    private void MovePointerRight()
    {
        Ref.UI._steeringWheelScript._steeringWheelPointer.transform.Rotate(Vector3.back * rotationspeed * _rotationSpeedMultiplier *  Time.deltaTime);
        AngleToRotateTo -= rotationspeed * _rotationSpeedMultiplier * Time.deltaTime;
    }

    //  Rotate Steering Wheel
    private void SetPointerRotationRelativeToTank()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float rot = HM.GetAngle2DBetween(transform.position, mouse);
        HM.RotateTransformToAngle(Ref.UI._steeringWheelScript._steeringWheelPointer.transform, new Vector3(0, 0, rot + 90));
        AngleToRotateTo = Ref.UI._steeringWheelScript._steeringWheelPointer.transform.eulerAngles.z;
        pointerAngleSet = true;
    }
    private void SetPointerRotationToAngle(float angle)
    {
        HM.RotateTransformToAngle(Ref.UI._steeringWheelScript._steeringWheelPointer.transform, new Vector3(0, 0, angle));
        AngleToRotateTo = Ref.UI._steeringWheelScript._steeringWheelPointer.transform.eulerAngles.z;
        pointerAngleSet = true;
    }
    public void SetPointerRotationRelativeToSteeringWheel()
    {
        float rot = HM.GetAngle2DBetween(Ref.UI._steeringWheelScript._steeringWheel.transform.position, Input.mousePosition);
        HM.RotateTransformToAngle(Ref.UI._steeringWheelScript._steeringWheelPointer.transform, new Vector3(0, 0, rot + 90));
        AngleToRotateTo = Ref.UI._steeringWheelScript._steeringWheelPointer.transform.eulerAngles.z;
        pointerAngleSet = true;
    }

    private void SetRotationOfSteeringWheel()
    {
        HM.RotateTransformToAngle(Ref.UI._steeringWheelScript._steeringWheel.transform, rotatableObjects[0].transform.rotation.eulerAngles);
    }

    //  Rotate Tank Manually using the arrow keys
    private void RotateTankLeftManually()
    {
        RotateAllObjectsByRotation(rotationspeed * _rotationSpeedMultiplier * Time.deltaTime);
        Ref.UI._steeringWheelScript._steeringWheelPointer.transform.Rotate(Vector3.forward * rotationspeed * _rotationSpeedMultiplier * Time.deltaTime);
        AngleToRotateTo += rotationspeed * Time.deltaTime;
    }
    private void RotateTankRightManually()
    {
        RotateAllObjectsByRotation(-rotationspeed * _rotationSpeedMultiplier * Time.deltaTime);
        Ref.UI._steeringWheelScript._steeringWheelPointer.transform.Rotate(Vector3.back * rotationspeed * _rotationSpeedMultiplier * Time.deltaTime);
        AngleToRotateTo -= rotationspeed * Time.deltaTime;
    }

    //  Rotate Tank
    private void RotateTankToPointerAngle()
    {
        float currentRot = rotatableObjects[0].transform.rotation.eulerAngles.z;
        if (currentRot > 180) currentRot -= 360;
        if (AngleToRotateTo > 180) AngleToRotateTo -= 360;

        float difference = currentRot - AngleToRotateTo;

        if (difference > 0)
        {
            if (Mathf.Abs(difference) < (rotationspeed * _rotationSpeedMultiplier * Time.deltaTime))
            {
                pointerAngleSet = false;
                RotateAllObjectsToRotation(AngleToRotateTo);
                HM.RotateTransformToAngle(Ref.UI._steeringWheelScript._steeringWheel.transform, new Vector3(0, 0, AngleToRotateTo));
            }
            else
            {
                RotateAllObjectsByRotation(-rotationspeed * _rotationSpeedMultiplier * Time.deltaTime);
                SetRotationOfSteeringWheel();
            }
        }
        if (difference < 0)
        {
            if (Mathf.Abs(difference) < (rotationspeed * _rotationSpeedMultiplier * Time.deltaTime))
            {
                pointerAngleSet = false;
                RotateAllObjectsToRotation(AngleToRotateTo);
                HM.RotateTransformToAngle(Ref.UI._steeringWheelScript._steeringWheel.transform, new Vector3(0, 0, AngleToRotateTo));
            }
            else
            {
                RotateAllObjectsByRotation(rotationspeed * _rotationSpeedMultiplier * Time.deltaTime);
                SetRotationOfSteeringWheel();
            }
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
        AngleToRotateTo = 0;
    }

    //  (De)select the SteeringWheel to drag it around
    public void SelectSteeringWheel()
    {
        steeringWheelSelectedByMouse = true;
    }
    public void DeselectSteeringWheel()
    {
        steeringWheelSelectedByMouse = false;
    }
}
