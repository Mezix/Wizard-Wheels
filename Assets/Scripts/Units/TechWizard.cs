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
    private Vector3 localPositionToMoveTo;

    private void Awake()
    {
        WizardRenderer = GetComponentInChildren<SpriteRenderer>();
        WizardAnimator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        InitUnit();
    }
    private void Update()
    {
        if(UnitSelected)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) DeterminePathToRoom();
        }
    }
    private void FixedUpdate()
    {
        if (UnitIsMoving)
        {
            MoveUnit(localPositionToMoveTo);
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
        localPositionToMoveTo = transform.position;
    }

    //  Move Unit

    public void DeterminePathToRoom()
    {
        localPositionToMoveTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        localPositionToMoveTo.z = 0; 
        //calculate the local vector of our mouse position relative to the tank
        localPositionToMoveTo -= PlayerTankController.instance.transform.position;
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
