using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AUnit : MonoBehaviour
{
    public UnitStats _unitStats;
    public string UnitName { get; set; }
    public string UnitClass { get; set; }
    public float UnitHealth { get; set; }
    public float UnitSpeed { get; set; }
    public SpriteRenderer Rend { get; set; }
    protected Shader defaultShader;
    public Animator WizardAnimator { get; set; }
    public bool UnitSelected { get; set; }
    public bool UnitIsMoving { get; set; }
    public GameObject UnitObj { get; set; }
    public PlayerWizardUI PlayerWizardUI { get; set; }
    public int Index { get; set; }

    //  Wizard UI

    public GameObject WizardUI;
    public Text UIName;
    public Image WizardHealthbar;
    public bool _showUI;

    //  Pathfinding

    [HideInInspector]
    public Vector3 RoomLocalPos;
    [HideInInspector]
    public Room CurrentRoom;
    [HideInInspector]
    public RoomPosition CurrentRoomPos;
    [HideInInspector]
    public Room DesiredRoom;
    [HideInInspector]
    public RoomPosition DesiredRoomPos;
    [HideInInspector]
    public Room SavedRoom;
    [HideInInspector]
    public RoomPosition SavedRoomPos;
    [HideInInspector]
    public List<RoomPosition> PathToRoom;
    [HideInInspector]
    public int CurrentWaypoint; //the index of our path
    [HideInInspector]
    private GameObject movingToPosIndicator;

    public void InitUnit()
    {
        defaultShader = Rend.material.shader;
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

        UIName.text = UnitName;
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

    public void Highlight()
    {
        _showUI = true;
        Rend.material.shader = (Shader) Resources.Load("Shaders\\SpriteOutline");
        Rend.material.SetFloat("Vector1_53CFC1A5", 0.05f);
        Rend.material.SetColor("Color_B1427637", Color.red);
    }
    public void DeHighlight()
    {
        _showUI = false;
        Rend.material.shader = defaultShader;
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
            //Debug.Log("destination reached");
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
        RoomLocalPos = nextRoomPos.transform.position - transform.parent.position;
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
        if (movingToPosIndicator)
        {
            Destroy(movingToPosIndicator);
            movingToPosIndicator = null;
        }
    }
    public void SetNextPosIndicator(RoomPosition rPos)
    {
        if (!rPos) return;
        if (movingToPosIndicator) Destroy(movingToPosIndicator);

        movingToPosIndicator = Instantiate((GameObject)Resources.Load("UnitMovingToIndicator"));
        movingToPosIndicator.transform.parent = rPos.transform;
        movingToPosIndicator.transform.localPosition = Vector3.zero;
    }
    public void ClearUnitPath()
    {
        PathToRoom.Clear();
    }
    protected void UpdateWizardUI()
    {
        //TODO: add unit health script

        WizardUI.SetActive(_showUI);
        if (PlayerWizardUI)
        {
            PlayerWizardUI._UIWizardHealthbarFill.fillAmount = Mathf.Min(1, UnitHealth / UnitHealth);
            PlayerWizardUI.UpdateButton(UnitSelected);
        }
        WizardHealthbar.fillAmount = Mathf.Min(1, UnitHealth / UnitHealth);
    }
}
