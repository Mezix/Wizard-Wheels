using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSystem : ASystem
{
    private UIUpgradeField _upgradeField;
    private List<int> _upgradeLevels = new List<int>();
    public List<GameObject> EngineBlocks;

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

        SetLevel(_upgradeField._currentLevel);
    }
    private void CreateUpgradeField()
    {
        if (!REF.CombatUI) return;
        _upgradeField = REF.CombatUI._upgradeScreen.CreateUpgradeField();
        InitUpgradeField();

        _upgradeField.InitUpgradeField("Engine", _upgradeField._currentLevel, _upgradeField._maxLevel, _upgradeLevels);
        _upgradeField._upgradeButton.onClick.AddListener(() => Upgrade());
        _upgradeField._downgradeButton.onClick.AddListener(() => Downgrade());

        REF.CombatUI._engineUIScript.UpdateEngineLevel(_upgradeField._currentLevel, _upgradeField._maxLevel);

        Events.instance.UpgradesSaved += SaveChanges;
        Events.instance.UpgradesReverted += RevertChanges;
    }
    public void Upgrade()
    {
        if (_upgradeField._tempLevel >= _upgradeField._maxLevel) return;
        if (REF.CombatUI._upgradeScreen._remainingScrap <= _upgradeLevels[_upgradeField._tempLevel +1]) return;
        _upgradeField._tempLevel++;
        REF.CombatUI._upgradeScreen.RemoveTempScrap(_upgradeLevels[_upgradeField._tempLevel]);
        _upgradeField.SetTempLevel(_upgradeField._tempLevel);
    }

    private void SetLevel(int level)
    {
        foreach (GameObject block in EngineBlocks)
        {
            block.SetActive(false);
        }
        switch (level)
        {
            case 0:
                EngineBlocks[0].transform.gameObject.SetActive(true);
                break;
            case 1:
                EngineBlocks[0].transform.gameObject.SetActive(true);
                EngineBlocks[1].transform.gameObject.SetActive(true);
                break;
            case 2:
                EngineBlocks[0].transform.gameObject.SetActive(true);
                EngineBlocks[1].transform.gameObject.SetActive(true);
                EngineBlocks[2].transform.gameObject.SetActive(true);
                break;
            case 3:
                EngineBlocks[0].transform.gameObject.SetActive(true);
                EngineBlocks[1].transform.gameObject.SetActive(true);
                EngineBlocks[2].transform.gameObject.SetActive(true);
                EngineBlocks[3].transform.gameObject.SetActive(true);
                break;
            case 4:
                EngineBlocks[0].transform.gameObject.SetActive(true);
                EngineBlocks[1].transform.gameObject.SetActive(true);
                EngineBlocks[2].transform.gameObject.SetActive(true);
                EngineBlocks[3].transform.gameObject.SetActive(true);
                EngineBlocks[4].transform.gameObject.SetActive(true);
                break;
            default: //6 blocks or more
                EngineBlocks[0].transform.gameObject.SetActive(true);
                EngineBlocks[1].transform.gameObject.SetActive(true);
                EngineBlocks[2].transform.gameObject.SetActive(true);
                EngineBlocks[3].transform.gameObject.SetActive(true);
                EngineBlocks[4].transform.gameObject.SetActive(true);
                EngineBlocks[5].transform.gameObject.SetActive(true);
                break;
        }
    }

    public void Downgrade()
    {
        if (_upgradeField._tempLevel <= _upgradeField._currentLevel) return;
        _upgradeField._tempLevel--;
        REF.CombatUI._upgradeScreen.AddTempScrap(_upgradeLevels[_upgradeField._tempLevel +1]);
        _upgradeField.SetTempLevel(_upgradeField._tempLevel);
    }
    public void SaveChanges()
    {
        _upgradeField._currentLevel = _upgradeField._tempLevel;
        if (REF.CombatUI) REF.CombatUI._engineUIScript.UpdateEngineLevel(_upgradeField._currentLevel, _upgradeField._maxLevel);
        REF.CombatUI._upgradeScreen.UpdateMainScrapCounter();
        SetLevel(_upgradeField._currentLevel);
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
        REF.CombatUI._upgradeScreen.UpdateMainScrapCounter();
    }
}
