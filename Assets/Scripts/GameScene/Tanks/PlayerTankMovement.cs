using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankMovement : TankMovement
{
    public bool cruiseModeOn;
    public bool _matchSpeed;
    [HideInInspector]
    public EnemyTankMovement enemyToMatch;
    public bool movementInput;

    //  Brake

    public bool emergencyBrakeOn;
    private float emergencyBrakeSpeedStart;

    private void Start()
    {
        emergencyBrakeOn = false;
    }
    void Update()
    {
        if (!Ref.PCon._dying)
        {
            if (Input.GetKeyDown(KeyCode.C)) ToggleCruise();
            if (_matchSpeed)
            {
                MatchDesiredSpeedSliderToEnemy();
            }
            HandleMovementInput();
        }
        else DeathDeacceleration();

        SetTireAnimationSpeed();
        UpdateEngineUI();
        UpdateCurrentSpeedSlider();

        if (!cruiseModeOn && !movementInput)
        {
            Decelerate();
        }

        if (emergencyBrakeOn)
        {
            deceleration = 3 * Ref.PCon._tStats._tankDecel;
            if(currentSpeed <= 1)
            {
                StopEmergencyBrake();
            }
        }
        else deceleration = Ref.PCon._tStats._tankDecel;
    }

    private void FixedUpdate()
    {
        Move();
    }
    public void InitTankMovement()
    {
        TurnOnCruise(true);
        InitTires();
    }
    private void HandleMovementInput()
    {
        movementInput = false;
        if (cruiseModeOn)
        {
            if (_matchSpeed)
            {
                if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
                {
                    if (Input.GetKey(KeyCode.W)) Accelerate();
                    if (Input.GetKey(KeyCode.S)) Decelerate();
                }
                else
                {
                    SlowlyMatchSpeedToSliderValue();
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.W)) ChangeDesiredSliderSpeedUp();
                if (Input.GetKey(KeyCode.S)) ChangeDesiredSliderSpeedDown();
                SlowlyMatchSpeedToSliderValue();
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                movementInput = true;
                Ref.UI._engineUIScript.ActivateEmergencyBrake(false);
                Accelerate();
            }
            if (Input.GetKey(KeyCode.S))
            {
                movementInput = true;
                Ref.UI._engineUIScript.ActivateEmergencyBrake(false);
                Decelerate();
            }
        }
    }

    //  MOVEMENT
    
    private void ChangeDesiredSliderSpeedUp()
    {
        Ref.UI._engineUIScript._desiredSpeedSlider.value += acceleration;
        Ref.UI._engineUIScript.ActivateEmergencyBrake(false);
    }
    private void ChangeDesiredSliderSpeedDown()
    {
        Ref.UI._engineUIScript._desiredSpeedSlider.value -= deceleration;
        Ref.UI._engineUIScript.ActivateEmergencyBrake(false);
    }
    private void UpdateCurrentSpeedSlider()
    {
        Ref.UI._engineUIScript._currentSpeedSlider.value = currentSpeed;
    }
    private void MatchDesiredSpeedSliderToEnemy()
    {
        Ref.UI._engineUIScript._desiredSpeedSlider.value = enemyToMatch.currentSpeed;
    }
    public void SlowlyMatchSpeedToSliderValue()
    {
        if (Ref.UI._engineUIScript._currentSpeedSlider.value > Ref.UI._engineUIScript._desiredSpeedSlider.value)
        {
            Decelerate();
            TurnOnCruise(true);
            if (Ref.UI._engineUIScript._currentSpeedSlider.value < Ref.UI._engineUIScript._desiredSpeedSlider.value)
            {
                currentSpeed = Ref.UI._engineUIScript._desiredSpeedSlider.value;
                return;
            }
        }
        if (Ref.UI._engineUIScript._currentSpeedSlider.value < Ref.UI._engineUIScript._desiredSpeedSlider.value)
        {
            Accelerate();
            TurnOnCruise(true);
            if (Ref.UI._engineUIScript._currentSpeedSlider.value > Ref.UI._engineUIScript._desiredSpeedSlider.value)
            {
                currentSpeed = Ref.UI._engineUIScript._desiredSpeedSlider.value;
                return;
            }
        }
    }

    public void MatchSpeed(EnemyTankMovement e, bool b)
    {
        if (e)
        {
            _matchSpeed = b;
            enemyToMatch = e;
            TurnOnCruise(true);
        }
        else
        {
            _matchSpeed = false;
            print("error couldnt find enemy to match");
        }
    }

    //  Emergency Brake

    public void StartEmergencyBrake()
    {
        emergencyBrakeOn = true;
        emergencyBrakeSpeedStart = currentSpeed;
    }
    private void StopEmergencyBrake()
    {
        //  Play sound effect!
            emergencyBrakeOn = false;
        if (emergencyBrakeSpeedStart > (maxSpeed * 0.5f))
        {
            Ref.PCon.TakeDamage(1);
        }
    }
    public void DeathDeacceleration()
    {
        if (currentSpeed > 0) currentSpeed -= deceleration * 2 * Time.timeScale;
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
        _matchSpeed = false;
    }
    public void TurnOnCruise(bool b)
    {
        if(!b) _matchSpeed = false;
        Ref.UI._engineUIScript.TurnOnCruiseUI(b);
        cruiseModeOn = b;
    }
    public void UpdateEngineUI()
    {
        Ref.UI._engineUIScript._engineAnimator.speed = currentSpeed/maxSpeed * 5;
        currentSpeed = Mathf.Max(0, currentSpeed);
        Ref.SD.SetSpeed(currentSpeed);
    }
}
