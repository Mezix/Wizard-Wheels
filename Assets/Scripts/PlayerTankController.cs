using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    public TankMovement tMov;
    public TankRotation tRot;
    public TankWeapons tWep;

    public static PlayerTankController instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        InitTank();
    }

    private void InitTank()
    {
        tMov = GetComponentInChildren<TankMovement>();
        tRot = GetComponentInChildren<TankRotation>();
        tWep = GetComponentInChildren<TankWeapons>();
    }
}
