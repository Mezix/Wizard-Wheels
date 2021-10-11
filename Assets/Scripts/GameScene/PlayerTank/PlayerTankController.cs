﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    public static PlayerTankController instance;

    //  Important scripts

    public TankStats _tStats;
    public PlayerTankHealth THealth { get; private set; }
    public PlayerTankMovement TMov { get; private set; }
    public PlayerTankRotation TRot { get; private set; }
    public PlayerTankWeapons TWep { get; private set; }
    public TankGeometry TGeo { get; private set; }

    public string _tankName;
    public List<TechWizard> _wizardList = new List<TechWizard>();

    public bool _dying;
    public bool _dead;

    private void Awake()
    {
        References.PlayerTankController = instance = this;
        References.PlayerGameObject = gameObject;
        References.PlayerIsDead = false;
        InitEvents();
        TGeo = GetComponentInChildren<TankGeometry>();
        TMov = GetComponentInChildren<PlayerTankMovement>();
        TRot = GetComponentInChildren<PlayerTankRotation>();
        TWep = GetComponentInChildren<PlayerTankWeapons>();
        THealth = GetComponentInChildren<PlayerTankHealth>();
    }

    void Start()
    {
        TGeo.SpawnTank();
        TMov.InitTankMovement();
        TRot.InitTankRotation();
        TWep.InitWeapons();
        TWep.CreateWeaponsUI();
        InitTankStats();
        InitWizards();
    }

    private void Update()
    {
        if (!_dying) ;
        else
        {
            if (!_dead) SlowlyDie();
        }
    }
    private void InitEvents()
    {
        Events.instance.EnemyTankDestroyed += RemoveEnemyRoomFromWeapons;
    }
    private void RemoveEnemyRoomFromWeapons(GameObject enemy)
    {
        foreach (IWeapon wep in TWep.IWeaponArray)
        {
            if (!enemy || !wep.Room) return;
            if (wep.Room.transform.root.gameObject.Equals(enemy))
            {
                wep.ResetAim();
            }
        }
    }

    private void InitTankStats()
    {
        if (_tStats)
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
    private void DeselectAllWizards()
    {
        foreach (TechWizard wizard in _wizardList) wizard.UnitSelected = true;
    }
    public void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
    public void InitiateDeathBehaviour()
    {
        //  Send event to our enemies to remove the target of their weapons
        Events.instance.PlayerDying();
        References.PlayerIsDead = true;
        DeselectAllWizards();
        TWep.WeaponBehaviourInDeath();
        TMov.cruiseModeOn = false;
        _dying = true;
        StartCoroutine(DeathAnimation());
    }
    private void SlowlyDie()
    {
        if (TMov.velocity < 0.01f)
        {
            _dead = true;
        }
    }
    private IEnumerator DeathAnimation()
    {
        print("Player Tank Destroyed :(");

        List<GameObject> explosions = new List<GameObject>();
        while (!_dead)
        {
            GameObject explosion = Instantiate((GameObject)Resources.Load("SingleExplosion"));
            explosions.Add(explosion);
            explosion.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.0f, 1.0f), 0);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.435f);
        foreach (GameObject expl in explosions) Destroy(expl);
        Destroy(gameObject);

        Events.instance.PlayerDestroyed();
    }
}