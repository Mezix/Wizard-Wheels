﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EnemyTankController : TankController
{
    public EnemyTankMovement TMov { get; private set; }
    public EnemyTankWeaponsAndSystems TWep { get; private set; }
    private EnemyUI enemyUI;

    private void Awake()
    {
        InitEnemyScripts();
    }
    public void InitEnemyScripts()
    {
        THealth = GetComponentInChildren<TankHealth>();
        TMov = GetComponentInChildren<EnemyTankMovement>();
        TRot = GetComponentInChildren<TankRotation>();
        TWep = GetComponentInChildren<EnemyTankWeaponsAndSystems>();
        TGeo = GetComponentInChildren<TankGeometry>();
    }

    void Start()
    {
        InitEvents();
        TGeo.CreateTankGeometry();
        TWep.SetUpWeapons(false, _tankColor);
        TWep.SetUpSystems(false);
        TWep.CreateWeaponsUI();
        TMov.InitTires();
        SpawnWizards();
        SpawnUI();
        InitTankStats();
    }

    private void SpawnUI()
    {
        GameObject ui = (GameObject) Instantiate(Resources.Load("EnemyUI"));
        enemyUI = ui.GetComponent<EnemyUI>();
        enemyUI.transform.SetParent(transform);
        enemyUI.transform.localPosition = new Vector2(0, (0.5f * 0.5f * TGeo._tankRoomConstellation.YTilesAmount) + 0.5f);
        THealth.GetComponent<EnemyTankHealth>()._healthBarParent = enemyUI.hpBar;
    }

    private void Update()
    {
        if (!_dying) EnemyBehaviour();
        else
        {
            if(!_dead) SlowlyDie();
        }
    }
    private void InitEvents()
    {
        Events.instance.PlayerIsDying += StopAimingAtPlayer;
    }
    private void InitTankStats()
    {
        if(_tStats)
        {
            THealth._maxHealth = _tStats._tankHealth;
            _tankName = _tStats._tankName;
        }
        else
        {
            THealth._maxHealth = 10;
            _tankName = "DefaultEnemy";
            print("Enemy has no stats, adding defaults!");
        }
        THealth.InitHealth();
        enemyUI.tankNameText.text = _tankName;
        enemyUI.ScaleTankHealth(THealth._maxHealth);
    }
    public void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
    public void EnemyBehaviour()
    {
        //Maintain a certain distance away from us
        TMov.Accelerate();
        if(!Ref.PDead) TWep.AcquireTargetsForAllWeapons();
    }
    private void StopAimingAtPlayer()
    {
        TWep.ResetAllWeapons();
    }

    //  Death

    public void InitiateDeathBehaviour()
    {
        TWep.WeaponBehaviourInDeath();
        _dying = true;

        //  Send event to our player to remove the target of its weapons
        Events.instance.EnemyDestroyed(gameObject);
        StartCoroutine(DeathAnimation());
        print("enemy tank being destroyed");
    }
    private void SlowlyDie()
    {
        TMov.Decelerate();
        if(TMov.currentSpeed < 0.01f)
        {
            _dead = true;
        }
    }
    private IEnumerator DeathAnimation()
    {
        List<GameObject> explosions = new List<GameObject>();
        while (!_dead)
        {
            GameObject explosion = Instantiate((GameObject)Resources.Load("SingleExplosion"));
            explosions.Add(explosion);
            explosion.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.0f, 1.0f), 0);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.435f);

        Events.instance.EnemyDestroyed(gameObject);
        Destroy(gameObject);
    }
}
