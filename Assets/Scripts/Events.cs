using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public static Events current;
    private void Awake()
    {
        current = this;
    }
    public event Action<GameObject> EnemyTankDestroyed;
    public event Action PlayerTankDestroyed;
    public void EnemyDestroyed(GameObject enemyTank)
    {
        if (EnemyTankDestroyed != null)
        {
            EnemyTankDestroyed(enemyTank);
        }
    }
    public void PlayerDestroyed()
    {
        if (PlayerTankDestroyed != null)
        {
            PlayerTankDestroyed();
        }
    }
}
