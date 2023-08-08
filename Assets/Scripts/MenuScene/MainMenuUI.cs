using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Launch UI")]
    public GameObject _launchUIObjects;
    [SerializeField]
    private Button mainMenuStartButton;
    [SerializeField]
    private Button nonMainMenuSettingsButton;
    [SerializeField]
    private Button quitGameButton;

    public GameObject _wizardTextObj;
    public GameObject _wheelsTextObj;

    [Space(10)]
    [Header("Save Slot UI")]
    public GameObject _saveSlotObjects;
    public List<MainMenuSaveSlot> _saveSlots;

    [Space(10)]
    [Header("Overworld UI")]

    public GameObject _overworldUIObjects;
    [SerializeField]
    private Button returnToMainMenuButton;

    [Space(10)]
    [Header("Select Vehicle UI")]
    public GameObject _selectVehicleUIObjects;

    [SerializeField]
    private Button launchGameButton;
    [SerializeField]
    private Button previousTankButton;
    [SerializeField]
    private Button nextTankButton;
    public Text _selectedTankText;

    [Space(10)]
    [Header("Other UI Scripts")]
    public SettingsScript _settingsScript;

    private void Awake()
    {
        REF.mUI = this;
        InitButtons();
        InitSaveSlots();
    }

    private void Start()
    {
        ShowLaunchMenuUI(true);
        ShowSaveSlotUI(false);
        ShowOverworldUI(false);
        _settingsScript.CloseSettings();
    }

    private void Update()
    {
        if (!_launchUIObjects.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ReturnToMainMenu();
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                REF.mMenu.wiz.movementLocked = false;
            }
        }
    }
    private void InitButtons()
    {
        //  Main Menu

        mainMenuStartButton.onClick.AddListener(() => ShowSaveSlotUI(true));
        quitGameButton.onClick.AddListener(() => REF.mMenu.QuitGame());

        //  Non Main Menu

        returnToMainMenuButton.onClick.AddListener(() => ReturnToMainMenu());
        nonMainMenuSettingsButton.onClick.AddListener(() => _settingsScript.ToggleSettings());

        //  Select Screen

        nextTankButton.onClick.AddListener(() => REF.mMenu._mmTankPreview.NextTank());
        previousTankButton.onClick.AddListener(() => REF.mMenu._mmTankPreview.PreviousTank());
        launchGameButton.onClick.AddListener(() => REF.mMenu.LaunchGame());
    }
    private void InitSaveSlots()
    {
        int saveSlotIndex = 0;
        foreach (MainMenuSaveSlot slot in _saveSlots)
        {
            PlayerData tmpData = SavePlayerData.LoadPlayer(saveSlotIndex);
            slot._playerData = tmpData;
            slot.UpdateValues(saveSlotIndex, tmpData);
            if (tmpData != null)
            {
                slot._selectButton.onClick.AddListener(() => SelectSlot(saveSlotIndex));
            }
            else
            {
                slot._selectButton.onClick.AddListener(() => GenerateNewSaveFileForSlot(slot));
            }
            slot._deleteButton.onClick.AddListener(() => DeleteSaveFileForSlot(slot));
            saveSlotIndex++;
        }
    }

    private void DeleteSaveFileForSlot(MainMenuSaveSlot slot)
    {
        slot._playerData = null;
        slot.UpdateValues(slot._slotIndex, slot._playerData);
        SavePlayerData.DeleteSaveFile(slot._slotIndex);
        slot._selectButton.onClick.RemoveAllListeners();
        slot._selectButton.onClick.AddListener(() => GenerateNewSaveFileForSlot(slot));
    }

    private void GenerateNewSaveFileForSlot(MainMenuSaveSlot slot)
    {
        slot._playerData = SavePlayerData.GenerateFreshSaveFile(slot._slotIndex);
        slot._selectButton.onClick.RemoveAllListeners();
        slot.UpdateValues(slot._slotIndex, slot._playerData);
        slot._selectButton.onClick.AddListener(() => SelectSlot(slot._slotIndex));
    }

    private void SelectSlot(int index)
    {
        DataStorage.Singleton.saveSlot = index;
        ShowOverworldUI();
    }

    //  Main Menu
    private void ShowLaunchMenuUI(bool activate)
    {
        _launchUIObjects.SetActive(activate);
        _overworldUIObjects.SetActive(activate);
        if (activate)
        {
            StartCoroutine(WizardLogoAnimation());
        }
        REF.mMenu.wiz.movementLocked = activate;
    }

    private void ShowSaveSlotUI(bool activate)
    {
        _saveSlotObjects.SetActive(activate);
        _settingsScript.CloseSettings();

        if(activate)
        {
            foreach (MainMenuSaveSlot slot in _saveSlots)
            {
                slot.UpdateValues(slot._slotIndex, slot._playerData);
            }
        }
    }
    private IEnumerator WizardLogoAnimation()
    {
        float xPosEnd = 120;
        for (int i = 0; i < xPosEnd; i += 2)
        {
            _wizardTextObj.transform.localPosition = new Vector3(-i, 0, 0);
            _wheelsTextObj.transform.localPosition = new Vector3(i, 0, 0);
            yield return new WaitForFixedUpdate();
        }
    }
    public void UpdateSelectedTankText(string tankName)
    {
        _selectedTankText.text = tankName;
    }

    public void ReturnToMainMenu()
    {
        REF.mCam.SetZoom(REF.mCam.furthestZoom);
        ShowLaunchMenuUI(true);
        ShowSaveSlotUI(false);
        ShowOverworldUI(false);
        _settingsScript.CloseSettings();
    }
    public void ShowOverworldUI()
    {
        REF.mCam.SetZoom(REF.mCam.closestZoom);
        ShowLaunchMenuUI(false);
        ShowSaveSlotUI(false);
        ShowOverworldUI(true);
        _settingsScript.CloseSettings();
    }

    private void ShowOverworldUI(bool b)
    {
        _selectVehicleUIObjects.SetActive(b);
        _overworldUIObjects.SetActive(b);
        REF.mMenu.wiz.movementLocked = !b;
    }
}
