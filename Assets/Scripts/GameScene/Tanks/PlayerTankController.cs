using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankController : TankController
{
    public PlayerTankMovement TMov { get; private set; }
    public PlayerTankWeaponsAndSystems TWep { get; private set; }
    public List<PlayerWizardUI> _UIWizards = new List<PlayerWizardUI>();

    private void Awake()
    {
        InitTank();
    }

    public void InitTank()
    {
        Ref.PCon = this;
        Ref.PlayerGO = gameObject;
        Ref.PDead = false;
        InitEvents();
        TGeo = GetComponentInChildren<TankGeometry>();
        TMov = GetComponentInChildren<PlayerTankMovement>();
        TRot = GetComponentInChildren<PlayerTankRotation>();
        TWep = GetComponentInChildren<PlayerTankWeaponsAndSystems>();
        THealth = GetComponentInChildren<TankHealth>();
    }
    public void SpawnTank()
    {
        TGeo.CreateTankGeometry();
        TWep.SetUpWeapons(true, _tankColor);
        TWep.SetUpSystems(true);
        TMov.InitSpeedStats();
        TMov.InitTankMovement();
        TRot.InitRotationSpeed();
        TRot.GetComponent<PlayerTankRotation>().InitTankRotation();
        TWep.CreateWeaponsUI();
        InitTankStats();
        SpawnWizards();
        SpawnWizardsUI();
        //TGeo.VisualizeMatrix();
    }

    private void SpawnWizardsUI()
    {
        foreach (AUnit u in _spawnedWizards)
        {
            u.PlayerWizardUI = Ref.UI.CreateWizardUI(u);
            _UIWizards.Add(u.PlayerWizardUI);
        }
    }

    private void Update()
    {
        if (!_dying)
        {
            HandleWizardSelection();
        }
        else
        {
            if (!_dead) SlowlyDie();
        }
    }
    public void ReturnAllWizardsToSavedPositions()
    {
        foreach(AUnit unit in _spawnedWizards)
        {
            if(unit.SavedRoom && unit.SavedRoomPos)
            {
                Ref.Path.SetPathToRoom(unit, unit.SavedRoomPos);
            }
        }
    }
    public void SaveAllWizardPositions()
    {
        foreach (AUnit unit in _spawnedWizards)
        {
            if(unit.DesiredRoom && unit.DesiredRoom)
            {
                unit.SavedRoom = unit.DesiredRoom;
                unit.SavedRoomPos = unit.DesiredRoomPos;
            }
            else if(unit.CurrentRoom && unit.CurrentRoomPos)
            {
                unit.SavedRoom = unit.CurrentRoom;
                unit.SavedRoomPos = unit.CurrentRoomPos;
            }
        }
    }
    private void HandleWizardSelection()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(0);
            } 
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(6);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(7);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAllWizards();
                SelectWizard(8);
            }
        }
    }

    private void SelectWizard(int wizIndex)
    {
        if(wizIndex < _spawnedWizards.Count)
        {
            _spawnedWizards[wizIndex].UnitSelected = true;
        }
    }

    private void InitEvents()
    {
        Events.instance.EnemyTankDestroyed += RemoveEnemy;
    }
    private void RemoveEnemy(GameObject enemy)
    {
        foreach (AWeapon wep in TWep.AWeaponArray)
        {
            if (!enemy || !wep.TargetedRoom) return;
            if (wep.TargetedRoom.transform.root.gameObject.Equals(enemy))
            {
                wep.ResetAim();
            }
        }

        if(TMov._matchSpeed)
        {
            if(TMov.enemyToMatch.gameObject.Equals(enemy))
            {
                TMov.enemyToMatch = null;
                TMov._attemptingMatchingSpeed = false;
                TMov._matchSpeed = false;
                TMov.TurnOnCruise(true);
                Ref.UI.SetMatchSpeedButton(0);
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
        foreach (AUnit wizard in _spawnedWizards) wizard.UnitSelected = false;
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
        if (TMov.currentSpeed < 0.01f)
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
        //foreach (GameObject expl in explosions) Destroy(expl);
        Destroy(gameObject);

        Events.instance.PlayerDestroyed();
    }
}