using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private Rigidbody2D tankRB;
    private Collider2D tankCollider;
    public float _movespeed = 0.1f;
    public float velocity = 0f;
    private float acceleration = 0.0025f;
    private float deceleration = 0.005f;
    private float maxVelocity = 3f;
    public bool cruiseModeOn;

    public bool sliderValueSet = false;

    private void Awake()
    {
        cruiseModeOn = false;
        sliderValueSet = false;
        tankRB = GetComponent<Rigidbody2D>();
        tankCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) ToggleCruise();
        HandleMovementInput();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void HandleMovementInput()
    {
        if (sliderValueSet)
        {
            ChangeSpeedToSliderValue();
            if (Input.GetKey(KeyCode.W)) ; //TODO: Change Speed Slider Value Up
            if (Input.GetKey(KeyCode.S)) ; //Change Speed Slider Value Down
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) SpeedUp();
            if (Input.GetKey(KeyCode.S)) SpeedDown();
        }
    }
    public void ToggleCruise()
    {
        if(!cruiseModeOn)
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

    //  MOVEMENT
    private void Move()
    {
        transform.position += GetComponentInChildren<TankRotation>().transform.up * velocity * Time.deltaTime;
        if (!cruiseModeOn && !sliderValueSet) SpeedDown();
    }
    private void SpeedUp()
    {
        if (velocity < maxVelocity) velocity += acceleration;
        else velocity = maxVelocity;
        UIScript.instance._currentSpeedSlider.value = velocity;
        //if (!cruiseModeOn && UIScript.instance._currentSpeedSlider.value > UIScript.instance._desiredSpeedSlider.value)
        //{
        //    UIScript.instance._desiredSpeedSlider.value = UIScript.instance._currentSpeedSlider.value;
        //}
    }
    private void SpeedDown()
    {
        if (velocity > 0) velocity -= deceleration;
        else velocity = 0;
        UIScript.instance._currentSpeedSlider.value = velocity;
        //if (!cruiseModeOn && UIScript.instance._currentSpeedSlider.value < UIScript.instance._desiredSpeedSlider.value)
        //{
        //    UIScript.instance._desiredSpeedSlider.value = UIScript.instance._currentSpeedSlider.value;
        //}
    }
    private void EmergencyBrake()
    {

    }

    public void ChangeSpeedToSliderValue()
    {
        if (UIScript.instance._currentSpeedSlider.value > UIScript.instance._desiredSpeedSlider.value)
        {
            SpeedDown();
            if (UIScript.instance._currentSpeedSlider.value < UIScript.instance._desiredSpeedSlider.value)
            {
                velocity = UIScript.instance._desiredSpeedSlider.value;
                TurnOnCruise(true);
                sliderValueSet = false;
            }
        }
        if (UIScript.instance._currentSpeedSlider.value < UIScript.instance._desiredSpeedSlider.value)
        {
            SpeedUp();
            if (UIScript.instance._currentSpeedSlider.value > UIScript.instance._desiredSpeedSlider.value)
            {
                velocity = UIScript.instance._desiredSpeedSlider.value;
                TurnOnCruise(true);
                sliderValueSet = false;
            }
        }
    }
}
