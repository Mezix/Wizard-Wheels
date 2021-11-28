using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject _enemyPrefab;
    public List<GameObject> _enemies = new List<GameObject>();
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
        if (_enemies.Contains(enemy)) _enemies.Remove(enemy);
    }

    private void Start()
    {
        maxEnemies = 1;
    }

    public void SpawnBehaviour()
    {
        SpawnEnemy();
    }
    private void SpawnEnemy()
    {
        //Check the number of enemies spawned at once
        if (_enemies.Count > maxEnemies-1) return;

        //Get Bounds of the unit we want to spawn next

        //get the positions of all tanks in the game, as well as the players, and save them to be iterated over

        //get the bounds of these tanks

        //define the area enemies can be spawned in; outside the max zoom out level of the player

        Vector3 spawnPos;
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
