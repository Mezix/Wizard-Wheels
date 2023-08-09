using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeField : MonoBehaviour
{
    public Text _updateFieldName;
    public Text _nextScrapUpgrade;
    public Image _fillLevel;
    public Button _upgradeButton;
    public Button _downgradeButton;

    public HorizontalLayoutGroup _layout;

    [HideInInspector]
    public int _maxLevel;
    [HideInInspector]
    public int _currentLevel;
    [HideInInspector]
    public int _tempLevel;
    [HideInInspector]
    public List<int> _upgradeLevels = new List<int>();

    private void Start()
    {
        Events.instance.UpgradeScreenUpdated += UpdateUpgradeField;
        Events.instance.UpgradesSaved += UpdateUpgradeField;
        Events.instance.UpdateUpgradeScreen();
    }
    public void InitUpgradeField(string fieldName, int currentLvl, int maxLvl, List<int> levels)
    {
        _updateFieldName.text = fieldName;
        _maxLevel = maxLvl;
        _tempLevel = _currentLevel = currentLvl;

        foreach(int i in levels)
        {
            _upgradeLevels.Add(i);
            GameObject go = Instantiate(Resources.Load(GS.UIPrefabs("UpgradeFieldBarSplitter"), typeof (GameObject)) as GameObject);
            go.transform.SetParent(_layout.transform);
            go.transform.localScale = Vector3.one;
        }
        float a = 51 - levels.Count + 2;
        float b = levels.Count - 1;
        float spacing = a / b;
        _layout.spacing = spacing;
        UpdateUpgradeField();
    }
    public void SetTempLevel(int lvl)
    {
        _tempLevel = lvl;
        Events.instance.UpdateUpgradeScreen();
    }
    public void UpdateUpgradeField()
    {
        if (_tempLevel != _maxLevel) _nextScrapUpgrade.text = _upgradeLevels[_tempLevel + 1].ToString();
        else _nextScrapUpgrade.text = "--";

        if (_tempLevel == _maxLevel || REF.CombatUI._upgradeScreen._remainingScrap <= _upgradeLevels[_currentLevel+1]) _upgradeButton.interactable = false;
        else _upgradeButton.interactable = true;

        if (_tempLevel <= _currentLevel || _currentLevel == _maxLevel) _downgradeButton.interactable = false;
        else _downgradeButton.interactable = true;

        _fillLevel.fillAmount = _tempLevel / (float) _maxLevel;
    }
}
