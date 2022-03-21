using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationUpgradeField : MonoBehaviour
{
    private UIUpgradeField _upgradeField;
    public int _rotationLevel;
    public int _tempLevel;
    public int _maxLevel;
    private float weaponRotationIncreaseModifier;

    private List<int> _upgradeLevels = new List<int>();

    private void Awake()
    {
        _tempLevel = _rotationLevel = 0;
        _maxLevel = 3;
        weaponRotationIncreaseModifier = 0.75f;
        InitUpgradeLevels();
    }
    private void InitUpgradeLevels()
    {
        _upgradeLevels.Add(0);
        _upgradeLevels.Add(30);
        _upgradeLevels.Add(50);
        _upgradeLevels.Add(70);
    }
    public void CreateUpgradeField()
    {
        UIUpgradeField ui = Ref.UI._upgradeScreen.CreateUpgradeField();
        _upgradeField = ui;

        ui.InitUpgradeField("Joint Fluidity", _rotationLevel, _maxLevel, _upgradeLevels);
        ui._upgradeButton.onClick.AddListener(() => Upgrade());
        ui._downgradeButton.onClick.AddListener(() => Downgrade());

        Events.instance.UpgradesSaved += SaveChanges;
        Events.instance.UpgradesReverted += RevertChanges;
    }
    public void Upgrade()
    {
        if (_tempLevel >= _maxLevel) return;
        if (Ref.UI._upgradeScreen._remainingScrap <= _upgradeLevels[_tempLevel+1]) return;
        _tempLevel++;
        Ref.UI._upgradeScreen.RemoveTempScrap(_upgradeLevels[_tempLevel]);
        _upgradeField.SetTempLevel(_tempLevel);
    }
    public void Downgrade()
    {
        if (_tempLevel <= _rotationLevel) return;
        _tempLevel--;
        Ref.UI._upgradeScreen.AddTempScrap(_upgradeLevels[_tempLevel+1]);
        _upgradeField.SetTempLevel(_tempLevel);
    }
    public void SaveChanges()
    {
        _rotationLevel = _tempLevel;
        Ref.PCon.GetComponent<PlayerTankRotation>()._rotationSpeedMultiplier = 1 + _rotationLevel/(float)_maxLevel;
        Ref.PCon.TWep._weaponRotationSpeedMultiplier = 1 +  weaponRotationIncreaseModifier * (_rotationLevel / (float)_maxLevel);
        Ref.PCon.TWep.UpdateWeaponRotationSpeed();
        Ref.UI._upgradeScreen.UpdateMainScrapCounter();
    }
    public void RevertChanges()
    {
        int diff = _rotationLevel - _tempLevel;
        if (diff < 0)
        {
            for (int i = -1 * diff; i > 0; i--)
            {
                Downgrade();
            }
        }
        else
        {
            for (int i = 0; i < diff; i++)
            {
                Upgrade();
            }
        }
        Ref.UI._upgradeScreen.UpdateMainScrapCounter();
    }
}
