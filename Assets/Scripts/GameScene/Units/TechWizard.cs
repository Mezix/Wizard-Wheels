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
                Ref.Path.SetPathToRoomWithMouse(GetComponent<AUnit>());
        }
        UpdateWizardUI();
    }
    private void FixedUpdate()
    {
        UnitBehaviour();
    }
    private void UnitBehaviour()
    {
        if (UnitIsMoving)
        {
            MoveAlongPath();
        }
    }
}