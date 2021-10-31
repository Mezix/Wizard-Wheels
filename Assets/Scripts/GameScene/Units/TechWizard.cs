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
    public int currentWaypoint { get; set; } //the index of our path

    private void Awake()
    {
        Rend = GetComponentInChildren<SpriteRenderer>();
        WizardAnimator = GetComponentInChildren<Animator>();
        UnitObj = gameObject;
    }
    private void Start()
    {
        currentWaypoint = 0;
        PathToRoom = new List<RoomPosition>();
        InitUnit();
    }
    private void Update()
    {
        if (UnitSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKey(KeyCode.LeftShift) && !Ref.mouse.IsPointerOverUIElement())
                Ref.Path.DeterminePathToRoom(GetComponent<IUnit>());
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

    /*public void DeterminePathToRoom()
    {
        //  Attempt to find a valid room, if we dont find one, deselect units instead
        RaycastHit2D hit = HM.RaycastToMouseCursor(LayerMask.GetMask("Room"));

        if (!hit.collider || !hit.collider.transform.TryGetComponent(out Room roomToGetTo) || roomToGetTo.GetNextFreeRoomPos() == null)
        {
            Ref.mouse.DeselectAllUnits();
            print("no valid room found, deselecting unit and aborting pathfinding");
            return;
        }
        if (!CurrentRoom.tr.Equals(roomToGetTo.tr))
        {
            Ref.mouse.DeselectAllUnits();
            print("Trying to get to a different Tank than the one we are in, Returning and deselecting Unit!");
            return;
        }
        if (roomToGetTo.Equals(CurrentRoom))
        {
            Ref.mouse.DeselectAllUnits();
            print("Trying to enter same room, Deselecting unit!");
            return;
        }

        //  Check if Path is possible!

        //  free up the room we are currently in so someone else can go there
        if (CurrentRoom && CurrentRoomPos) CurrentRoom.FreeUpRoomPos(CurrentRoomPos);

        //if we were already moving somewhere, free up the space we were last moving to and find our current room
        if (PathToRoom.Count > 0)
        {
            print("Diverting path!");
            //currentRoom = PathToRoom[currentWaypoint].ParentRoom;
            //currentRoomPos = currentRoom.allRoomPositions[0];
            DesiredRoom.FreeUpRoomPos(DesiredRoomPos);
        }

        //  reserve the spot we are going to for ourselves
        DesiredRoom = roomToGetTo;
        DesiredRoomPos = DesiredRoom.GetNextFreeRoomPos();
        roomToGetTo.OccupyRoomPos(DesiredRoomPos);

        //calculate the path
        ClearPathToRoom();
        currentWaypoint = 0;
        PathToRoom = UnitPathfinding.instance.FindPath(CurrentRoomPos, DesiredRoomPos, PlayerTankController.instance.TGeo._tankRoomConstellation);

        // stop interacting with the system in our room if we have one
        if (CurrentRoom.roomSystem != null) StopInteraction();

        //start the movement
        UnitIsMoving = true;
        UnitSelected = false;
    }*/
    public void ClearPathToRoom()
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
        RoomPosition roomPosToMoveTo = PathToRoom[currentWaypoint];

        //check if we have reached the last room
        if (Vector3.Distance(transform.position, PathToRoom[PathToRoom.Count - 1].transform.position) <= (UnitSpeed * Time.deltaTime))
        {
            print("destination reached");
            transform.localPosition = RoomLocalPos;
            UnitIsMoving = false;
            WizardAnimator.SetFloat("Speed", 0);

            ClearPathToRoom();
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

        if (distance <= UnitSpeed * Time.deltaTime && !(currentWaypoint == PathToRoom.Count - 1))
        {
            currentWaypoint++;
            CurrentRoomPos = nextRoomPos;
            CurrentRoom = nextRoomPos.ParentRoom;
            transform.localPosition = RoomLocalPos;
        }
    }
}