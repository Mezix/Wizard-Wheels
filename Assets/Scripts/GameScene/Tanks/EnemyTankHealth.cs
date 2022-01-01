using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTankHealth : TankHealth
{
    public GameObject _healthBarParent;
    public List<Image> _allHPSegments;

    public override void InitHealth()
    {
        SetCurrentHealth(_maxHealth);
        CreateHealthBar(_maxHealth);
    }
    public void CreateHealthBar(int maxHealth)
    {
        if (_allHPSegments.Count > 0)
        {
            foreach (Image g in _allHPSegments) Destroy(g);
            _allHPSegments.Clear();
        }
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject tmp = Instantiate((GameObject)Resources.Load("HPSegment"));
            Image img = tmp.GetComponent<Image>();

            if (i == 0) img.sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Left", typeof(Sprite)) as Sprite;
            else if (i == maxHealth - 1) img.sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Right", typeof(Sprite)) as Sprite;
            else img.sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Middle", typeof(Sprite)) as Sprite;

            _allHPSegments.Add(img);
            tmp.transform.SetParent(_healthBarParent.transform, false);
        }

    }
    public override void TakeDamage(int dmg)
    {
        SetCurrentHealth(_currentHealth - dmg);
        if(_currentHealth <= 0 && !GetComponent<EnemyTankController>()._dying) GetComponent<EnemyTankController>().InitiateDeathBehaviour();
        UpdateHealthBar(_currentHealth, _maxHealth);
    }
    public void UpdateHealthBar(int current, int maxHealth)
    {
        current = Mathf.Max(0, current);
        for (int i = maxHealth - 1; i >= 0; i--)
        {
            if (i >= current)
            {
                if (i == 0) _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Broken Left", typeof(Sprite)) as Sprite;
                else if (i == maxHealth - 1) _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Broken Right", typeof(Sprite)) as Sprite;
                else _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Broken Middle", typeof(Sprite)) as Sprite;
            }
            else
            {
                if (i == 0) _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Left", typeof(Sprite)) as Sprite;
                else if (i == maxHealth - 1) _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Right", typeof(Sprite)) as Sprite;
                else _allHPSegments[i].sprite = Resources.Load("Art\\UI\\HP Bar\\HP Segment Middle", typeof(Sprite)) as Sprite;
            }
        }
    }
}
