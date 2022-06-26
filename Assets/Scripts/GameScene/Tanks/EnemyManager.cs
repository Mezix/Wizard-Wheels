using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject _enemyPrefab;
    public List<EnemyTankController> _enemyTanks = new List<EnemyTankController>();
    public List<EnemyColor> EnemyColors;

    public List<EnemyIndicator> _enemyIndicators = new List<EnemyIndicator>();
    public Transform _enemyIndicatorParent;

    [Serializable]
    public class EnemyColor
    {
        public bool taken;
        public Color color;
        public GameObject enemyTakingColor;
    }
    private int maxEnemies;
    private void Update()
    {
        SpawnBehaviour();
    }
    private void Awake()
    {
        Ref.EM = this;
        Events.instance.EnemyTankDestroyed += EnemyDestroyed;
    }
    private void EnemyDestroyed(GameObject enemy)
    {
        EnemyTankController enemyTank = enemy.GetComponent<EnemyTankController>();
        if (_enemyTanks.Contains(enemyTank))
        {
            if (_enemyIndicators.Contains(enemyTank._indicator))
            {
                Destroy(enemyTank._indicator);
            }
            _enemyTanks.Remove(enemyTank);
            ReturnColor(enemy);
        }
    }

    private void Start()
    {
        maxEnemies = 0;
    }

    public void SpawnBehaviour()
    {
        SpawnEnemy();
    }
    private void SpawnEnemy()
    {
        //Check the number of enemies spawned at once
        if (_enemyTanks.Count > maxEnemies-1) return;

        //Get Bounds of the unit we want to spawn next

        //get the positions of all tanks in the game, as well as the players, and save them to be iterated over

        //get the bounds of these tanks

        //define the area enemies can be spawned in; outside the max zoom out level of the player

        Vector3 spawnPos;
        GameObject enemy = Instantiate(_enemyPrefab);
        EnemyTankController enemyTank = enemy.GetComponent<EnemyTankController>();
        enemyTank._tankColor = GetNextColor(enemy);
        _enemyTanks.Add(enemyTank);

        if (Ref.PCon)
        {
            spawnPos = Ref.PCon.transform.position + new Vector3(10, -5, 0);
        }
        else
        {
            spawnPos = new Vector3(0, 0, 0);
        }

        //Check if we are spawning something on an object
        spawnPos = CheckSpawnPos(spawnPos);
        enemy.transform.position = spawnPos;

        GameObject g = Instantiate((GameObject) Resources.Load("EnemyIndicator"));
        EnemyIndicator eIndicator = g.GetComponent<EnemyIndicator>();
        eIndicator.InitIndicator(_enemyIndicatorParent, enemyTank);
        enemyTank._indicator = eIndicator;
        _enemyIndicators.Add(eIndicator);
    }
    public void UntrackAllEnemyTanks()
    {
        foreach (EnemyTankController g in _enemyTanks) g.enemyUI.TrackTank(false);
    }
    private Color GetNextColor(GameObject enemy)
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
    private void ReturnColor(GameObject enemy)
    {
        foreach (EnemyColor c in EnemyColors)
        {
            if (c.enemyTakingColor.Equals(enemy))
            {
                c.taken = false;
                c.enemyTakingColor = null;
                //print("returning");
            }
        }
    }
    private Vector3 CheckSpawnPos(Vector3 checkPos)
    {
        //returns the same vector if it works, otherwise modifies the vector and returnsa viable position
        return checkPos;
    }
}
