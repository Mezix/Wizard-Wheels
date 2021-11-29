using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    //  Menu

    public GameObject _mainMenuGO;

    [SerializeField]
    private Button mainMenuStartButton;
    [SerializeField]
    private Button mainMenuSettingsButton;
    [SerializeField]
    private Button quitGameButton;

    //  Non Main Menu UI

    public GameObject _nonMainMenuGO;

    [SerializeField]
    private Button returnToMainMenuButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button closeSettingsButton;

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

    [SerializeField]
    private GameObject settingsGO;
    public bool settingsOn;

    private void Awake()
    {
        Ref.mUI = this;
    }
    private void Start()
    {
        settingsOn = false;
        ActivateMainMenuUI(true);
        ActivateOverworldUI(false);
        ShowSettings(false);
        InitButtons();
    }
    
    private void Update()
    {
        if (!_mainMenuGO.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ReturnToMainMenu();
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                Ref.mMenu.wiz.movementLocked = false;
            }
        }
    }
    private void InitButtons()
    {
        mainMenuStartButton.onClick.AddListener(() => ShowOverworldUI());
        mainMenuSettingsButton.onClick.AddListener(() => ToggleSettings());
        quitGameButton.onClick.AddListener(() => Ref.mMenu.QuitGame());

        returnToMainMenuButton.onClick.AddListener(() => ReturnToMainMenu());
        settingsButton.onClick.AddListener(() => ToggleSettings());
        closeSettingsButton.onClick.AddListener(() => ToggleSettings());

        nextTankButton.onClick.AddListener(() => Ref.mMenu.NextTank());
        previousTankButton.onClick.AddListener(() => Ref.mMenu.PreviousTank());
        launchGameButton.onClick.AddListener(() => Ref.mMenu.LaunchGame());
    }
    public void UpdateSelectedTankText(string tankName)
    {
        _selectedTankText.text = tankName;
    }
    
    public void ReturnToMainMenu()
    {
        Ref.mCam.SetZoom(Ref.mCam.furthestZoom);
        ActivateMainMenuUI(true);
        ActivateOverworldUI(false);
        ShowSettings(false);
    }
    public void ShowOverworldUI()
    {
        Ref.mCam.SetZoom(Ref.mCam.closestZoom);
        ActivateMainMenuUI(false);
        ActivateOverworldUI(true);
        ShowSettings(false);
    }
    private void ActivateMainMenuUI(bool b)
    {
        _mainMenuGO.SetActive(b);
        _nonMainMenuGO.SetActive(b);
        Ref.mMenu.wiz.movementLocked = b;
    }
    private void ActivateOverworldUI(bool b)
    {
        _selectScreenGO.SetActive(b);
        _nonMainMenuGO.SetActive(b);
        Ref.mMenu.wiz.movementLocked = !b;
    }
    public void ToggleSettings()
    {
        settingsOn = !settingsOn;
        ShowSettings(settingsOn);
    }
    public void ShowSettings(bool b)
    {
        settingsGO.SetActive(b);
        settingsOn = b;
    }
}
