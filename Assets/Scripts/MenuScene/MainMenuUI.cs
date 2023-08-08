using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    //  Main Menu

    public GameObject _mainMenuGO;
    [SerializeField]
    private Button mainMenuStartButton;
    [SerializeField]
    private Button nonMainMenuSettingsButton;
    [SerializeField]
    private Button quitGameButton;

    public GameObject _wizardTextObj;
    public GameObject _wheelsTextObj;

    //  Non Main Menu UI

    public GameObject _nonMainMenuGO;

    [SerializeField]
    private Button returnToMainMenuButton;

    //  Select Screen UI

    public GameObject _selectScreenGO;

    [SerializeField]
    private Button launchGameButton;
    [SerializeField]
    private Button previousTankButton;
    [SerializeField]
    private Button nextTankButton;
    public Text _selectedTankText;

    //  SettingsScreen

    public SettingsScript _settingsScript;

    private void Awake()
    {
        REF.mUI = this;
    }
    private void Start()
    {
        ActivateMainMenuUI(true);
        ActivateOverworldUI(false);
        InitButtons();
        _settingsScript.CloseSettings();
    }
    
    private void Update()
    {
        if (!_mainMenuGO.activeInHierarchy)
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

        mainMenuStartButton.onClick.AddListener(() => ShowOverworldUI());
        quitGameButton.onClick.AddListener(() => REF.mMenu.QuitGame());

        //  Non Main Menu

        returnToMainMenuButton.onClick.AddListener(() => ReturnToMainMenu());
        nonMainMenuSettingsButton.onClick.AddListener(() => _settingsScript.ToggleSettings());

        //  Select Screen

        nextTankButton.onClick.AddListener(() => REF.mMenu._mmTankPreview.NextTank());
        previousTankButton.onClick.AddListener(() => REF.mMenu._mmTankPreview.PreviousTank());
        launchGameButton.onClick.AddListener(() => REF.mMenu.LaunchGame());
    }

    //  Main Menu
    private void ActivateMainMenuUI(bool b)
    {
        _mainMenuGO.SetActive(b);
        _nonMainMenuGO.SetActive(b);
        StartCoroutine(WizardLogoAnimation());
        REF.mMenu.wiz.movementLocked = b;
    }

    private IEnumerator WizardLogoAnimation()
    {
        for (int i = 0; i < 120; i++)
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
        ActivateMainMenuUI(true);
        ActivateOverworldUI(false);
        _settingsScript.CloseSettings();
    }
    public void ShowOverworldUI()
    {
        REF.mCam.SetZoom(REF.mCam.closestZoom);
        ActivateMainMenuUI(false);
        ActivateOverworldUI(true);
        _settingsScript.CloseSettings();
    }

    private void ActivateOverworldUI(bool b)
    {
        _selectScreenGO.SetActive(b);
        _nonMainMenuGO.SetActive(b);
        REF.mMenu.wiz.movementLocked = !b;
    }
}
