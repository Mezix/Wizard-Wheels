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
    public event Action<GameObject> EnemyTankDestroyed;
    public event Action PlayerTankDestroyed;
    public event Action PlayerIsDying;
    public event Action<GameObject> CheckDoubleClick;

    //  UI
    public event Action UpgradesSaved;
    public event Action UpgradesReverted;
    public event Action UpgradeScreenUpdated;

    public void EnemyDestroyed(GameObject enemyTank)
    {
        if (EnemyTankDestroyed != null) EnemyTankDestroyed(enemyTank);
    }
    public void PlayerDestroyed()
    {
        if (PlayerTankDestroyed != null) PlayerTankDestroyed();
    }
    public void PlayerDying()
    {
        if (PlayerIsDying != null)PlayerIsDying();
    }
    public void DoubleClickAttempted(GameObject obj)
    {
        if (CheckDoubleClick != null) CheckDoubleClick(obj);
    }
    public void SaveUpgrades()
    {
        if (UpgradesSaved != null) UpgradesSaved();
    }
    public void UpdateUpgradeScreen()
    {
        if (UpgradeScreenUpdated != null) UpgradeScreenUpdated();
    }
    public void RevertUpgrades()
    {
        if (UpgradesReverted != null) UpgradesReverted();
    }
}
