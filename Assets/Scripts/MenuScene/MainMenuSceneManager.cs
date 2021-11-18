using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField]
    private PixelPerfectCamera pixelCam;

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
    [SerializeField]
    private Text SelectedTankText;

    private void Awake()
    {
        InitButtons();
    }
    private void Start()
    {
        ActivateMainMenuUI(true);
        ActivateSelectScreen(false);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!MainMenuGO.activeInHierarchy)
            {
                ReturnToMainMenu();
            }
        }
    }
    private void InitButtons()
    {

    }

    public void NextTank()
    {

    }
    public void PreviousTank()
    {

    }
    public void ShowSelectionScreen()
    {
        pixelCam.assetsPPU = 100;
        ActivateMainMenuUI(false);
        ActivateSelectScreen(true);
    }

    private void ActivateMainMenuUI(bool b)
    {
        MainMenuGO.SetActive(b);
    }
    private void ActivateSelectScreen(bool b)
    {
        SelectScreenGO.SetActive(b);
    }

    public void ReturnToMainMenu()
    {
        pixelCam.assetsPPU = 32;
        ActivateMainMenuUI(true);
        ActivateSelectScreen(false);
    }
    public void LaunchGame()
    {
        StartCoroutine(ShowLoadingScreen());
    }

    public IEnumerator ShowLoadingScreen()
    {
        Instantiate((GameObject) Resources.Load("LoadingScreen"));
        yield return new WaitForSeconds(0.5f);
        Loader.Load(Loader.Scene.GameScene);
    }

    public void LaunchSettings()
    {
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
