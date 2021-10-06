using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EnemyTankController : MonoBehaviour, IEnemy
{
    //  Important scripts

    public TankStats TStats;
    public EnemyTankHealth THealth { get; private set; }
    public EnemyTankMovement TMov { get; private set; }
    public EnemyTankRotation TRot { get; private set; }
    public EnemyTankWeapons TWep { get; private set; }
    public TankGeometry TGeo { get; private set; }

    public string _tankName;
    public Text tankNameText;
    public List<TechWizard> _wizardList = new List<TechWizard>();

    public bool _dying;
    public bool _dead;

    private void Awake()
    {
        THealth = GetComponentInChildren<EnemyTankHealth>();
        TMov = GetComponentInChildren<EnemyTankMovement>();
        TRot = GetComponentInChildren<EnemyTankRotation>();
        TWep = GetComponentInChildren<EnemyTankWeapons>();
        TGeo = GetComponentInChildren<TankGeometry>();
    }
    void Start()
    {
        TGeo.SpawnTank();
        TWep.InitWeapons();
        TWep.CreateWeaponsUI();
        InitTankStats();
        InitWizards();
    }
    private void Update()
    {
        if (!_dying) EnemyBehaviour();
        else
        {
            if(!_dead) SlowlyDie();
        }
    }
    private void InitTankStats()
    {
        if(TStats)
        {
            THealth._maxHealth = TStats._tankHealth;
            _tankName = TStats._tankName;
        }
        else
        {
            THealth._maxHealth = 10;
            _tankName = "DefaultEnemy";
            print("Enemy has no stats, adding defaults!");
        }
        THealth.InitHealth();
        tankNameText.text = _tankName;
    }
    private void InitWizards()
    {
        foreach (TechWizard w in GetComponentsInChildren<TechWizard>())
        {
            _wizardList.Add(w);
        }
    }
    public void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
    public void EnemyBehaviour()
    {
        //TRot.RotateTankLeft();
        TMov.Accelerate();
        TWep.FireAllWeapons();
    }
    public void InitiateDeathBehaviour()
    {
        print("tank destroyed");

        //  Send event to our player to remove the target of its weapons
        
        _dying = true;
    }
    private void SlowlyDie()
    {
        TMov.Decelerate();
        if(TMov.velocity < 0.01f)
        {
            _dead = true;
            StartCoroutine(DeathAnimation());
        }
    }
    private IEnumerator DeathAnimation()
    {
        print("Tank has died");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        //Play Explosion
    }
}
