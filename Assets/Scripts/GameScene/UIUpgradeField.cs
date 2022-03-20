using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeField : MonoBehaviour
{
    public Text _updateFieldName;
    public Text _upgradeLevel;
    public Image _fillLevel;
    public Button _upgradeButton;
    public Button _downgradeButton;

    public int maxLevel;
    public int currentLevel;
    public int tempCurrentLevel;

    public List<int> _upgradeLevels = new List<int>();

    private void Awake()
    {
        Events.instance.UpgradeScreenUpdated += UpdateUpgradeField;
    }
    private void Start()
    {
        Events.instance.UpdateUpgradeScreen();
    }
    public void InitUpgradeField(string fieldName, int currentLvl, int maxLvl, List<int> levels)
    {
        _updateFieldName.text = fieldName;
        maxLevel = maxLvl;
        tempCurrentLevel = currentLevel = currentLvl;

        foreach(int i in levels)
        {
            _upgradeLevels.Add(i);
        }
        UpdateUpgradeField();
    }
    public void SetTempLevel(int lvl)
    {
        tempCurrentLevel = lvl;
        Events.instance.UpdateUpgradeScreen();
    }
    public void UpdateUpgradeField()
    {
        _upgradeLevel.text = tempCurrentLevel.ToString();

        if (tempCurrentLevel == maxLevel || Ref.UI._upgradeScreen._remainingScrap <= _upgradeLevels[currentLevel+1]) _upgradeButton.interactable = false;
        else _upgradeButton.interactable = true;

        if (tempCurrentLevel == 0) _downgradeButton.interactable = false;
        else _downgradeButton.interactable = true;

        _fillLevel.fillAmount = tempCurrentLevel / (float)maxLevel;
    }
}
