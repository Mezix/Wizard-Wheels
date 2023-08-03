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
        if (!REF.PCon._dying)
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
            deceleration = 3 * REF.PCon._tStats._tankDecel;
            if(currentSpeed <= 1)
            {
                StopEmergencyBrake();
            }
        }
        else deceleration = REF.PCon._tStats._tankDecel;
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

    //  MOVEMENT
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
                REF.UI._engineUIScript.ActivateEmergencyBrake(false);
                Accelerate();
            }
            if (Input.GetKey(KeyCode.S))
            {
                movementInput = true;
                REF.UI._engineUIScript.ActivateEmergencyBrake(false);
                Decelerate();
            }
        }
    }

    private void ChangeDesiredSliderSpeedUp()
    {
        REF.UI._engineUIScript._desiredSpeedSlider.value += acceleration;
        REF.UI._engineUIScript.ActivateEmergencyBrake(false);
    }
    private void ChangeDesiredSliderSpeedDown()
    {
        REF.UI._engineUIScript._desiredSpeedSlider.value -= deceleration;
        REF.UI._engineUIScript.ActivateEmergencyBrake(false);
    }
    private void UpdateCurrentSpeedSlider()
    {
        REF.UI._engineUIScript._currentSpeedSlider.value = currentSpeed;
    }
    private void MatchDesiredSpeedSliderToEnemy()
    {
        REF.UI._engineUIScript._desiredSpeedSlider.value = enemyToMatch.currentSpeed;
    }
    public void SlowlyMatchSpeedToSliderValue()
    {
        if (REF.UI._engineUIScript._currentSpeedSlider.value > REF.UI._engineUIScript._desiredSpeedSlider.value)
        {
            Decelerate();
            TurnOnCruise(true);
            if (REF.UI._engineUIScript._currentSpeedSlider.value < REF.UI._engineUIScript._desiredSpeedSlider.value)
            {
                currentSpeed = REF.UI._engineUIScript._desiredSpeedSlider.value;
                return;
            }
        }
        if (REF.UI._engineUIScript._currentSpeedSlider.value < REF.UI._engineUIScript._desiredSpeedSlider.value)
        {
            Accelerate();
            TurnOnCruise(true);
            if (REF.UI._engineUIScript._currentSpeedSlider.value > REF.UI._engineUIScript._desiredSpeedSlider.value)
            {
                currentSpeed = REF.UI._engineUIScript._desiredSpeedSlider.value;
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

    //  Collisions

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ProjectileObstacle")
        {
            if(REF.PCon.TMov._tRB.velocity.magnitude / maxSpeed > 0.25f)
            {
                currentSpeed = 0.5f * maxSpeed;
                //REF.PCon.TakeDamage(1);
            }
            Debug.Log("Hit walls");
        }
        if (collision.gameObject.TryGetComponent(out EnemyTankController enemy))
        {
            if ((REF.PCon.TMov._tRB.velocity - new Vector2(enemy._navMeshAgent.velocity.x, enemy._navMeshAgent.velocity.y)).magnitude > 2f)
            {
                currentSpeed = 0.5f * maxSpeed;
                REF.PCon.TakeDamage(1);

                enemy.TMov.currentSpeed = 0.5f * enemy.TMov.maxSpeed;
                enemy.TakeDamage(1);
            }
            Debug.Log("hit enemy tank");
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
            REF.PCon.TakeDamage(1);
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
        REF.UI._engineUIScript.TurnOnCruiseUI(b);
        cruiseModeOn = b;
    }
    public void UpdateEngineUI()
    {
        REF.UI._engineUIScript._engineAnimator.speed = currentSpeed/maxSpeed * 5;
        currentSpeed = Mathf.Max(0, currentSpeed);
        REF.SD.SetSpeed(currentSpeed);
    }
}
