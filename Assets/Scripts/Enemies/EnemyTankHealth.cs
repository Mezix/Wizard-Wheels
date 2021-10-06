using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTankHealth : MonoBehaviour
{
    public int _maxHealth;
    public int _currentHealth;

    public GameObject _healthBarParent;
    public List<Image> _allHealthBarUnits;
    public GameObject _healthBarUnitPrefab;

    public void InitHealth()
    {
        SetCurrentHealth(_maxHealth);
        CreateHealthBar(_maxHealth);
    }
    public void SetCurrentHealth(int health)
    {
        _currentHealth = health;
    }
    public void CreateHealthBar(int maxHealth)
    {
        if (_allHealthBarUnits.Count > 0)
        {
            foreach (Image g in _allHealthBarUnits) Destroy(g);
            _allHealthBarUnits.Clear();
        }
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject tmp = Instantiate(_healthBarUnitPrefab);
            _allHealthBarUnits.Add(tmp.GetComponent<Image>());
            tmp.transform.SetParent(_healthBarParent.transform, false);
        }

    }
    public void TakeDamage(int dmg)
    {
        SetCurrentHealth(_currentHealth - dmg);
        if(_currentHealth <= 0) GetComponent<EnemyTankController>().InitiateDeathBehaviour();
        UpdateHealthBar(_currentHealth, _maxHealth);
    }
    public void UpdateHealthBar(int current, int maxHealth)
    {
        current = Mathf.Max(0, current);
        for (int i = 0; i < maxHealth - 1; i++)
        {
            SetHealthUnitStatus(i, true); //set all health to full
        }
        for (int i = maxHealth - 1; i > current - 1; i--)
        {
            SetHealthUnitStatus(i, false); //now set all the destroyed health
        }
    }
    public void SetHealthUnitStatus(int i, bool full)
    {
        if (full) _allHealthBarUnits[i].color = Color.white;
        else _allHealthBarUnits[i].color = Color.black;
    }
}
