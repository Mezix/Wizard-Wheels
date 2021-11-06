using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankMovement : TankMovement
{
    public bool cruiseModeOn;

    void Update()
    {
        if (!PlayerTankController.instance._dying)
        {
            if (Input.GetKeyDown(KeyCode.C)) ToggleCruise();
            HandleMovementInput();
        }
        else DeathDeacceleration();

        SetTireAnimationSpeed();
        UpdateCurrentSpeedSlider();
    }
    private void FixedUpdate()
    {
        Move();
        if (!cruiseModeOn) Decelerate();
    }
    public void InitTankMovement()
    {
        TurnOnCruise(true);
        InitTires();
    }
    private void HandleMovementInput()
    {
        if (cruiseModeOn)
        {
            SlowlyMatchSpeedToSliderValue();
            if (Input.GetKey(KeyCode.W)) ChangeDesiredSliderSpeedUp();
            if (Input.GetKey(KeyCode.S)) ChangeDesiredSliderSpeedDown();
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                Accelerate();
            }
            if (Input.GetKey(KeyCode.S))
            {
                Decelerate();
            }
        }
    }

    //  MOVEMENT
    
    private void ChangeDesiredSliderSpeedUp()
    {
        Ref.UI._desiredSpeedSlider.value += acceleration;
    }
    private void ChangeDesiredSliderSpeedDown()
    {
        Ref.UI._desiredSpeedSlider.value -= deceleration;
    }
    private void UpdateCurrentSpeedSlider()
    {
        Ref.UI._currentSpeedSlider.value = currentSpeed;
    }
    public void SlowlyMatchSpeedToSliderValue()
    {
        if (Ref.UI._currentSpeedSlider.value > Ref.UI._desiredSpeedSlider.value)
        {
            Decelerate();
            TurnOnCruise(true);
            if (Ref.UI._currentSpeedSlider.value < Ref.UI._desiredSpeedSlider.value)
            {
                currentSpeed = Ref.UI._desiredSpeedSlider.value;
                return;
            }
        }
        if (Ref.UI._currentSpeedSlider.value < Ref.UI._desiredSpeedSlider.value)
        {
            Accelerate();
            TurnOnCruise(true);
            if (Ref.UI._currentSpeedSlider.value > Ref.UI._desiredSpeedSlider.value)
            {
                currentSpeed = Ref.UI._desiredSpeedSlider.value;
                return;
            }
        }
    }

    private void EmergencyBrake()
    {

    }
    
    public void DeathDeacceleration()
    {
        if (currentSpeed > 0) currentSpeed -= deceleration * Time.timeScale;
        else currentSpeed = 0;
    }

    //  Cruise Mode

    public void ToggleCruise()
    {
        if (!cruiseModeOn)
        {
            TurnOnCruise(true);
        }
        else
        {
            TurnOnCruise(false);
        }
    }
    private void TurnOnCruise(bool b)
    {
        cruiseModeOn = b;
        Ref.UI.TurnOnCruiseMode(b);
    }
}
