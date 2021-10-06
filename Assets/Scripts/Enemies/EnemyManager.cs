using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject _enemyPrefab;
    private List<GameObject> _enemies = new List<GameObject>();
    private int maxEnemies;
    private void Update()
    {
        SpawnEnemyBehaviour();
    }
    private void Awake()
    {
        Events.current.EnemyTankDestroyed += EnemyDestroyed;
    }
    private void EnemyDestroyed(GameObject enemy)
    {
        if (_enemies.Contains(enemy)) _enemies.Remove(enemy);
    }

    private void Start()
    {
        maxEnemies = 1;
    }

    public void SpawnEnemyBehaviour()
    {
        SpawnEnemy();
    }
    private void SpawnEnemy()
    {
        //Check the number of enemies spawned at once
        if (_enemies.Count > maxEnemies-1) return;

        Vector3 spawnPos = Vector3.zero;
        GameObject tmp = Instantiate(_enemyPrefab);
        _enemies.Add(tmp);
        if (PlayerTankController.instance)
        {
            spawnPos = PlayerTankController.instance.transform.position + new Vector3(10, -5, 0);
        }
        else
        {
            spawnPos = new Vector3(0, 0, 0);
        }

        //Check if we are spawning something on an object
        spawnPos = CheckSpawnPos(spawnPos);
        tmp.transform.position = spawnPos;
    }

    private Vector3 CheckSpawnPos(Vector3 checkPos)
    {
        //returns the same vector if it works, otherwise modifies the vector and returnsa viable position
        return checkPos;
    }
}
