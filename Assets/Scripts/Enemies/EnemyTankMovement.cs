using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankMovement : MonoBehaviour
{
    private Rigidbody2D tankRB;
    private Collider2D tankCollider;
    public float _movespeed = 0.1f;
    public float velocity = 0f;
    private float acceleration = 0.0025f;
    private float deceleration = 0.005f;
    private float maxVelocity = 3f;

    public List<Tire> Tires = new List<Tire>();

    private void Awake()
    {
        tankRB = GetComponent<Rigidbody2D>();
        tankCollider = GetComponent<Collider2D>();
        InitTires();
    }
    private void Start()
    {
    }

    void Update()
    {
        SetTireAnimationSpeed();
    }

    private void FixedUpdate()
    {
        Move();
    }

    //  MOVEMENT
    private void Move()
    {
        transform.position += GetComponentInChildren<EnemyTankRotation>().tankRotation.up * velocity * Time.deltaTime;
    }
    public void Accelerate()
    {
        if (velocity < maxVelocity) velocity += acceleration * Time.timeScale;
        else velocity = maxVelocity;
    }
    public void Decelerate()
    {
        if (velocity > 0) velocity -= deceleration * Time.timeScale;
        else velocity = 0;
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
