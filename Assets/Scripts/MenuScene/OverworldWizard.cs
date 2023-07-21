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
    public SpriteRenderer _orb;
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
        if (!REF.mUI._mainMenuGO.activeInHierarchy)
        {
            CheckPlayerDistanceFromOrb();
        }

        if (!movementLocked)
        {
            MovementInput();
        }
        anim.SetFloat("Speed", moveVector.magnitude);
        anim.SetFloat("Horizontal", moveVector.x);
        anim.SetFloat("Vertical", moveVector.y);

        UpdateOrbSortingLayer();
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
        moveVector = Vector2.zero;
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
        if (Vector3.Distance(transform.position, REF.mMenu.orb.transform.position) <= 1.25f)
        {
            REF.mCam.SetZoom(REF.mCam.closestZoom);
            REF.mCam.SetCamParent(REF.mMenu.orb.transform);
            REF.mUI._selectScreenGO.SetActive(true);
            REF.TankPreview.ShowTank(REF.TankPreview.tankIndex);
        }
        else
        {
            REF.mCam.SetZoom(REF.mCam.furthestZoom);
            REF.mCam.SetCamParent(transform);
            REF.mUI._selectScreenGO.SetActive(false);
            REF.TankPreview.HideAllTanks();
        }
    }
    private void UpdateOrbSortingLayer()
    {
        _orb.sortingOrder = Mathf.Clamp(Mathf.FloorToInt(transform.position.y * 10),-1, 1);
    }
}
