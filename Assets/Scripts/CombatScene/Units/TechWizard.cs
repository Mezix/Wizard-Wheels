using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechWizard : AUnit
{
    private void Awake()
    {
        Rend = GetComponentInChildren<SpriteRenderer>();
        defaultShader = Rend.material.shader;
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
            if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKey(KeyCode.LeftShift) && !MouseCursor.IsPointerOverUIElement())
                REF.Path.SetPathToRoomWithMouse(GetComponent<AUnit>());
        }
        UpdateWizardUI();
    }
    private void FixedUpdate()
    {
        UnitBehaviour();
    }
    private void UnitBehaviour()
    {
        if (_unitState.Equals(UnitState.Moving))
        {
            MoveAlongPath();
        }
        else if (CurrentRoom._currentHP < CurrentRoom._maxHP)
        {
            _unitState = UnitState.Repairing;
            WizardAnimator.SetBool("Interacting", true);
            bool repairsDone = CurrentRoom.RepairSlowly(0.5f);
            if(repairsDone) _unitState = UnitState.Idle;
        }
        else if (CurrentRoom._roomSystem != null)
        {
            if (CurrentRoom._roomSystem.RoomPosForInteraction.Equals(CurrentRoomPos))
            {
                _unitState = UnitState.Interacting;
                InteractWithRoom();
            }
        }
        else if (_unitState.Equals(UnitState.Idle))
        {
            Idle();
        }
    }
}