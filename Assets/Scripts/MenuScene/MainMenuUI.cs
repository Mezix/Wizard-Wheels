using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    //  Menu

    [SerializeField]
    private GameObject MainMenuGO;

    [SerializeField]
    private Button MenuStartButton;
    [SerializeField]
    private Button MenuSettingsButton;

    //  Select Screen UI

    [SerializeField]
    private GameObject SelectScreenGO;

    [SerializeField]
    private Button LaunchGameButton;
    [SerializeField]
    private Button PreviousTankButton;
    [SerializeField]
    private Button NextTankButton;
    public Text SelectedTankText;

    private void Awake()
    {
        Ref.mUI = this;
    }
    private void Start()
    {
        ActivateMainMenuUI(true);
        ActivateSelectScreen(false);
        InitButtons();
    }
    
    private void Update()
    {
        if (!MainMenuGO.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ReturnToMainMenu();
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                Ref.mMenu.wiz.movementLocked = false;
            }
            CheckPlayerDistanceFromOrb();
        }
    }
    private void InitButtons()
    {
        MenuStartButton.onClick.AddListener(() => ShowSelectionScreen());
        MenuSettingsButton.onClick.AddListener(() => LaunchSettings());

        NextTankButton.onClick.AddListener(() => Ref.mMenu.NextTank());
        PreviousTankButton.onClick.AddListener(() => Ref.mMenu.PreviousTank());
        LaunchGameButton.onClick.AddListener(() => Ref.mMenu.LaunchGame());
    }
    public void UpdateSelectedTankText(string tankName)
    {
        SelectedTankText.text = tankName;
    }
    private void CheckPlayerDistanceFromOrb()
    {
        if (Vector3.Distance(Ref.mMenu.wiz.transform.position, Ref.mMenu.orb.transform.position) <= 1f)
        {
            Ref.mCam.SetZoom(100);
            Ref.mCam.SetCamParent(Ref.mMenu.orb.transform);
            SelectScreenGO.SetActive(true);
        }
        else
        {
            Ref.mCam.SetZoom(32);
            Ref.mCam.SetCamParent(Ref.mMenu.wiz.transform);
            SelectScreenGO.SetActive(false);
        }
    }
    public void ShowSelectionScreen()
    {
        Ref.mCam.SetZoom(100);
        ActivateMainMenuUI(false);
        ActivateSelectScreen(true);
    }

    private void ActivateMainMenuUI(bool b)
    {
        MainMenuGO.SetActive(b);
        Ref.mMenu.wiz.movementLocked = b;
    }
    private void ActivateSelectScreen(bool b)
    {
        SelectScreenGO.SetActive(b);
        Ref.mMenu.wiz.movementLocked = !b;
    }

    public void ReturnToMainMenu()
    {
        Ref.mCam.SetZoom(32);
        ActivateMainMenuUI(true);
        ActivateSelectScreen(false);
    }
    
    public void LaunchSettings()
    {
    }
}
