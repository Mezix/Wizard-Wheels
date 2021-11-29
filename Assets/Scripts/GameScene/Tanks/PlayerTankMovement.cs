using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankMovement : TankMovement
{
    public bool cruiseModeOn;
    [HideInInspector]
    public bool _attemptingMatchingSpeed;
    [HideInInspector]
    public bool _matchSpeed;
    [HideInInspector]
    public TankMovement enemyToMatch;

    void Update()
    {
        if(_attemptingMatchingSpeed)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) AttemptMatchSpeed();
        }
        
        if (!Ref.PCon._dying)
        {
            if (Input.GetKeyDown(KeyCode.C)) ToggleCruise();
            if (_matchSpeed)
            {
                UpdateDesiredSpeedSlider();
            }
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
    private void UpdateDesiredSpeedSlider()
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

    private void AttemptMatchSpeed()
    {
        RaycastHit2D ray = HM.RaycastToMouseCursor();
        if (ray.collider)
        {
            EnemyTankMovement e = ray.collider.transform.root.GetComponentInChildren<EnemyTankMovement>();
            if (e)
            {
                _matchSpeed = true;
                enemyToMatch = e;
                TurnOnCruise(true);
            }
            else
            {
                print("error couldnt find enemy to match");
            }
        }
        else
        {
            print("error couldnt find enemy to match");
        }
        _attemptingMatchingSpeed = false;
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
    public void TurnOnCruise(bool b)
    {
        cruiseModeOn = b;
        Ref.UI.TurnOnCruiseMode(b);
    }
}
