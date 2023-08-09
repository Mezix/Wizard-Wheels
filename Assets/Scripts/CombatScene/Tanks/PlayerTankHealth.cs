using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankHealth : TankHealth
{
    public AudioSource fullHealthSound;
    public AudioSource healSound;

    public override void InitHealth()
    {
        SetCurrentHealth(_maxHealth);
        REF.CombatUI.CreateHealthbar(_maxHealth);
    }
    public override void TakeDamage(int dmg)
    {
        SetCurrentHealth(_currentHealth - dmg);
        if (_currentHealth <= 0) GetComponent<PlayerTankController>().InitiateDeathBehaviour();
        REF.CombatUI.UpdateHealthBar(_currentHealth, _maxHealth);
    }
}
