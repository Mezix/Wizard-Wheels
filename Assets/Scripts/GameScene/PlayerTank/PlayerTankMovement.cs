﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankMovement : MonoBehaviour
{
    private Rigidbody2D tankRB;
    private Collider2D tankCollider;
    public float _movespeed = 0.1f;
    public float velocity = 0f;
    public Vector3 _movementVector;
    private float acceleration = 0.0025f;
    private float deceleration = 0.005f;
    private float maxVelocity = 5f;
    public bool cruiseModeOn;

    public List<Tire> Tires = new List<Tire>();

    private void Awake()
    {
        tankRB = GetComponent<Rigidbody2D>();
        tankCollider = GetComponent<Collider2D>();
    }
    void Update()
    {
        if (!PlayerTankController.instance._dying)
        {
            if (Input.GetKeyDown(KeyCode.C)) ToggleCruise();
            HandleMovementInput();
        }
        else DeathDeacceleration();
        SetTireAnimationSpeed();
    }
    private void FixedUpdate()
    {
        Move();
    }
    public void InitTankMovement()
    {
        UIScript.instance._currentSpeedSlider.value = 0;
        UIScript.instance._desiredSpeedSlider.value = 0;
        TurnOnCruise(true);
        InitTires();
    }
    private void HandleMovementInput()
    {
        if (cruiseModeOn)
        {
            SlowlyMatchSpeedToSliderValue();
            if (Input.GetKey(KeyCode.W)) ChangeSliderSpeedUp();
            if (Input.GetKey(KeyCode.S)) ChangeSliderSpeedDown();
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) Accelerate();
            if (Input.GetKey(KeyCode.S)) Decelerate();
        }

    }

    //  MOVEMENT
    private void Move()
    {
        _movementVector = GetComponentInChildren<PlayerTankRotation>().tankRotation.up;
        transform.position += _movementVector * velocity * Time.deltaTime;

        if (!cruiseModeOn) Decelerate();
    }
    private void Accelerate()
    {
        if (velocity < maxVelocity) velocity += acceleration * Time.timeScale;
        else velocity = maxVelocity;
        UIScript.instance._currentSpeedSlider.value = velocity;
    }
    private void Decelerate()
    {
        if (velocity > 0) velocity -= deceleration * Time.timeScale;
        else velocity = 0;
        UIScript.instance._currentSpeedSlider.value = velocity;
    }
    private void ChangeSliderSpeedUp()
    {
        UIScript.instance._desiredSpeedSlider.value += acceleration;
    }
    private void ChangeSliderSpeedDown()
    {
        UIScript.instance._desiredSpeedSlider.value -= deceleration;
    }
    public void SlowlyMatchSpeedToSliderValue()
    {
        if (UIScript.instance._currentSpeedSlider.value > UIScript.instance._desiredSpeedSlider.value)
        {
            Decelerate();
            TurnOnCruise(true);
            if (UIScript.instance._currentSpeedSlider.value < UIScript.instance._desiredSpeedSlider.value)
            {
                velocity = UIScript.instance._desiredSpeedSlider.value;
                return;
            }
        }
        if (UIScript.instance._currentSpeedSlider.value < UIScript.instance._desiredSpeedSlider.value)
        {
            Accelerate();
            TurnOnCruise(true);
            if (UIScript.instance._currentSpeedSlider.value > UIScript.instance._desiredSpeedSlider.value)
            {
                velocity = UIScript.instance._desiredSpeedSlider.value;
                return;
            }
        }
    }

    private void EmergencyBrake()
    {

    }
    public void DeathDeacceleration()
    {
        if (velocity > 0) velocity -= deceleration * Time.timeScale;
        else velocity = 0;
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
        UIScript.instance.TurnOnCruiseMode(b);
    }

    //  Change the animation of our tires
    private void InitTires()
    {
        foreach (Tire t in GetComponentsInChildren<Tire>()) Tires.Add(t);
    }
    private void SetTireAnimationSpeed()
    {
        foreach (Tire t in Tires)
        {
            t.AnimatorSpeed(velocity / maxVelocity);
        }
    }
}