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
    public int maxLevel;
    [HideInInspector]
    public int currentLevel;
    [HideInInspector]
    public int tempCurrentLevel;
    [HideInInspector]
    public List<int> _upgradeLevels = new List<int>();

    private void Start()
    {
        Events.instance.UpgradeScreenUpdated += UpdateUpgradeField;
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
            GameObject go = Instantiate((GameObject) Resources.Load("UpgradeFieldBarSplitter"));
            go.transform.parent = _layout.transform;
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
        tempCurrentLevel = lvl;
        Events.instance.UpdateUpgradeScreen();
    }
    public void UpdateUpgradeField()
    {
        if (tempCurrentLevel != maxLevel) _nextScrapUpgrade.text = _upgradeLevels[tempCurrentLevel + 1].ToString();
        else _nextScrapUpgrade.text = "--";

        if (tempCurrentLevel == maxLevel || Ref.UI._upgradeScreen._remainingScrap <= _upgradeLevels[currentLevel+1]) _upgradeButton.interactable = false;
        else _upgradeButton.interactable = true;

        if (tempCurrentLevel == 0) _downgradeButton.interactable = false;
        else _downgradeButton.interactable = true;

        _fillLevel.fillAmount = tempCurrentLevel / (float)maxLevel;
    }
}
