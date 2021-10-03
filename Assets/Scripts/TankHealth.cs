using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public int _maxHealth;
    public int _currentHealth;

    public AudioSource fullHealthSound;
    public AudioSource healSound;

    public void InitHealth()
    {
        SetCurrentHealth(_maxHealth);
        UIScript.instance.CreateHealthbar(_maxHealth);
        UIScript.instance.UpdateHealthBar(5, _maxHealth);
    }
    public void SetCurrentHealth(int health)
    {
        _currentHealth = health;
    }
    public void TakeDamage()
    {

    }
}
