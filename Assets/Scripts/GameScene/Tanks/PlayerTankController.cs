using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankController : TankController
{
    public static PlayerTankController instance;

    //  Important scripts

    public PlayerTankHealth THealth { get; private set; }
    public PlayerTankMovement TMov { get; private set; }
    public PlayerTankWeaponsAndSystems TWep { get; private set; }

    public List<UIWizard> _UIWizards = new List<UIWizard>();

    private void Awake()
    {
        Ref.PCon = instance = this;
        Ref.PlayerGO = gameObject;
        Ref.PDead = false;
        InitEvents();
        TGeo = GetComponentInChildren<TankGeometry>();
        TMov = GetComponentInChildren<PlayerTankMovement>();
        TRot = GetComponentInChildren<PlayerTankRotation>();
        TWep = GetComponentInChildren<PlayerTankWeaponsAndSystems>();
        THealth = GetComponentInChildren<PlayerTankHealth>();
    }

    void Start()
    {
        TGeo.SpawnTank();
        TMov.InitTankMovement();
        TRot.GetComponent<PlayerTankRotation>().InitTankRotation();
        TWep.InitWeaponsAndSystems();
        TWep.CreateWeaponsUI();
        InitTankStats();
        SpawnWizards();
        SpawnWizardsUI();
        //TGeo.VisualizeMatrix();
    }

    private void SpawnWizardsUI()
    {
        foreach (IUnit u in _spawnedWizards)
        {
            u.UIWizard = Ref.UI.CreateWizardUI(u);
            _UIWizards.Add(u.UIWizard);
        }
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
    public void DeselectAllWizards()
    {
        foreach (IUnit wizard in _spawnedWizards) wizard.UnitSelected = false;
    }
    public void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
    public void InitiateDeathBehaviour()
    {
        //  Send event to our enemies to remove the target of their weapons
        Events.instance.PlayerDying();
        Ref.PDead = true;
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