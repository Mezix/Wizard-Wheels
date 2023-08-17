using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static PlayerData;

public class PlayerTankController : TankController
{
    public PlayerTankMovement TMov { get; private set; }
    public PlayerTankWeaponsAndSystems TWep { get; private set; }
    public List<PlayerWizardUI> _UIWizards = new List<PlayerWizardUI>();
    private RotationUpgradeField rotUpgrade;

    private void Awake()
    {
        rotUpgrade = GetComponent<RotationUpgradeField>();
        InitTank();
    }
    private void Start()
    {
        rotUpgrade.CreateUpgradeField();
        InitEvents();
    }
    public void InitTank()
    {
        REF.PCon = this;
        REF.PlayerGO = gameObject;
        REF.PDead = false;
        TGeo = GetComponentInChildren<TankGeometry>();
        TMov = GetComponentInChildren<PlayerTankMovement>();
        TRot = GetComponentInChildren<PlayerTankRotation>();
        TWep = GetComponentInChildren<PlayerTankWeaponsAndSystems>();
        THealth = GetComponentInChildren<PlayerTankHealth>();
    }
    public void SpawnTank()
    {
        TGeo.CreateTankGeometry();
        TGeo.AddScrapCollector();
        TWep.SetUpWeapons(true, _tankColor);
        TWep.SetUpSystems(true);
        TMov.InitSpeedStats();
        TMov.InitTankMovement();
        TRot.InitRotationSpeed();
        TRot.GetComponent<PlayerTankRotation>().InitTankRotation();
        TWep.CreateWeaponsUI();
        InitTankStats();
        _wizardData = DataStorage.Singleton.playerData.WizardList;
        SpawnWizards();
        SpawnWizardsUI();
        //TGeo.VisualizeMatrix();
    }

    private void SpawnWizardsUI()
    {
        foreach (AUnit u in _spawnedWizards)
        {
            u.PlayerWizardUI = REF.CombatUI.CreateWizardUI(u);
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
                REF.Path.SetPathToRoom(unit, unit.SavedRoomPos);
            }
        }
    }
    public void SaveAllWizardPositions()
    {
        int wizIndex = 0;
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
            //  Save the wizard data
            DataStorage.Singleton.playerData.WizardList[wizIndex].RoomPositionX = unit.SavedRoomPos._xPos;
            DataStorage.Singleton.playerData.WizardList[wizIndex].RoomPositionY = unit.SavedRoomPos._yPos;
            wizIndex++;
        }
        DataStorage.Singleton.playerData.WizardList = _wizardData;
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
            TWep.DeselectAllWeapons();
        }
    }

    private void InitEvents()
    {
        Events.instance.EnemyTankDying += RemoveEnemy;
    }
    private void RemoveEnemy(EnemyTankController enemy)
    {
        foreach (AWeapon wep in TWep.AWeaponArray)
        {
            if (!enemy || !wep.TargetedRoom) return;
            if (wep.TargetedRoom.transform.root.gameObject.Equals(enemy.gameObject))
            {
                wep.ResetAim();
            }
        }

        if(TMov._matchSpeed)
        {
            if(TMov.enemyToMatch.gameObject.Equals(enemy))
            {
                REF.CombatUI._engineUIScript.UnmatchSpeedUI();
                TMov.enemyToMatch = null;
                TMov._matchSpeed = false;
                TMov.TurnOnCruise(true);
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
        foreach (PlayerWizardUI ui in _UIWizards) ui.UpdateButton(false);
    }
    public override void TakeDamage(int damage)
    {
       // THealth.GetComponent<PlayerTankHealth>().TakeDamage(damage);
        REF.Cam.StartShake(0.1f, 0.1f);
    }
    public void InitiateDeathBehaviour()
    {
        if (REF.PDead) return;
        //  Send event to our enemies to remove the target of their weapons
        Events.instance.PlayerDying();
        REF.PDead = true;
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
            GameObject explosion = Instantiate((GameObject)Resources.Load(GS.Effects("SingleExplosion")));
            explosions.Add(explosion);
            explosion.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.0f, 1.0f), 0);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.435f);
        Destroy(gameObject);

        Events.instance.PlayerDestroyed();
    }
}