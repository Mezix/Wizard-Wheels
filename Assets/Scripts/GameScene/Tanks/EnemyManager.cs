using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyEvent;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyTankController> _enemyTanks = new List<EnemyTankController>();
    public List<EnemyColor> EnemyColors;

    public List<EnemyIndicator> _enemyIndicators = new List<EnemyIndicator>();
    public Transform _enemyIndicatorParent;

    public EnemyEvent _enemyEvent;
    private List<EnemySpawn> enemiesToSpawn;
    private float timeUntilNextEnemySpawned;
    private int _enemyIndex;

    [Serializable]
    public class EnemyColor
    {
        public bool taken;
        public Color color;
        public EnemyTankController enemyTakingColor;
    }
    private void Awake()
    {
        REF.EM = this;
        Events.instance.EnemyTankDestroyed += EnemyDestroyed;
        Events.instance.EnemyTankDying += EnemyIsDying;
    }

    private void Start()
    {
        enemiesToSpawn = _enemyEvent.EnemyWaves;
        timeUntilNextEnemySpawned = 0;
        _enemyIndex = 0;
    }
    private void Update()
    {
        SpawnBehaviour();
    }
    public void SpawnBehaviour()
    {
        if (_enemyIndex >= enemiesToSpawn.Count) return;

        timeUntilNextEnemySpawned += Time.deltaTime;
        if(timeUntilNextEnemySpawned >= enemiesToSpawn[_enemyIndex]._delay)
        {
            timeUntilNextEnemySpawned = 0;
            SpawnEnemy(enemiesToSpawn[_enemyIndex]);
            _enemyIndex++;
        }

    }
    private void SpawnEnemy(EnemySpawn enemyToSpawn)
    {
        TankRoomConstellation _constellation = enemyToSpawn._tankRoomConstellation;
        TankStats stats = enemyToSpawn._tankStats;

        Vector3 spawnPos;
        EnemyTankController enemyTank = Instantiate(Resources.Load(GS.Enemy("EnemyTank"), typeof (EnemyTankController)) as EnemyTankController);

        enemyTank.TGeo._tankRoomConstellation = _constellation;
        enemyTank._tStats = stats;
        enemyTank._tankColor = GetNextColor(enemyTank);

        enemyTank.SpawnTank();
        _enemyTanks.Add(enemyTank);

        float spawnAngle = UnityEngine.Random.Range(0, 360f);
        float distanceFromPlayer = 30;
        if (REF.PCon)
        {
            spawnPos = REF.PCon.transform.position + new Vector3(distanceFromPlayer * Mathf.Cos(spawnAngle * Mathf.Deg2Rad), 
                                                                 distanceFromPlayer * Mathf.Sin(spawnAngle * Mathf.Deg2Rad), 0);

            //Check if we are spawning something on an object and change it if needed
            spawnPos = CheckSpawnPos(spawnPos);

            // Set rotation of wheels to movetowards player
            float angleBetweenTankAndPlayer = spawnAngle + 90;
            enemyTank.TRot.GetComponent<EnemyTankRotation>().SetRotationToAngle(angleBetweenTankAndPlayer);

            // Set Starting Speed to max
            enemyTank.TMov.currentSpeed = enemyTank._tStats._tankMaxSpeed;
        }
        else
        {
            spawnPos = Vector3.zero;
        }
        enemyTank.transform.position = spawnPos;

        //  Instantiate the enemy indicator
        EnemyIndicator eIndicator = Instantiate(Resources.Load(GS.Enemy("EnemyIndicator"), typeof (EnemyIndicator)) as EnemyIndicator);
        eIndicator.InitIndicator(_enemyIndicatorParent, enemyTank);
        enemyTank._indicator = eIndicator;
        _enemyIndicators.Add(eIndicator);
    }

    private void EnemyIsDying(EnemyTankController enemy)
    {
        ReturnColor(enemy);
    }
    private void EnemyDestroyed(EnemyTankController enemy)
    {
        if (_enemyTanks.Contains(enemy))
        {
            if (_enemyIndicators.Contains(enemy._indicator))
            {
                Destroy(enemy._indicator);
            }
            _enemyTanks.Remove(enemy);
        }
        Destroy(enemy.gameObject);
        CheckAllTanksDestroyed();
    }
    private void CheckAllTanksDestroyed()
    {
        if (_enemyIndex < enemiesToSpawn.Count) return;
        if (_enemyTanks.Count > 0) return;
        LevelManager.instance.CombatHasBeenWon();
    }
    public void UntrackAllEnemyTanks()
    {
        foreach (EnemyTankController g in _enemyTanks) g.enemyUI.TrackTank(false);
    }
    private Color GetNextColor(EnemyTankController enemy)
    {
        foreach(EnemyColor c in EnemyColors)
        {
            if (!c.taken)
            {
                c.taken = true;
                c.enemyTakingColor = enemy;
                return c.color;
            }
        }
        return Color.red;
    }
    private void ReturnColor(EnemyTankController enemy)
    {
        foreach (EnemyColor c in EnemyColors)
        {
            if (c.enemyTakingColor.Equals(enemy))
            {
                c.taken = false;
                c.enemyTakingColor = null;
                return;
            }
        }
    }
    private Vector3 CheckSpawnPos(Vector3 checkPos)
    {
        //returns the same vector if it works, otherwise modifies the vector and returnsa viable position
        return checkPos;
    }


}