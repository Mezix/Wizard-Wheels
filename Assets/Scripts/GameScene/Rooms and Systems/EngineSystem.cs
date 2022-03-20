using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSystem : ISystem
{
    public int _engineLevel;
    public int _tempLevel;
    public int _maxEngineLevel;
    public UIUpgradeField _upgradeField;

    private List<int> _upgradeLevels = new List<int>();

    private void Awake()
    {
        SystemObj = gameObject;
        _maxEngineLevel = 5;
        _tempLevel = _engineLevel = 3;
        
        InitUpgradeLevels();
    }
    private void Start()
    {
        if (Ref.UI)
        {
            Ref.UI.UpdateEngineLevel(_engineLevel, _maxEngineLevel);
            if (!_upgradeField) CreateUpgradeField();
        }
    }
    private void InitUpgradeLevels()
    {
        _upgradeLevels.Add(0);
        _upgradeLevels.Add(20);
        _upgradeLevels.Add(40);
        _upgradeLevels.Add(60);
        _upgradeLevels.Add(80);
        _upgradeLevels.Add(150);
    }
    public override void InitSystemStats()
    {

    }
    public override void StartInteraction()
    {
        IsBeingInteractedWith = true;
        //print("Engine go brrr hahahaha :D");
    }
    public override void StopInteraction()
    {
        IsBeingInteractedWith = false;
        //print("Engine stop go brrr");
    }
    private void CreateUpgradeField()
    {
        UIUpgradeField ui = Ref.UI._upgradeScreen.CreateUpgradeField();
        _upgradeField = ui;

        ui.InitUpgradeField("Engine", _engineLevel, _maxEngineLevel, _upgradeLevels);
        ui._upgradeButton.onClick.AddListener(() => Upgrade());
        ui._downgradeButton.onClick.AddListener(() => Downgrade());

        Events.instance.UpgradesSaved += SaveChanges;
        Events.instance.UpgradesReverted += RevertChanges;
    }
    public void Upgrade()
    {
        if (_tempLevel >= _maxEngineLevel) return;
        if (Ref.UI._upgradeScreen._remainingScrap <= _upgradeLevels[_tempLevel+1]) return;
        _tempLevel++;
        Ref.UI._upgradeScreen.RemoveTempScrap(_upgradeLevels[_tempLevel]);
        _upgradeField.SetTempLevel(_tempLevel);
    }
    public void Downgrade()
    {
        if (_tempLevel <= 0) return;
        _tempLevel--;
        Ref.UI._upgradeScreen.AddTempScrap(_upgradeLevels[_tempLevel+1]);
        _upgradeField.SetTempLevel(_tempLevel);
    }

    public void SaveChanges()
    {
        _engineLevel = _tempLevel;
        if (Ref.UI) Ref.UI.UpdateEngineLevel(_engineLevel, _maxEngineLevel);
        Ref.UI._upgradeScreen.UpdateScrapCounter();
    }
    public void RevertChanges()
    {
        int diff = _engineLevel - _tempLevel;
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
        Ref.UI._upgradeScreen.UpdateScrapCounter();
    }
}
