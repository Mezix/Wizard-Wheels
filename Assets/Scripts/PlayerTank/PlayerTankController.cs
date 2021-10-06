using System;
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
        instance = this;
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
    public void TakeDamage(int damage)
    {
        THealth.TakeDamage(damage);
    }
    public void InitiateDeathBehaviour()
    {
        print("tank destroyed");

        //  Send event to our enemies to remove the target of their weapons
        TWep.DeselectAllWeapons();
        TMov.cruiseModeOn = false;
        _dying = true;
    }
    private void SlowlyDie()
    {
        if (TMov.velocity < 0.01f)
        {
            _dead = true;
            StartCoroutine(DeathAnimation());
        }
    }
    private IEnumerator DeathAnimation()
    {
        print("Tank has died");
        List<GameObject> explosions = new List<GameObject>();
        for(int i = 0; i < 7; i++)
        {
            GameObject explosion = Instantiate((GameObject)Resources.Load("SingleExplosion"));
            explosions.Add(explosion);
            explosion.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), 0);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.435f);
        foreach (GameObject expl in explosions) Destroy(expl);
        Destroy(gameObject);
        //Play Explosion
    }
}
