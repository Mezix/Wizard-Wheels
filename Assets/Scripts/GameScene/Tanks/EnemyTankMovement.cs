using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankMovement : TankMovement
{
    private void Awake()
    {
        InitTires();
    }
    void Update()
    {
        SetTireAnimationSpeed();
    }
    private void FixedUpdate()
    {
        Move();
    }
}
