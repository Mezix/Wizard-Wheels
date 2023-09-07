using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class MenuSceneTankStats : MonoBehaviour
{
    public GameObject _allObjects;
    public GameObject _newRunStatsAllObjects;
    public GameObject _currentRunStatsAllObjects;
    [SerializeField]
    private Button launchGameButton;
    [SerializeField]
    private Button previousTankButton;
    [SerializeField]
    private Button nextTankButton;

    [Header("New Run")]

    public Text _vehicleName;
    public Text _vehicleMaxHP;
    public Text _vehicleMaxSpeed;
    public Text _vehicleRotationSpeed;
    public Text _vehicleAccel;
    public Text _vehicleDecel;
    public Text _vehicleFlavorText;

    [Header("Continue Run")]
    [SerializeField]
    private Button continueRunButton;
    [SerializeField]
    private Button resetRunButton;
    public Text _runVehicleName;
    public Text _runScrap;
    public Text _runWizards;
    public Text _runLength;
    public Text _runCurrentEvent;

    private void Awake()
    {
        nextTankButton.onClick.AddListener(() => REF.mMenu._mmTankPreview.NextTank());
        previousTankButton.onClick.AddListener(() => REF.mMenu._mmTankPreview.PreviousTank());
        launchGameButton.onClick.AddListener(() => REF.mMenu.StartNewRun());
        continueRunButton.onClick.AddListener(() => REF.mMenu.ContinueRun());
        resetRunButton.onClick.AddListener(() => REF.mMenu.ResetRun());
    }

    public void Show(bool active)
    {
        _allObjects.SetActive(active);

        bool newRun = !DataStorage.Singleton.playerData.RunStarted;
        _newRunStatsAllObjects.gameObject.SetActive(newRun);
        _currentRunStatsAllObjects.gameObject.SetActive(!newRun);
    }
    public void UpdateNewRunVehicleText(VehicleInfo vehicleInfo)
    {
        _vehicleName.text = vehicleInfo.TankName;
        _vehicleMaxHP.text = vehicleInfo.TankHealth.ToString() + "HP";
        _vehicleMaxSpeed.text = vehicleInfo.TankMaxSpeed.ToString() + " m/s";
        _vehicleRotationSpeed.text = vehicleInfo.RotationSpeed.ToString() + "°";
        _vehicleAccel.text = vehicleInfo.TankAccel.ToString() + " m/s";
        _vehicleDecel.text = vehicleInfo.TankDecel.ToString() + " m/s";
        _vehicleFlavorText.text = vehicleInfo.FlavorText;
    }
    public void UpdateCurrentRunVehicleStats(PlayerData data)
    {
        _runVehicleName.text = data.Info.TankName;
        _runLength.text = "Time Played: " + HM.SecondsToTimeDisplay(data.TimeInSecondsPlayed);
        _runWizards.text = "Crew Size: " + data.WizardList.Count.ToString();
        _runScrap.text = "Scrap: " + data.GetScrap();
        _runCurrentEvent.text = "Current Event: " + data.CurrentEventPath[data.CurrentEventPathIndex]._event.ToString();
    }
}
