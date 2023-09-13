using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public int _maxHealth;
    public int _currentHealth;
    public virtual void InitHealth()
    {
        SetCurrentHealth(_maxHealth);
    }
    public void SetCurrentHealth(int health)
    {
        _currentHealth = health;
    }
    public virtual void TakeDamage(int dmg)
    {
        print("Implement TakeDamage()");
    }
    public virtual void Heal(int dmg)
    {
        print("Implement Heal()");
    }
}