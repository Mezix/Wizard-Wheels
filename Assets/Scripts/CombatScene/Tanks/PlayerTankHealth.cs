using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankHealth : TankHealth
{
    public bool _shouldBeInvincible;
    public AudioSource fullHealthSound;
    public AudioSource healSound;

    public override void InitHealth()
    {
        SetCurrentHealth(_maxHealth);
        REF.CombatUI.CreateHealthbar(_maxHealth);
    }
    public override void TakeDamage(int dmg)
    {
        if (_shouldBeInvincible)
        {
            Debug.Log("Would take " + dmg + " damage, but player is invincible!");
            return;
        }
        SetCurrentHealth(_currentHealth - dmg);
        if (_currentHealth <= 0) GetComponent<PlayerTankController>().InitiateDeathBehaviour();
        REF.CombatUI.UpdateHealthBar(_currentHealth, _maxHealth);
    }
}
