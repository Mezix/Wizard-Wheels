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

    private Transform desiredTransform;
    private Vector3 VectorToGetTo;
    public Room currentRoom;
    public Room desiredRoom;
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
        CalculateCurrentRoom();
    }
    private void FixedUpdate()
    {
        if (UnitIsMoving)
        {
            MoveAlongPath();
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
        if (desiredRoom) desiredRoom.FreeUpRoom(desiredTransform);

        //  reserve the spot we are going to for ourselves
        desiredTransform = roomToGetTo.freeRoomPositions[0];
        desiredRoom = roomToGetTo;

        roomToGetTo.TakeUpRoom(roomToGetTo.freeRoomPositions[0]);
        //calculate the path
        PathToRoom = UnitPathfinding.instance.FindPath(currentRoom, desiredRoom, PlayerTankController.instance.TGeo._tankRoomConstellation);

        //start the movement
        UnitIsMoving = true;
        UnitSelected = false;
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