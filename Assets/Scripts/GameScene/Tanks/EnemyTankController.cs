using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EnemyTankController : TankController
{
    public EnemyTankMovement TMov { get; private set; }
    public EnemyTankWeaponsAndSystems TWep { get; private set; }
    public EnemyUI enemyUI;
    public EnemyIndicator _indicator;

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
        //SpawnTanks();
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
        TRot.InitRotationSpeed();
        SpawnWizards();
        SpawnUI();
        InitTankStats();
    }

    private void SpawnUI()
    {
        GameObject ui = (GameObject) Instantiate(Resources.Load(GS.Enemy("EnemyUI")));
        enemyUI = ui.GetComponent<EnemyUI>();
        enemyUI.transform.SetParent(transform);
        enemyUI.transform.localPosition = new Vector2(0, (0.5f * 0.5f * TGeo._tankRoomConstellation._savedYSize) + 1f);
        THealth.GetComponent<EnemyTankHealth>()._healthBarParent = enemyUI.hpBar;
    }
    private void Update()
    {
        if (!_dying) EnemyBehaviour();
        /*else
        {
            if (!_dead) SlowlyDie();
        }*/
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
        enemyUI.InitUI(_tankName, TMov);
    }
    public override void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
    public void EnemyBehaviour()
    {
        //Maintain a certain distance away from us
        TMov.Accelerate();
        if(!REF.PDead) TWep.AcquireTargetsForAllWeapons();
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
        TMov.deceleration *= 3;

        //  Send event to our player to remove the target of its weapons
        Events.instance.EnemyIsDying(gameObject);
        StartCoroutine(DeathAnimation());
    }
    private void SlowlyDie()
    {
        TMov.Decelerate();
        if(TMov.currentSpeed < 0.05f)
        {
            _dead = true;
        }
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
            GameObject explosion = Instantiate((GameObject)Resources.Load(GS.Effects("SingleExplosion")));
            explosions.Add(explosion);
            explosion.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.0f, 1.0f), 0);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.435f);

        ScrapPile scrap = Instantiate(Resources.Load(GS.Prefabs("ScrapPile"), typeof(ScrapPile)) as ScrapPile);
        scrap.transform.position = transform.position;
        scrap.InitScrap(UnityEngine.Random.Range(_tStats._scrapDropAmount, _tStats._scrapDropAmount + 10));
        Debug.Log(_tStats._tankName + " has been destroyed.");
        Events.instance.EnemyDestroyed(gameObject);
        Destroy(gameObject);
    }
}
