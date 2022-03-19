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
    private void Awake()
    {
        SystemObj = gameObject;
        _maxEngineLevel = 5;
        _tempLevel = _engineLevel = 3;
        if (Ref.UI)
        {
            Ref.UI.UpdateEngineLevel(_engineLevel, _maxEngineLevel);
            if (!_upgradeField) CreateUpgradeField();
        }

        Events.instance.UpgradesSaved += SaveChanges;
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

        ui.InitUpgradeField("Engine", _engineLevel, _maxEngineLevel);
        ui._upgradeButton.onClick.AddListener(() => Upgrade());
        ui._downgradeButton.onClick.AddListener(() => Downgrade());
    }
    public void Upgrade()
    {
        if (_tempLevel >= _maxEngineLevel) return;
        _tempLevel++;
        _upgradeField.SetLevel(_tempLevel);
    }
    public void Downgrade()
    {
        if (_tempLevel <= 0) return;
        _tempLevel--;
        _upgradeField.SetLevel(_tempLevel);
    }

    public void SaveChanges()
    {
        _engineLevel = _tempLevel;
        if (Ref.UI) Ref.UI.UpdateEngineLevel(_engineLevel, _maxEngineLevel);
    }
}
