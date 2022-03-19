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
    private int maxLevel;
    private int currentLevel;
    private int tempCurrentLevel;

    public void InitUpgradeField(string fieldName, int currentLvl, int maxLvl)
    {
        _updateFieldName.text = fieldName;
        maxLevel = maxLvl;
        tempCurrentLevel = currentLevel = currentLvl;

        UpdateLevel();
    }
    public void SetLevel(int lvl)
    {
        tempCurrentLevel = lvl;
        UpdateLevel();
    }
    public void UpdateLevel()
    {
        _upgradeLevel.text = tempCurrentLevel.ToString();
        _fillLevel.fillAmount = tempCurrentLevel/ (float)maxLevel;
    }
    public void SaveChanges()
    {

    }
}
