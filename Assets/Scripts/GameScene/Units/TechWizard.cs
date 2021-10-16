using System;
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

    //  Pathfinding

    private Vector3 VectorToGetTo;
    public Room currentRoom;
    public Transform currentRoomPos;
    public Room desiredRoom;
    public Transform desiredRoomPos;
    public List<Room> PathToRoom;
    private int currentWaypoint = 0; //the index of our path.vectorPath
    private float nextWayPointDistance = 0.1f; //the distance before we seek out our next waypoint => the higher, the smoother the movement

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
        if (UnitSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) DeterminePathToRoom();
        }
    }
    private void FixedUpdate()
    {
        if (UnitIsMoving)
        {
            MoveAlongPath();
        }
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
    }

    //  Move Unit

    public void DeterminePathToRoom()
    {
        //  Attempt to find a valid room, if we dont find one, deselect units instead
        RaycastHit2D hit = HM.RaycastToMouseCursor(LayerMask.GetMask("Room"));
        if (!hit.collider || !hit.collider.transform.TryGetComponent(out Room roomToGetTo) || roomToGetTo.freeRoomPositions.Count == 0)
        {
            Ref.mouse.DeselectAllUnits();
            print("no valid room found, deselecting unit and aborting pathfinding");
            return;
        }

        //  free up the room we are currently in so someone else can go there
        if (currentRoom && currentRoomPos) currentRoom.FreeUpRoomPos(currentRoomPos);

        //if we were already moving somewhere, free up the space we were last moving to and find our current room
        if (PathToRoom.Count > 0)
        {
            print("Diverting path!");
            currentRoom = PathToRoom[currentWaypoint];
            desiredRoom.FreeUpRoomPos(desiredRoomPos);
        }

        //  reserve the spot we are going to for ourselves
        desiredRoom = roomToGetTo;
        desiredRoomPos = desiredRoom.freeRoomPositions[0];
        roomToGetTo.OccupyRoomPos(desiredRoomPos);


        //calculate the path
        ClearPathToRoom();
        currentWaypoint = 0;
        PathToRoom = UnitPathfinding.instance.FindPath(currentRoom, desiredRoom, PlayerTankController.instance.TGeo._tankRoomConstellation);
        
        //make sure someone can enter the room we just left
        currentRoom = null;
        currentRoomPos = null;

        //start the movement
        UnitIsMoving = true;
        UnitSelected = false;
    }
    public void ClearPathToRoom()
    {
        PathToRoom.Clear();
    }
    private void MoveAlongPath()
    {
        if (PathToRoom.Count == 0) //stop the method if we dont even have a path
        {
            UnitIsMoving = false;
            return;
        }
        Room roomToMoveTo = PathToRoom[currentWaypoint];

        //calculate the local vector of our room relative to the tank
        VectorToGetTo = roomToMoveTo.transform.position - PlayerTankController.instance.transform.position;
        //set the z to 0 so our sprite doesnt disappear in the ground
        VectorToGetTo.z = 0;
        float distance = Vector2.Distance(transform.localPosition, VectorToGetTo);

        if ((distance < nextWayPointDistance) && !(currentWaypoint == PathToRoom.Count - 1))
        {
            currentWaypoint++;
        }
        else
        {
            //check if weve reached our end destination
            if (Vector3.Distance(transform.localPosition, VectorToGetTo) <= (UnitSpeed * Time.deltaTime))
            {
                transform.localPosition = VectorToGetTo;
                UnitIsMoving = false;
                WizardAnimator.SetFloat("Speed", 0);

                ClearPathToRoom();
                currentRoom = desiredRoom;
                currentRoomPos = desiredRoomPos;
            }
        }
        MoveUnitToRoom(roomToMoveTo);
    }

    private void MoveUnitToRoom(Room nextRoom)
    {
        //calculate local vector between wizard and the next position
        Vector3 moveVector = Vector3.Normalize(VectorToGetTo - transform.localPosition);

        // set Animator Values
        WizardAnimator.SetFloat("Speed", moveVector.sqrMagnitude);
        WizardAnimator.SetFloat("Horizontal", moveVector.x);
        WizardAnimator.SetFloat("Vertical", moveVector.y);

        float distance = Vector2.Distance(transform.position, VectorToGetTo);

        //move by this vector
        transform.localPosition += moveVector * UnitSpeed * Time.deltaTime;

    }
}