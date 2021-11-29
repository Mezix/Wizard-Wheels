using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{
    //  Player
    public OverworldWizard wiz;
    public TankRoomConstellation[] PlayerTanks;
    public int tankIndex;
    public GameObject orb;


    private void Awake()
    {
        Ref.mMenu = this;
    }
    private void Start()
    {
        wiz.movementLocked = true;

        tankIndex = 0;
        Ref.mUI.UpdateSelectedTankText(PlayerTanks[tankIndex].name);
    }
    private void Update()
    {
    }
    public void NextTank()
    {
        tankIndex++;
        if(tankIndex >= PlayerTanks.Length)
        {
            tankIndex = 0;
        }
        Ref.mUI.UpdateSelectedTankText(PlayerTanks[tankIndex].name);
    }
    public void PreviousTank()
    {
        tankIndex--;
        if (tankIndex < 0)
        {
            tankIndex = PlayerTanks.Length - 1;
        }
        Ref.mUI.UpdateSelectedTankText(PlayerTanks[tankIndex].name);
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
    public void QuitGame()
    {
        Application.Quit();
    }
}
