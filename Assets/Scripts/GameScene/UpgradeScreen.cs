using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour
{
    public List<UIUpgradeField> _upgradeFields = new List<UIUpgradeField>();
    public Button _saveButton;
    public Button _closeWindow;
    public Button _toggleUpgrades;
    public bool _closed;
    public Transform _layoutGroup;

    public Text _remainingScrapText;
    public int _remainingScrap;
    public int _totalScrap;

    private void Awake()
    {
        CloseUpgrades();
        _saveButton.onClick.AddListener(() => SaveUpgrades());
        _closeWindow.onClick.AddListener(() => CloseUpgrades());
        _toggleUpgrades.onClick.AddListener(() => ToggleUpgradeScreen());
    }

    private void Start()
    {
        _remainingScrap = 350;
        InitPoints();
        UpdateUpgradeScreen();

        ShowSaveButton(false);

        Events.instance.UpgradeScreenUpdated += CheckSaveStatus;
    }
    private void InitPoints()
    {
        _totalScrap = 0;
        foreach(UIUpgradeField ui in _upgradeFields)
        {
            int lvl = ui.currentLevel;
            foreach(int i in ui._upgradeLevels)
            {
                _totalScrap += i;
            }
        }
        _totalScrap += _remainingScrap;
    }
    private void UpdateUpgradeScreen()
    {
        string pointsString = "";
        string remaining = _remainingScrap.ToString();
        for (int i = 3 - remaining.Length; i > 0; i--)
        {
            pointsString += "0";
        }
        pointsString += remaining;
        _remainingScrapText.text = pointsString;
    }
    public void AddPoints(int points)
    {
        _remainingScrap += points;
        UpdateUpgradeScreen();
    }
    public void RemovePoints(int points)
    {
        if(_remainingScrap > 0) _remainingScrap -= points;
        UpdateUpgradeScreen();
    }

    public UIUpgradeField CreateUpgradeField()
    {
        GameObject field = Instantiate((GameObject)Resources.Load("UpgradeField"));
        field.transform.parent = _layoutGroup;
        field.transform.localScale = Vector3.one;
        UIUpgradeField upgrade = field.GetComponent<UIUpgradeField>();
        Ref.UI._upgradeScreen._upgradeFields.Add(upgrade);

        return upgrade;
    }
    private void CheckSaveStatus()
    {
        bool savingPossible = false;
        foreach(UIUpgradeField ui in _upgradeFields)
        {
            if (ui.tempCurrentLevel != ui.currentLevel) savingPossible = true;
        }
        ShowSaveButton(savingPossible);
    }
    public void ShowSaveButton(bool b)
    {
        _saveButton.interactable = b;
    }
    public void SaveUpgrades()
    {
        Events.instance.SaveUpgrades();
        ShowSaveButton(false);
    }
    public void ToggleUpgradeScreen()
    {
        if (_closed) OpenUpgrades();
        else CloseUpgrades();
    }
    public void OpenUpgrades()
    {
        gameObject.SetActive(true);

        //  Set everything back to temp without saving!
        _closed = false;
    }
    public void CloseUpgrades()
    {
        gameObject.SetActive(false);
        _closed = true;
    }
}
