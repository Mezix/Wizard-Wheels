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

    public GameObject _upgradeScreenObj;
    public int scrapInventoryItemSlotIndex;

    private void Awake()
    {
        REF.UpgrScreen = this;

        _saveButton.onClick.AddListener(() => SaveUpgrades());
        _revertButton.onClick.AddListener(() => RevertUpgrades());
        _popUpSaveButton.onClick.AddListener(() => SaveUpgrades());
        _popUpRevertButton.onClick.AddListener(() => RevertUpgrades());
        _closeWindow.onClick.AddListener(() => CloseUpgradeScreen());
        _toggleUpgrades.onClick.AddListener(() => ToggleUpgradeScreen());
    }

    private void Start()
    {
        _popUpOpened = false;
        InitPoints();
        UpdateUpgradeScrapCounter();

        ShowSaveButton(false);
        ShowRevertButton(false);
        ShowPopUpWindow(false);

        Events.instance.UpgradeScreenUpdated += UpdateSaveAndRevertStatus;
        CloseUpgradeScreen();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            AddNewScrap(25, true);
        }
    }
    private void InitPoints()
    {
        _totalScrap = 0;
        foreach(UIUpgradeField ui in _upgradeFields)
        {
            int lvl = ui._currentLevel;
            foreach(int i in ui._upgradeLevels)
            {
                _totalScrap += i;
            }
        }
        _totalScrap += _remainingScrap;
    }
    public void UpdateUpgradeScrapCounter()
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
    public void AddNewScrap(int points, bool animPlay)
    {
        _remainingScrap += points;
        UpdateMainScrapCounter();
        UpdateUpgradeScrapCounter();
        REF.InvUI.AddAmount(scrapInventoryItemSlotIndex, points);
        if (animPlay) StartCoroutine(AddScrapAnim(points));
    }
    private IEnumerator AddScrapAnim(int points)
    {
        GameObject g = Instantiate((GameObject) Resources.Load("ScrapText"));
        g.transform.parent = _mainUIScrapCounter.transform.parent;

        RectTransform rect = g.GetComponent<RectTransform>();
        rect.anchoredPosition = _mainUIScrapCounter.rectTransform.anchoredPosition + new Vector2(0,0);
        rect.transform.localScale = _mainUIScrapCounter.transform.localScale;

        Text t = g.GetComponent<Text>();
        t.text = "+" + points.ToString();
        //  TODO: Add sound effect to prefab

        for(int i = 0; i < 50; i++)
        {
            rect.anchoredPosition += new Vector2(i/50f, -i/10f);
            t.color = new Color(1,1,1, (50 - i)/50f);
            yield return new WaitForFixedUpdate();
        }
        Destroy(g);
    }

    public void AddTempScrap(int points)
    {
        _remainingScrap += points;
        UpdateUpgradeScrapCounter();
    }
    public void RemoveTempScrap(int points)
    {
        if(_remainingScrap > 0) _remainingScrap -= points;
        UpdateUpgradeScrapCounter();
    }
    public void UpdateMainScrapCounter()
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
        field.transform.SetParent(_layoutGroup);
        field.transform.localScale = Vector3.one;
        UIUpgradeField upgrade = field.GetComponent<UIUpgradeField>();
        REF.UI._upgradeScreen._upgradeFields.Add(upgrade);

        return upgrade;
    }

    //  Save and Revert

    private void UpdateSaveAndRevertStatus()
    {
        bool savingPossible = UnconfirmedChanges();
        
        ShowSaveButton(savingPossible);
        ShowRevertButton(savingPossible);
    }
    private bool UnconfirmedChanges()
    {
        bool savingPossible = false;
        foreach (UIUpgradeField ui in _upgradeFields)
        {
            if (ui._tempLevel != ui._currentLevel)
            {
                savingPossible = true;
                break;
            }
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
        _upgradeScreenObj.SetActive(true);

        //  Set everything back to temp without saving!
        _closed = false;
    }
    public void CloseUpgradeScreen()
    {
        if(UnconfirmedChanges() && !_popUpOpened)
        {
            ShowPopUpWindow(true);
        }
        else
        {
            ShowPopUpWindow(false);
            _upgradeScreenObj.SetActive(false);
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
