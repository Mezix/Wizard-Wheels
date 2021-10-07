﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechWizard : MonoBehaviour
{
    public UnitStats _unitStats;
    public string UnitName { get; set; }
    public string UnitClass { get; set; }
    public float UnitHealth { get; set; }
    public float UnitSpeed { get; set; }
    public SpriteRenderer WizardRenderer { get; set; }
    public Animator WizardAnimator { get; set; }
    public bool UnitSelected { get; set; }
    public bool UnitIsMoving { get; set; }
    private Transform desiredTransform;
    private Vector3 VectorToGetTo;
    public Room currentRoom;
    public Room desiredRoom;
    public List<Room> PathToRoom;

    private void Awake()
    {
        WizardRenderer = GetComponentInChildren<SpriteRenderer>();
        WizardAnimator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        PathToRoom = new List<Room>();
        InitUnit();
    }
    private void Update()
    {
        if(UnitSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) DeterminePathToRoom();
        }
        CalculateCurrentRoom();
    }
    private void FixedUpdate()
    {
        if (UnitIsMoving)
        {
            MoveUnit(VectorToGetTo);
        }
    }

    private void CalculateCurrentRoom()
    {
        currentRoom = HM.RaycastOnPosition(transform.position, LayerMask.GetMask("Room")).collider.GetComponent<Room>();
    }

    public void InitUnit()
    {
        if (_unitStats)  //if we have a scriptableobject, use its stats
        {
            UnitName = _unitStats._unitName;
            UnitClass = _unitStats._unitClass;
            UnitHealth = _unitStats._unitHealth;
            UnitSpeed = _unitStats._unitSpeed;
        }
        else  //set default stats
        {
            print("No UnitStats found, setting default values");

            UnitName = "Bob";
            UnitClass = "Tech Wizard";
            UnitHealth = 100f;
            UnitSpeed = 5f;
        }
        UnitIsMoving = false;
        UnitSelected = false;
        desiredTransform = transform;
    }

    //  Move Unit

    public void DeterminePathToRoom()
    {
        //  Attempt to find a room, and then a position within that room
        RaycastHit2D hit = HM.RaycastToMouseCursor(LayerMask.GetMask("Room"));
        if (!hit.collider) return;
        if (!hit.collider.transform.TryGetComponent(out Room roomToGetTo)) return;
        if (roomToGetTo.freeRoomPositions.Count == 0) return;

        //  if we were in a room before, try to free up the last location we were in from that room so someone else can go there
        if(desiredRoom) desiredRoom.FreeUpRoom(desiredTransform);

        //  reserve the spot we are going to for ourselves
        desiredTransform = roomToGetTo.freeRoomPositions[0];
        desiredRoom = roomToGetTo;
        roomToGetTo.TakeUpRoom(roomToGetTo.freeRoomPositions[0]);

        //calculate the local vector of our room relative to the tank
        VectorToGetTo = desiredTransform.position - PlayerTankController.instance.transform.position;

        //calculate the path
        PathToRoom = UnitPathfinding.instance.FindPath(currentRoom, desiredRoom, PlayerTankController.instance.TGeo._tankRoomConstellation);
        print(PathToRoom.Count);

        //set the z to 0 so our sprite doesnt disappear
        VectorToGetTo.z = 0;

        //start the movement
        UnitIsMoving = true;
        UnitSelected = false;
    }
    private void MoveUnit(Vector3 newPos)
    {
        //calculate local vector between wizard and pos
        Vector3 moveVector = Vector3.Normalize(newPos - transform.localPosition);

        // set Animator Values
        WizardAnimator.SetFloat("Speed", moveVector.sqrMagnitude);
        WizardAnimator.SetFloat("Horizontal", moveVector.x);
        WizardAnimator.SetFloat("Vertical", moveVector.y);

        //move by this vector
        transform.localPosition += moveVector * UnitSpeed * Time.deltaTime;

        //check if weve reached our destination
        if (Vector3.Distance(transform.localPosition, newPos) <= (UnitSpeed * Time.deltaTime))
        {
            transform.localPosition = newPos;
            UnitIsMoving = false;
            WizardAnimator.SetFloat("Speed", 0);
            //if in room, set animation to interacting and set wizard to work
        }
    }
}
