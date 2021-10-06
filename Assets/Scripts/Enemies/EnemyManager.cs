using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject _enemyPrefab;
    private List<GameObject> _enemies = new List<GameObject>();

    private void Update()
    {
        
    }
    private void Start()
    {
        SpawnEnemy();
    }
    private void SpawnEnemy()
    {
        GameObject tmp = Instantiate(_enemyPrefab);
        _enemies.Add(tmp);
        if (PlayerTankController.instance) tmp.transform.position = PlayerTankController.instance.transform.position + new Vector3(10,-5,0);
        else tmp.transform.position = new Vector3(0, 0, 0);
    }
}
