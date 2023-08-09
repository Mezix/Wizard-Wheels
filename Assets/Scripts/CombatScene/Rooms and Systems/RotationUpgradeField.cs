using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationUpgradeField : MonoBehaviour
{
    private UIUpgradeField _upgradeField;
    private float weaponRotationIncreaseModifier;

    private List<int> _upgradeLevels = new List<int>();

    private void Start()
    {
    }
    public void CreateUpgradeField()
    {
        _upgradeField = REF.CombatUI._upgradeScreen.CreateUpgradeField();
        InitUpgradeField();

        _upgradeField.InitUpgradeField("Joint Fluidity", _upgradeField._currentLevel, _upgradeField._maxLevel, _upgradeLevels);
        _upgradeField._upgradeButton.onClick.AddListener(() => Upgrade());
        _upgradeField._downgradeButton.onClick.AddListener(() => Downgrade());

        Events.instance.UpgradesSaved += SaveChanges;
        Events.instance.UpgradesReverted += RevertChanges;
    }
    private void InitUpgradeField()
    {
        _upgradeLevels.Add(0);
        _upgradeLevels.Add(30);
        _upgradeLevels.Add(50);
        _upgradeLevels.Add(70);

        _upgradeField._tempLevel = 0;
        _upgradeField._maxLevel = 3;
        weaponRotationIncreaseModifier = 0.75f;
    }
    public void Upgrade()
    {
        if (_upgradeField._currentLevel >= _upgradeField._maxLevel) return;
        if (REF.CombatUI._upgradeScreen._remainingScrap <= _upgradeLevels[_upgradeField._currentLevel + 1]) return;
        _upgradeField._tempLevel++;
        REF.CombatUI._upgradeScreen.RemoveTempScrap(_upgradeLevels[_upgradeField._tempLevel]);
        _upgradeField.SetTempLevel(_upgradeField._tempLevel);
    }
    public void Downgrade()
    {
        if (_upgradeField._tempLevel <= _upgradeField._currentLevel) return;
        _upgradeField._tempLevel--;
        REF.CombatUI._upgradeScreen.AddTempScrap(_upgradeLevels[_upgradeField._tempLevel + 1]);
        _upgradeField.SetTempLevel(_upgradeField._tempLevel);
    }
    public void SaveChanges()
    {
        _upgradeField._currentLevel = _upgradeField._tempLevel;
        REF.PCon.GetComponent<PlayerTankRotation>()._rotationSpeedMultiplier = 1 + _upgradeField._currentLevel / (float)_upgradeField._maxLevel;
        REF.PCon.TWep._weaponRotationSpeedMultiplier = 1 +  weaponRotationIncreaseModifier * (_upgradeField._currentLevel / (float)_upgradeField._maxLevel);
        REF.PCon.TWep.UpdateWeaponRotationSpeed();
        REF.CombatUI._upgradeScreen.UpdateMainScrapCounter();
    }
    public void RevertChanges()
    {
        int diff = _upgradeField._currentLevel - _upgradeField._tempLevel;
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
        REF.CombatUI._upgradeScreen.UpdateMainScrapCounter();
    }
}
