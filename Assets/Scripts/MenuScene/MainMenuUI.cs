using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public enum MainMenuStatus
    {
        LaunchUI,
        SaveSlotUI,
        Overworld
    }

    public MainMenuStatus _mainMenuStatus;

    [Header("Launch UI")]
    public GameObject _launchUIObjects;
    public Button _mainMenuStartButton;
    public Button _quitGameButton;

    public GameObject _wizardTextObj;
    public GameObject _wheelsTextObj;

    [Space(10)]
    [Header("Save Slot UI")]
    public GameObject _saveSlotUIObjects;
    public List<MainMenuSaveSlot> _saveSlots;

    [Space(10)]
    [Header("Overworld UI")]

    public GameObject _overworldUIObjects;
    public Button _returnToMainMenuButton;

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
        ShowMenu(MainMenuStatus.LaunchUI);
        _settingsScript.CloseSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMenu(MainMenuStatus.LaunchUI);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            REF.mMenu.wiz.movementLocked = false;
        }
    }
    private void InitButtons()
    {
        //  Main Menu

        _mainMenuStartButton.onClick.AddListener(() => ShowMenu(MainMenuStatus.SaveSlotUI));
        _quitGameButton.onClick.AddListener(() => REF.mMenu.QuitGame());

        //  Non Main Menu

        _returnToMainMenuButton.onClick.AddListener(() => ShowMenu(MainMenuStatus.LaunchUI));

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
                slot._selectButton.onClick.AddListener(() => SelectSlot(slot._slotIndex));
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
        DataStorage.Singleton.playerData = _saveSlots[index]._playerData;
        ShowMenu(MainMenuStatus.Overworld);
    }

    //  Main Menu
    private void ShowMenu(MainMenuStatus status)
    {
        _mainMenuStatus = status;
        if(status == MainMenuStatus.LaunchUI)
        {
            _launchUIObjects.SetActive(true);
            _overworldUIObjects.SetActive(false);
            _saveSlotUIObjects.SetActive(false);
            _selectVehicleUIObjects.SetActive(false);

            REF.mMenu.wiz.movementLocked = true;
            REF.mCam.SetZoom(REF.mCam.furthestZoom);
            StartCoroutine(WizardLogoAnimation());
        }
        else if (status == MainMenuStatus.SaveSlotUI)
        {
            _launchUIObjects.SetActive(false);
            _overworldUIObjects.SetActive(false);
            _saveSlotUIObjects.SetActive(true);
            _selectVehicleUIObjects.SetActive(false);

            REF.mMenu.wiz.movementLocked = true;
            foreach (MainMenuSaveSlot slot in _saveSlots)
            {
                slot.UpdateValues(slot._slotIndex, slot._playerData);
            }
        }
        else if (status == MainMenuStatus.Overworld)
        {
            _launchUIObjects.SetActive(false);
            _overworldUIObjects.SetActive(true);
            _saveSlotUIObjects.SetActive(false);
            _selectVehicleUIObjects.SetActive(true);

            REF.mCam.SetZoom(REF.mCam.closestZoom);
            REF.mMenu.wiz.movementLocked = false;
        }
        _settingsScript.CloseSettings();
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

}
