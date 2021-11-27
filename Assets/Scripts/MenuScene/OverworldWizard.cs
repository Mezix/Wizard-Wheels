﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldWizard : MonoBehaviour
{
    private Vector2 moveVector;
    private float moveSpeed;
    private Rigidbody2D rb;
    private Animator anim;

    public bool movementLocked;
    private void Awake()
    {
        movementLocked = false;
        moveVector = Vector2.zero;
        moveSpeed = 1.5f;
        rb = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if(!movementLocked)
        {
            if(!MovementInput()) moveVector = Vector2.zero;
        }
        else
        {
            moveVector = Vector2.zero;
        }

        anim.SetFloat("Speed", moveVector.magnitude);
        anim.SetFloat("Horizontal", moveVector.x);
        anim.SetFloat("Vertical", moveVector.y);
    }

    private bool MovementInput()
    {
        bool movementInput = false;
        if (Input.GetKey(KeyCode.W))
        {
            moveVector.y = 1;
            movementInput = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveVector.y = -1;
            movementInput = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveVector.x = -1;
            movementInput = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveVector.x = 1;
            movementInput = true;
        }
        moveVector.Normalize();
        moveVector.Scale(new Vector2(moveSpeed, moveSpeed));
        return movementInput;
    }

    private void FixedUpdate()
    {
        if (!movementLocked)
        {
            Move();
        }
    }
    public void Move()
    {
        rb.MovePosition(rb.transform.position + (Vector3) moveVector * Time.deltaTime);
    }
}
