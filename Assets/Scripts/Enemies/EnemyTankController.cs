using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyTankController : MonoBehaviour, IEnemy
{
    //  Important scripts

    public TankStats TStats;
    public EnemyTankHealth THealth { get; private set; }
    public EnemyTankMovement TMov { get; private set; }
    public EnemyTankRotation TRot { get; private set; }
    public TankGeometry TGeo { get; private set; }

    public string _tankName;
    public List<TechWizard> _wizardList = new List<TechWizard>();

    private void Awake()
    {
        THealth = GetComponentInChildren<EnemyTankHealth>();
        TMov = GetComponentInChildren<EnemyTankMovement>();
        TRot = GetComponentInChildren<EnemyTankRotation>();
        TGeo = GetComponentInChildren<TankGeometry>();
    }
    void Start()
    {
        InitTankStats();
        InitWizards();
        TGeo.SpawnTank();
    }
    private void Update()
    {
        EnemyBehaviour();
    }
    public void EnemyBehaviour()
    {
        TRot.RotateTankLeft();
        TMov.Accelerate();
    }
    private void InitTankStats()
    {
        if(TStats)
        {
            THealth._maxHealth = TStats._tankHealth;
        }
        else
        {
            THealth._maxHealth = 10;
            print("Enemy has no stats");
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
    public void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
    public void DestroyTank()
    {
        print("tank destroyed");
    }
}
