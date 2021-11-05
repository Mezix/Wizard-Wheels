using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AUnit : MonoBehaviour
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
    public GameObject MovingToPosIndicator { get; set; }

    private void Update()
    {
        UpdateWizardUI();
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
            Debug.Log("No UnitStats found, setting default values");

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
    public void StartInteraction()
    {
        if (CurrentRoom.roomSystem != null)
        {
            if (CurrentRoom.roomSystem.RoomPosForInteraction.Equals(CurrentRoomPos))
            {
                CurrentRoom.roomSystem.StartInteraction();
                WizardAnimator.SetBool("Interacting", true);
            }
        }
    }
    public void StopInteraction()
    {
        WizardAnimator.SetBool("Interacting", false);
        CurrentRoom.roomSystem.StopInteraction();
    }
    
    //  Move Unit
    protected void MoveAlongPath()
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
            Debug.Log("destination reached");
            transform.localPosition = RoomLocalPos;
            UnitIsMoving = false;
            WizardAnimator.SetFloat("Speed", 0);

            ClearUnitPath();
            RemovePosIndicator();
            CurrentRoom = DesiredRoom;
            CurrentRoomPos = DesiredRoomPos;
            StartInteraction();
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
    public void RemovePosIndicator()
    {
        if (MovingToPosIndicator)
        {
            Destroy(MovingToPosIndicator);
            MovingToPosIndicator = null;
        }
    }
    public void SetNextPosIndicator(RoomPosition rPos)
    {
        if (!rPos) return;
        if (MovingToPosIndicator) Destroy(MovingToPosIndicator);

        MovingToPosIndicator = Instantiate((GameObject)Resources.Load("UnitMovingToIndicator"));
        MovingToPosIndicator.transform.parent = rPos.transform;
        MovingToPosIndicator.transform.localPosition = Vector3.zero;
    }
    public void ClearUnitPath()
    {
        PathToRoom.Clear();
    }
    private void UpdateWizardUI()
    {
        if (UIWizard)
        {
            UIWizard._UIWizardHealthbarFill.fillAmount = Mathf.Min(1, UnitHealth / UnitHealth); //TODO: add unit health script
            UIWizard.UpdateButton(UnitSelected);
        }
    }
}
