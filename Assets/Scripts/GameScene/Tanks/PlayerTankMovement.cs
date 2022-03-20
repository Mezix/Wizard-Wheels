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
    public bool emergencyBrakeOn;

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
        UpdateEngineUI(cruiseModeOn);
        UpdateCurrentSpeedSlider();

        if (!cruiseModeOn && !movementInput) Decelerate();

        if (emergencyBrakeOn) deceleration = 4 * Ref.PCon._tStats._tankDecel;
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
            SlowlyMatchSpeedToSliderValue();
            if (Input.GetKey(KeyCode.W)) ChangeDesiredSliderSpeedUp();
            if (Input.GetKey(KeyCode.S)) ChangeDesiredSliderSpeedDown();
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                movementInput = true;
                Ref.UI.ActivateEmergencyBrake(false);
                Accelerate();
            }
            if (Input.GetKey(KeyCode.S))
            {
                movementInput = true;
                Ref.UI.ActivateEmergencyBrake(false);
                Decelerate();
            }
        }
    }

    //  MOVEMENT
    
    private void ChangeDesiredSliderSpeedUp()
    {
        Ref.UI._desiredSpeedSlider.value += acceleration;
        Ref.UI.ActivateEmergencyBrake(false);
    }
    private void ChangeDesiredSliderSpeedDown()
    {
        Ref.UI._desiredSpeedSlider.value -= deceleration;
        Ref.UI.ActivateEmergencyBrake(false);
    }
    private void UpdateCurrentSpeedSlider()
    {
        Ref.UI._currentSpeedSlider.value = currentSpeed;
    }
    private void MatchDesiredSpeedSliderToEnemy()
    {
        Ref.UI._desiredSpeedSlider.value = enemyToMatch.currentSpeed;
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
    public void SlowlyMatchEnemySpeed()
    {
        if (enemyToMatch.currentSpeed < currentSpeed)
        {
            Decelerate();
            if (enemyToMatch.currentSpeed > currentSpeed)
            {
                currentSpeed = Ref.UI._desiredSpeedSlider.value;
                return;
            }
        }
        if (enemyToMatch.currentSpeed > currentSpeed)
        {
            Accelerate();
            if (enemyToMatch.currentSpeed < currentSpeed)
            {
                currentSpeed = Ref.UI._desiredSpeedSlider.value;
                return;
            }
        }
    }

    public void TankEmergencyBrakeEffects()
    {
        print("brake effects initiated");
        emergencyBrakeOn = true;

        //Damage shield or hull once and stop extremely quickly
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
        cruiseModeOn = b;
        if (!b)
        {
            _matchSpeed = false;
        }
    }
    public void UpdateEngineUI(bool b)
    {
        if (b) Ref.UI._cruiseButton.targetGraphic.GetComponent<Animator>().speed = 1 + (currentSpeed/maxSpeed * 5);
        else Ref.UI._cruiseButton.targetGraphic.GetComponent<Animator>().speed = 0;
        currentSpeed = Mathf.Max(0, currentSpeed);
        Ref.SD.SetSpeed(currentSpeed * engineLevelMultiplier);
    }
}
