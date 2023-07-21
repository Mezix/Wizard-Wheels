using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSystem : ASystem
{
    private UIUpgradeField _upgradeField;
    private List<int> _upgradeLevels = new List<int>();

    public override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        if (!_upgradeField) CreateUpgradeField();
    }
    private void InitUpgradeField()
    {
        _upgradeLevels.Add(0);
        _upgradeLevels.Add(20);
        _upgradeLevels.Add(40);
        _upgradeLevels.Add(60);
        _upgradeLevels.Add(80);
        _upgradeLevels.Add(150);

        _upgradeField._tempLevel = _upgradeField._currentLevel = 0;
        _upgradeField._maxLevel = 5;
    }
    private void CreateUpgradeField()
    {
        _upgradeField = REF.UI._upgradeScreen.CreateUpgradeField();
        InitUpgradeField();

        _upgradeField.InitUpgradeField("Engine", _upgradeField._currentLevel, _upgradeField._maxLevel, _upgradeLevels);
        _upgradeField._upgradeButton.onClick.AddListener(() => Upgrade());
        _upgradeField._downgradeButton.onClick.AddListener(() => Downgrade());

        REF.UI._engineUIScript.UpdateEngineLevel(_upgradeField._currentLevel, _upgradeField._maxLevel);

        Events.instance.UpgradesSaved += SaveChanges;
        Events.instance.UpgradesReverted += RevertChanges;
    }
    public void Upgrade()
    {
        if (_upgradeField._tempLevel >= _upgradeField._maxLevel) return;
        if (REF.UI._upgradeScreen._remainingScrap <= _upgradeLevels[_upgradeField._tempLevel +1]) return;
        _upgradeField._tempLevel++;
        REF.UI._upgradeScreen.RemoveTempScrap(_upgradeLevels[_upgradeField._tempLevel]);
        _upgradeField.SetTempLevel(_upgradeField._tempLevel);
    }
    public void Downgrade()
    {
        if (_upgradeField._tempLevel <= _upgradeField._currentLevel) return;
        _upgradeField._tempLevel--;
        REF.UI._upgradeScreen.AddTempScrap(_upgradeLevels[_upgradeField._tempLevel +1]);
        _upgradeField.SetTempLevel(_upgradeField._tempLevel);
    }
    public void SaveChanges()
    {
        _upgradeField._currentLevel = _upgradeField._tempLevel;
        if (REF.UI) REF.UI._engineUIScript.UpdateEngineLevel(_upgradeField._currentLevel, _upgradeField._maxLevel);
        REF.UI._upgradeScreen.UpdateMainScrapCounter();
    }
    public void RevertChanges()
    {
        int diff = _upgradeField._currentLevel - _upgradeField._tempLevel;
        if (diff < 0)
        {
            for(int i = -1 * diff; i > 0; i--)
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
        REF.UI._upgradeScreen.UpdateMainScrapCounter();
    }

    //  System Stuff

    public override void InitSystemStats()
    {

    }
    public override void StartInteraction()
    {
        IsBeingInteractedWith = true;
    }
    public override void StopInteraction()
    {
        IsBeingInteractedWith = false;
    }
}
