using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EnemyTankController : TankController
{
    public EnemyTankMovement TMov { get; private set; }
    public EnemyTankWeaponsAndSystems TWep { get; private set; }
    public EnemyUI enemyUI;
    public EnemyIndicator _indicator;

    public NavMeshAgent _navMeshAgent;
    public int scrapDropAmount = 50;
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
        _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
    }

    public void SpawnTank()
    {
        InitEvents();
        TGeo.CreateTankGeometry();
        TWep.SetUpWeapons(false, _tankColor);
        TWep.SetUpSystems(false);
        TWep.CreateWeaponsUI();
        TMov.InitSpeedStats();
        TMov.InitTires();
        InitNavMeshStats();
        TRot.InitRotationSpeed();
        SpawnWizards();
        SpawnUI();
        InitTankStats();
    }

    private void SpawnUI()
    {
        GameObject ui = (GameObject) Instantiate(Resources.Load(GS.Enemy("EnemyUI")));
        enemyUI = ui.GetComponent<EnemyUI>();
        enemyUI.transform.SetParent(transform, false);
        enemyUI._allObjects.transform.localPosition = new Vector2(0, (0.5f * TGeo._vehicleData.SavedYSize * 100));
        THealth.GetComponent<EnemyTankHealth>()._healthBarParent = enemyUI.hpBar;
    }
    private void Update()
    {
        if (!_dying) EnemyBehaviour();
        enemyUI.KeepLevel(TMov);
    }
    private void InitEvents()
    {
        Events.instance.PlayerIsDying += StopAimingAtPlayer;
    }
    private void InitTankStats()
    {
        if(_vehicleInfo != null)
        {
            THealth._maxHealth = _vehicleInfo.TankHealth;
            _tankName = _vehicleInfo.TankName;
        }
        else
        {
            THealth._maxHealth = 10;
            _tankName = "DefaultEnemy";
            print("Enemy has no stats, adding defaults!");
        }
        THealth.InitHealth();
        enemyUI.InitUI(_tankName, TMov);
    }
    private void InitNavMeshStats()
    {
        _navMeshAgent.speed = _vehicleInfo.TankMaxSpeed;
        _navMeshAgent.angularSpeed = _vehicleInfo.RotationSpeed;
        _navMeshAgent.acceleration = _vehicleInfo.TankAccel / Time.deltaTime;
        _navMeshAgent.radius = TGeo._vehicleData.SavedXSize/2f;
        _navMeshAgent.height = TGeo._vehicleData.SavedYSize/2f;
        _navMeshAgent.baseOffset = TGeo._vehicleData.SavedYSize/2f;

        _navMeshAgent.stoppingDistance = _navMeshAgent.radius + REF.PCon.TGeo._vehicleData.SavedXSize;
    }
    public override void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
    public void EnemyBehaviour()
    {
        if(!REF.PDead)
        {
            _navMeshAgent.SetDestination(REF.PlayerGO.transform.position);
            TRot.GetComponent<EnemyTankRotation>().SetRotationToAngle(90 + HM.GetEulerAngle2DBetween(Vector3.zero, _navMeshAgent.velocity));
            TWep.AcquireTargetsForAllWeapons();
        }
    }
    private void StopAimingAtPlayer()
    {
        TWep.ResetAllWeapons();
    }

    //  Death

    public void InitiateDeathBehaviour()
    {
        REF.TM.TriggerHitStop(0.75f, 0.1f, 0.5f, 0.5f);
        TWep.WeaponBehaviourInDeath();
        _dying = true;
        TMov.deceleration *= 3;

        //  Send event to our player to remove the target of its weapons
        Events.instance.EnemyIsDying(this);
        StartCoroutine(DeathAnimation());
    }
    private IEnumerator DeathAnimation()
    {
        List<GameObject> explosions = new List<GameObject>();
        while (!_dead)
        {
            TMov.Decelerate();
            if (TMov.currentSpeed < 0.05f)
            {
                _dead = true;
            }
            GameObject explosion = Instantiate((GameObject)Resources.Load(GS.Effects("NormalExplosion2")));
            explosions.Add(explosion);
            explosion.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.0f, 1.0f), 0);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.435f);

        LootCrate scrap = Instantiate(Resources.Load(GS.Prefabs("LootCrate"), typeof(LootCrate)) as LootCrate);
        scrap.transform.position = transform.position;
        scrap.InitScrap(UnityEngine.Random.Range(scrapDropAmount, scrapDropAmount + 10));
        //Debug.Log(_tStats._tankName + " has been destroyed.");
        Events.instance.EnemyDestroyed(this);
    }
}
