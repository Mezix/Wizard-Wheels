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
    private float rotationspeed = 50f;
    public bool rotateBack;
    public bool cruiseModeOn;

    private void Awake()
    {
        rotateBack = false;
        cruiseModeOn = false;
        tankRB = GetComponent<Rigidbody2D>();
        tankCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) ToggleCruise();
        if (Input.GetKeyDown(KeyCode.Space)) ToggleRotateBack();
        HandleMovementInput();
        HandleRotationInput();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void HandleRotationInput()
    {
        if (rotateBack)
        {
            RotateBack();
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) RotateLeft();
            if (Input.GetKey(KeyCode.D)) RotateRight();
        }
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
        transform.position += transform.up * velocity * Time.deltaTime;
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

    //  ROTATION
    public void ToggleRotateBack()
    {
        rotateBack = !rotateBack;
    }
    private void RotateBack()
    {
        float vec = transform.rotation.eulerAngles.z - 180;
        if (vec > 0)
        {
            RotateLeft();
            if (Mathf.Abs(transform.rotation.eulerAngles.z) < 1)
            {
                transform.rotation = new Quaternion();
                rotateBack = false;
            }
        }
        if(vec < 0)
        {
            RotateRight();
            if (Mathf.Abs(transform.rotation.eulerAngles.z) < 1)
            {
                transform.rotation = new Quaternion();
                rotateBack = false;
            }
        }
    }
    private void RotateLeft()
    {
        transform.Rotate(Vector3.forward * rotationspeed * Time.deltaTime);
    }
    private void RotateRight()
    {
        transform.Rotate(Vector3.back * rotationspeed * Time.deltaTime);
    }
}
