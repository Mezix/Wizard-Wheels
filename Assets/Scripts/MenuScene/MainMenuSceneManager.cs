using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{
    //  Player
    [SerializeField]
    private OverworldWizard wiz;
    public TankRoomConstellation[] PlayerTanks;
    public int tankIndex;
    [SerializeField]
    private GameObject orb;

    //  Camera
    private Camera cam;
    private Transform camParent;
    private PixelPerfectCamera pixelCam;
    private int zoomLevel;

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
        cam = Camera.main;
        pixelCam = cam.GetComponent<PixelPerfectCamera>();
        InitButtons();
    }
    private void Start()
    {
        tankIndex = 0;
        pixelCam.assetsPPU = zoomLevel = 32;
        camParent = orb.transform;
        cam.transform.localPosition = new Vector3(0,0,-10);
        wiz.movementLocked = true;

        ActivateMainMenuUI(true);
        ActivateSelectScreen(false);
        UpdateSelectedTankText();
    }
    private void Update()
    {
        if (!MainMenuGO.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))  ReturnToMainMenu();
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                wiz.movementLocked = false;
            }
            CheckPlayerDistanceFromOrb();
        }
    }
    private void FixedUpdate()
    {
        ZoomSlowly();
        MoveCamToObjectSlowly();
    }
    private void InitButtons()
    {

    }
    private void MoveCamToObjectSlowly()
    {
        cam.transform.parent = camParent;
        Vector3 diff = Vector2.Lerp(cam.transform.localPosition, Vector2.zero, 0.1f);
        diff.z = -10;
        cam.transform.localPosition = diff;
    }
    private void CheckPlayerDistanceFromOrb()
    {
        if(Vector3.Distance(wiz.transform.position, orb.transform.position) <= 1f)
        {
            zoomLevel = 100;
            camParent = orb.transform;
            SelectScreenGO.SetActive(true);

        }
        else
        {
            zoomLevel = 32;
            camParent = wiz.transform;
            SelectScreenGO.SetActive(false);
        }
    }
    private void ZoomSlowly()
    {
        pixelCam.assetsPPU += Math.Sign(zoomLevel - pixelCam.assetsPPU);
    }
    public void NextTank()
    {
        tankIndex++;
        if(tankIndex >= PlayerTanks.Length)
        {
            tankIndex = 0;
        }
        UpdateSelectedTankText();
    }
    public void PreviousTank()
    {
        tankIndex--;
        if (tankIndex < 0)
        {
            tankIndex = PlayerTanks.Length - 1;
        }
        UpdateSelectedTankText();
    }
    public void UpdateSelectedTankText()
    {
        SelectedTankText.text = PlayerTanks[tankIndex].name;
    }
    public void ShowSelectionScreen()
    {
        zoomLevel = 100;
        ActivateMainMenuUI(false);
        ActivateSelectScreen(true);
    }

    private void ActivateMainMenuUI(bool b)
    {
        MainMenuGO.SetActive(b);
        wiz.movementLocked = b;
    }
    private void ActivateSelectScreen(bool b)
    {
        SelectScreenGO.SetActive(b);
        wiz.movementLocked = !b;
    }

    public void ReturnToMainMenu()
    {
        zoomLevel = 32;
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
        LevelManager.playerTankConstellationFromSelectScreen = PlayerTanks[tankIndex];
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
