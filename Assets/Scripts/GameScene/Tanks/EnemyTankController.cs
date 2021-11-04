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
    public EnemyTankWeaponsAndSystems TWep { get; private set; }
    public TankGeometry TGeo { get; private set; }

    public string _tankName;
    public Text tankNameText;
    public List<IUnit> _wizardList = new List<IUnit>();

    public bool _dying;
    public bool _dead;

    public Button TrackCameraButton;

    private void Awake()
    {
        THealth = GetComponentInChildren<EnemyTankHealth>();
        TMov = GetComponentInChildren<EnemyTankMovement>();
        TRot = GetComponentInChildren<EnemyTankRotation>();
        TWep = GetComponentInChildren<EnemyTankWeaponsAndSystems>();
        TGeo = GetComponentInChildren<TankGeometry>();
    }
    void Start()
    {
        InitEvents();
        TGeo.SpawnTank();
        TWep.InitWeaponsAndSystems(TGeo._tankRoomConstellation);
        TWep.CreateWeaponsUI();
        InitTankStats();
        InitWizards();

        InitTrackCameraButton();
    }

    private void InitTrackCameraButton()
    {
        TrackCameraButton.onClick = new Button.ButtonClickedEvent();
        TrackCameraButton.onClick.AddListener(() => Ref.Cam.SetTrackedVehicleToEnemy(transform));
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
        foreach (IUnit w in GetComponentsInChildren<IUnit>())
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
        _dying = true;
        TWep.WeaponBehaviourInDeath();

        //  Send event to our player to remove the target of its weapons
        Events.instance.EnemyDestroyed(gameObject);
        StartCoroutine(DeathAnimation());
        print("enemy tank being destroyed");
    }
    private void SlowlyDie()
    {
        TMov.Decelerate();
        if(TMov.velocity < 0.01f)
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
        foreach (GameObject expl in explosions) Destroy(expl);

        Events.instance.EnemyDestroyed(gameObject);
        Destroy(gameObject);
    }
}
