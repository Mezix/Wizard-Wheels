using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public static Events instance;
    private void Awake()
    {
        instance = this;
    }
    public event Action<EnemyTankController> EnemyTankDestroyed;
    public event Action<EnemyTankController> EnemyTankDying;
    public event Action PlayerTankDestroyed;
    public event Action PlayerIsDying;
    public event Action<GameObject> CheckDoubleClick;

    //  UI
    public event Action UpgradesSaved;
    public event Action UpgradesReverted;
    public event Action UpgradeScreenUpdated;

    public void EnemyDestroyed(EnemyTankController enemyTank)
    {
        EnemyTankDestroyed?.Invoke(enemyTank);
    }
    public void EnemyIsDying(EnemyTankController enemyTank)
    {
        EnemyTankDying?.Invoke(enemyTank);
    }
    public void PlayerDestroyed()
    {
        PlayerTankDestroyed?.Invoke();
    }
    public void PlayerDying()
    {
        PlayerIsDying?.Invoke();
    }
    public void DoubleClickAttempted(GameObject obj)
    {
        CheckDoubleClick?.Invoke(obj);
    }
    public void SaveUpgrades()
    {
        UpgradesSaved?.Invoke();
    }
    public void UpdateUpgradeScreen()
    {
        UpgradeScreenUpdated?.Invoke();
    }
    public void RevertUpgrades()
    {
        UpgradesReverted?.Invoke();
    }
}
