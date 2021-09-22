using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    public static PlayerTankController instance;

    public TankMovement tMov;
    public TankRotation tRot;
    public TankWeapons tWep;
    public List<Wizard> wizardList = new List<Wizard>();

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        InitTank();
        InitWizards();
    }

    private void InitWizards()
    {
        foreach (Wizard w in GetComponentsInChildren<Wizard>())
        {
            wizardList.Add(w);
        }
    }

    private void InitTank()
    {
        tMov = GetComponentInChildren<TankMovement>();
        tRot = GetComponentInChildren<TankRotation>();
        tWep = GetComponentInChildren<TankWeapons>();
    }
}
