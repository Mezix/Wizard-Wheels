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
    public event Action<GameObject> WizardOrWeaponClicked;
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
    public void PlayerDying()
    {
        if (PlayerIsDying != null)
        {
            PlayerIsDying();
        }
    }
    public void WizOrWepClicked(GameObject obj)
    {
        if (WizardOrWeaponClicked != null)
        {
            WizardOrWeaponClicked(obj);
        }
    }
}
