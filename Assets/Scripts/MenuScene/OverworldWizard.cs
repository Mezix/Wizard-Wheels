using System;
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
        if (!Ref.mUI._mainMenuGO.activeInHierarchy)
        {
            CheckPlayerDistanceFromOrb();
        }

        if (!movementLocked)
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
    private void FixedUpdate()
    {
        if (!movementLocked)
        {
            Move();
        }
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
    public void Move()
    {
        rb.MovePosition(rb.transform.position + (Vector3) moveVector * Time.deltaTime);
    }
    private void CheckPlayerDistanceFromOrb()
    {
        if (Vector3.Distance(transform.position, Ref.mMenu.orb.transform.position) <= 1.25f)
        {
            Ref.mCam.SetZoom(Ref.mCam.closestZoom);
            Ref.mCam.SetCamParent(Ref.mMenu.orb.transform);
            Ref.mUI._selectScreenGO.SetActive(true);
        }
        else
        {
            Ref.mCam.SetZoom(Ref.mCam.furthestZoom);
            Ref.mCam.SetCamParent(transform);
            Ref.mUI._selectScreenGO.SetActive(false);
        }
    }
}
