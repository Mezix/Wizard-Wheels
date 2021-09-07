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

    private void Awake()
    {
        cruiseModeOn = false;
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
        if (Input.GetKey(KeyCode.W)) SpeedUp();
        if (Input.GetKey(KeyCode.S)) SpeedDown();
    }
    public void ToggleCruise()
    {
        cruiseModeOn = !cruiseModeOn;
    }

    //  MOVEMENT
    private void Move()
    {
        transform.position += GetComponentInChildren<TankRotation>().transform.up * velocity * Time.deltaTime;
        if (!cruiseModeOn) SpeedDown();
    }
    private void SpeedUp()
    {
        if (velocity < maxVelocity) velocity += acceleration;
        else velocity = maxVelocity;
        UIScript.instance.AccelerationSlider.value = velocity;
    }
    private void SpeedDown()
    {
        if (velocity > 0) velocity -= deceleration;
        else velocity = 0;
        UIScript.instance.AccelerationSlider.value = velocity;
    }
}
