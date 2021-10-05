﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    public static PlayerTankController instance;

    //  Important scripts

    public TankStats _tStats;
    public TankHealth THealth { get; private set; }
    public PlayerTankMovement TMov { get; private set; }
    public PlayerTankRotation TRot { get; private set; }
    public PlayerTankWeapons TWep { get; private set; }
    public PlayerTankGeometry TGeo { get; private set; }

    public string _tankName;
    public List<TechWizard> _wizardList = new List<TechWizard>();

    private void Awake()
    {
        instance = this;
        TGeo = GetComponentInChildren<PlayerTankGeometry>();
        TMov = GetComponentInChildren<PlayerTankMovement>();
        TRot = GetComponentInChildren<PlayerTankRotation>();
        TWep = GetComponentInChildren<PlayerTankWeapons>();
        THealth = GetComponentInChildren<TankHealth>();
    }
    void Start()
    {
        TGeo.SpawnPlayerTank();
        TMov.InitTankMovement();
        TRot.InitTankRotation();
        TWep.InitWeapons();
        TWep.CreateWeaponsUI();
        InitTankStats();
        InitWizards();
    }

    private void InitTankStats()
    {
        if(_tStats)
        {
            THealth._maxHealth = _tStats._tankHealth;
        }
        else
        {
            THealth._maxHealth = 10;
        }
        THealth.InitHealth();
    }
    private void InitWizards()
    {
        foreach (TechWizard w in GetComponentsInChildren<TechWizard>())
        {
            _wizardList.Add(w);
        }
    }
}
