using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTankHealth : MonoBehaviour
{
    public int _maxHealth;
    public int _currentHealth;

    public AudioSource fullHealthSound;
    public AudioSource healSound;

    public void InitHealth()
    {
        SetCurrentHealth(_maxHealth);
        Ref.UI.CreateHealthbar(_maxHealth);
    }
    public void SetCurrentHealth(int health)
    {
        _currentHealth = health;
    }
    public void TakeDamage(int dmg)
    {
        SetCurrentHealth(_currentHealth - dmg);
        if (_currentHealth <= 0) GetComponent<PlayerTankController>().InitiateDeathBehaviour();
        Ref.UI.UpdateHealthBar(_currentHealth, _maxHealth);
    }
}
