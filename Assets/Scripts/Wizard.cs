using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    public SpriteRenderer unitSprite = null;
    public bool unitSelected = false;
    public bool unitIsMoving;

    public float unitMoveSpeed;
    public Vector3 localPositionToMoveTo;

    private void Awake()
    {
        
    }
    private void Start()
    {
        unitMoveSpeed = 5f;
        unitSelected = false;
        localPositionToMoveTo = transform.position;
    }
    private void Update()
    {
        if(unitSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) FindPathToRoom();
        }
        if (unitIsMoving)
        {
            MoveUnit(localPositionToMoveTo);
        }
    }
    private void FixedUpdate()
    {
    }

    private void MoveUnit(Vector3 newPos)
    {
        //calculate local vector between wizard and pos
        Vector3 moveVector = Vector3.Normalize(newPos - transform.localPosition);

        //move by this vector
        transform.localPosition += moveVector * unitMoveSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.localPosition, newPos) <= (unitMoveSpeed * Time.deltaTime))
        {
            transform.localPosition = newPos;
            unitIsMoving = false;
        }
    }

    public void FindPathToRoom()
    {
        localPositionToMoveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        localPositionToMoveTo.z = 0; 
        //calculate the local vector of our mouse position relative to the tank
        localPositionToMoveTo -= PlayerTankController.instance.transform.position;
        unitIsMoving = true;
        unitSelected = false;
    }
}
