using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyEvent;
using static PlayerData;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyTankController> _enemyTanks = new List<EnemyTankController>();
    public List<EnemyColor> EnemyColors;

    public List<EnemyIndicator> _enemyIndicators = new List<EnemyIndicator>();
    public Transform _enemyIndicatorParent;

    public int eventToSpawn;
    public List<EnemyEvent> _allPossibleEnemyEvents = new List<EnemyEvent>();
    private List<EnemySpawn> enemiesToSpawn;
    private float timeUntilNextEnemySpawned;
    private int _enemyIndex;

    [Serializable]
    public class EnemyColor
    {
        public bool taken;
        public Color color;
        public int enemyTakingColorID;
    }
    private void Awake()
    {
        REF.EM = this;
        Events.instance.EnemyTankDestroyed += EnemyDestroyed;
        Events.instance.EnemyTankDying += EnemyIsDying;
    }

    private void Start()
    {
        //eventToSpawn = UnityEngine.Random.Range(0, _allPossibleEnemyEvents.Count);
        enemiesToSpawn = _allPossibleEnemyEvents[eventToSpawn].EnemyWaves;
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
        VehicleConstellation _constellation = enemyToSpawn._tankRoomConstellation;
        VehicleInfo vehicleInfo = ConvertVehicleStatsToVehicleInfo(enemyToSpawn._tankStats);

        Vector3 spawnPos;
        EnemyTankController enemyTank = Instantiate(Resources.Load(GS.Enemy("EnemyTank"), typeof (EnemyTankController)) as EnemyTankController);
        enemyTank.TGeo._vehicleData = ConvertVehicleConstellationToVehicleData(_constellation);
        enemyTank._vehicleInfo = vehicleInfo;
        enemyTank._tankColor = GetNextColor(enemyTank.GetInstanceID());

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
            enemyTank.TMov.currentSpeed = enemyTank._vehicleInfo.TankMaxSpeed;
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
        ReturnColor(enemy.GetInstanceID());
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
        CombatSceneManager.instance.CombatHasBeenWon();
    }
    public void UntrackAllEnemyTanks()
    {
        foreach (EnemyTankController g in _enemyTanks) g.enemyUI.TrackTank(false);
    }
    //private Color GetNextColor(EnemyTankController enemy)
    private Color GetNextColor(int enemyID)
    {
        foreach(EnemyColor c in EnemyColors)
        {
            if (!c.taken)
            {
                c.taken = true;
                c.enemyTakingColorID = enemyID;
                return c.color;
            }
        }
        return Color.red;
    }
    //private void ReturnColor(EnemyTankController enemy)
    private void ReturnColor(int enemyID)
    {
        foreach (EnemyColor c in EnemyColors)
        {
            if (c.enemyTakingColorID == enemyID)
            {
                c.taken = false;
                c.enemyTakingColorID = -666;
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