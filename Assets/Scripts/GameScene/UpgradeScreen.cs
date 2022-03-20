using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour
{
    [HideInInspector]
    public List<UIUpgradeField> _upgradeFields = new List<UIUpgradeField>();
    public Button _saveButton;
    public Button _revertButton;
    public Button _closeWindow;
    public Button _toggleUpgrades;
    public bool _closed;
    public Transform _layoutGroup;

    public GameObject _popUpWindow;
    private bool _popUpOpened;
    public Button _popUpSaveButton;
    public Button _popUpRevertButton;

    public Text _mainUIScrapCounter;

    public Text _remainingScrapText;
    [HideInInspector]
    public int _remainingScrap;
    [HideInInspector]
    public int _totalScrap;

    private void Awake()
    {
        _saveButton.onClick.AddListener(() => SaveUpgrades());
        _revertButton.onClick.AddListener(() => RevertUpgrades());
        _popUpSaveButton.onClick.AddListener(() => SaveUpgrades());
        _popUpRevertButton.onClick.AddListener(() => RevertUpgrades());
        _closeWindow.onClick.AddListener(() => CloseUpgradeScreen());
        _toggleUpgrades.onClick.AddListener(() => ToggleUpgradeScreen());
    }

    private void Start()
    {
        AddNewScrap(350);
        _popUpOpened = false;
        InitPoints();
        UpdateUpgradeScreen();

        ShowSaveButton(false);
        ShowRevertButton(false);
        ShowPopUpWindow(false);

        Events.instance.UpgradeScreenUpdated += UpdateSaveAndRevertStatus;
        CloseUpgradeScreen();
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
    public void AddNewScrap(int points)
    {
        _remainingScrap += points;
        UpdateScrapCounter();
    }
    public void AddTempScrap(int points)
    {
        _remainingScrap += points;
        UpdateUpgradeScreen();
    }
    public void RemoveTempScrap(int points)
    {
        if(_remainingScrap > 0) _remainingScrap -= points;
        UpdateUpgradeScreen();
    }
    public void UpdateScrapCounter()
    {
        string pointsString = "";
        string remaining = _remainingScrap.ToString();
        for (int i = 3 - remaining.Length; i > 0; i--)
        {
            pointsString += "0";
        }
        pointsString += remaining;
        _mainUIScrapCounter.text = pointsString;
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

    //  Save and Revert

    private void UpdateSaveAndRevertStatus()
    {
        bool savingPossible = false;
        foreach (UIUpgradeField ui in _upgradeFields)
        {
            if (ui.tempCurrentLevel != ui.currentLevel) savingPossible = true;
        }
        ShowSaveButton(savingPossible);
        ShowRevertButton(savingPossible);
    }
    private bool CheckSaveAndRevertStatus()
    {
        bool savingPossible = false;
        foreach (UIUpgradeField ui in _upgradeFields)
        {
            if (ui.tempCurrentLevel != ui.currentLevel) savingPossible = true;
        }
        return savingPossible;
    }
    public void SaveUpgrades()
    {
        Events.instance.SaveUpgrades();
        ShowSaveButton(false);
        ShowRevertButton(false);

        if (_popUpOpened) CloseUpgradeScreen();
        ShowPopUpWindow(false);
    }
    public void ShowSaveButton(bool b)
    {
        _saveButton.interactable = b;
    }
    public void RevertUpgrades()
    {
        Events.instance.RevertUpgrades();
        ShowSaveButton(false);
        ShowRevertButton(false);

        if(_popUpOpened) CloseUpgradeScreen();
        ShowPopUpWindow(false);
    }
    public void ShowRevertButton(bool b)
    {
        _revertButton.interactable = b;
    }

    //   Open and Close Upgrade screen

    public void ToggleUpgradeScreen()
    {
        if (_closed) OpenUpgrades();
        else CloseUpgradeScreen();
    }
    public void OpenUpgrades()
    {
        gameObject.SetActive(true);

        //  Set everything back to temp without saving!
        _closed = false;
    }
    public void CloseUpgradeScreen()
    {
        if(CheckSaveAndRevertStatus() && !_popUpOpened)
        {
            ShowPopUpWindow(true);
        }
        else
        {
            ShowPopUpWindow(false);
            gameObject.SetActive(false);
            _closed = true;
        }
    }

    //  Open and Close Pop Up Window

    public void ShowPopUpWindow(bool b)
    {
        _popUpWindow.SetActive(b);
        _popUpOpened = b;
    }
}
