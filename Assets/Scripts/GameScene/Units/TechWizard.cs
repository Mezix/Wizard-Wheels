using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechWizard : MonoBehaviour, IUnit
{
    public UnitStats _unitStats;
    public string UnitName { get; set; }
    public string UnitClass { get; set; }
    public float UnitHealth { get; set; }
    public float UnitSpeed { get; set; }
    public SpriteRenderer Rend { get; set; }
    public Animator WizardAnimator { get; set; }
    public bool UnitSelected { get; set; }
    public bool UnitIsMoving { get; set; }
    public GameObject UnitObj { get; set; }
    public UIWizard UIWizard { get; set; }

    //  Pathfinding

    public Vector3 RoomLocalPos;
    public Room CurrentRoom { get; set; }
    public RoomPosition CurrentRoomPos { get; set; }
    public Room DesiredRoom { get; set; }
    public RoomPosition DesiredRoomPos { get; set; }
    public List<RoomPosition> PathToRoom { get; set; }
    public int CurrentWaypoint { get; set; } //the index of our path

    private void Awake()
    {
        Rend = GetComponentInChildren<SpriteRenderer>();
        WizardAnimator = GetComponentInChildren<Animator>();
        UnitObj = gameObject;
    }
    private void Start()
    {
        CurrentWaypoint = 0;
        PathToRoom = new List<RoomPosition>();
        InitUnit();
    }
    private void Update()
    {
        if (UnitSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKey(KeyCode.LeftShift) && !Ref.mouse.IsPointerOverUIElement())
                Ref.Path.SetPathToRoom(GetComponent<IUnit>());
        }
        UpdateWizardUI();
    }
    private void FixedUpdate()
    {
        UnitBehaviour();
    }
    private void UpdateWizardUI()
    {
        if (UIWizard)
        {
            UIWizard._UIWizardHealthbarFill.fillAmount = Mathf.Min(1, UnitHealth / UnitHealth); //TODO: add unit health script
            //UIWizard.UnitSelected(UnitSelected);
            UIWizard.UpdateButton(UnitSelected);
        }
    }
    private void UnitBehaviour()
    {
        if (UnitIsMoving)
        {
            MoveAlongPath();
        }
    }

    public void StartInteraction()
    {
        WizardAnimator.SetBool("Interacting", true);
        CurrentRoom.roomSystem.StartInteraction();
    }
    public void StopInteraction()
    {
        WizardAnimator.SetBool("Interacting", false);
        CurrentRoom.roomSystem.StopInteraction();
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

        if (CurrentRoom.roomSystem != null)
        {
            StartInteraction();
        }
    }

    //  Move Unit
    public void ClearUnitPath()
    {
        PathToRoom.Clear();
    }
    private void MoveAlongPath()
    {
        WizardAnimator.SetBool("Interacting", false);
        if (PathToRoom.Count == 0) //stop the method if we dont have a path
        {
            UnitIsMoving = false;
            return;
        }
        RoomPosition roomPosToMoveTo = PathToRoom[CurrentWaypoint];

        //check if we have reached the last room
        if (Vector3.Distance(transform.position, PathToRoom[PathToRoom.Count - 1].transform.position) <= (UnitSpeed * Time.deltaTime))
        {
            print("destination reached");
            transform.localPosition = RoomLocalPos;
            UnitIsMoving = false;
            WizardAnimator.SetFloat("Speed", 0);

            ClearUnitPath();
            CurrentRoom = DesiredRoom;
            CurrentRoomPos = DesiredRoomPos;

            if (CurrentRoom.roomSystem != null) StartInteraction();
        }
        else
        {
            MoveUnitToNextRoom(roomPosToMoveTo);
        }
    }
    private void MoveUnitToNextRoom(RoomPosition nextRoomPos)
    {
        WizardAnimator.SetBool("Interacting", false);
        //calculate the local vector of our room relative to the tank
        RoomLocalPos = nextRoomPos.transform.position - PlayerTankController.instance.transform.position;
        //set the z to 0 so our sprite doesnt move on the z axis
        RoomLocalPos.z = 0;

        float distance = Vector2.Distance(transform.localPosition, RoomLocalPos);

        //calculate local vector between wizard and the next position
        Vector3 moveVector = Vector3.Normalize(RoomLocalPos - transform.localPosition);

        // set Animator Values
        WizardAnimator.SetFloat("Speed", moveVector.sqrMagnitude);
        WizardAnimator.SetFloat("Horizontal", moveVector.x);
        WizardAnimator.SetFloat("Vertical", moveVector.y);

        //move by this vector
        transform.localPosition += moveVector * UnitSpeed * Time.deltaTime;

        if (distance <= UnitSpeed * Time.deltaTime && !(CurrentWaypoint == PathToRoom.Count - 1))
        {
            CurrentWaypoint++;
            CurrentRoomPos = nextRoomPos;
            CurrentRoom = nextRoomPos.ParentRoom;
            transform.localPosition = RoomLocalPos;
        }
    }
}