using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class MenuSceneTankStats : MonoBehaviour
{
    public GameObject _allObjects;
    public bool newRun;
    [SerializeField]
    private Button launchGameButton;
    [SerializeField]
    private Button continueRunButton;
    [SerializeField]
    private Button previousTankButton;
    [SerializeField]
    private Button nextTankButton;

    public Text _vehicleName;
    public Text _vehicleMaxHP;
    public Text _vehicleMaxSpeed;
    public Text _vehicleRotationSpeed;
    public Text _vehicleAccel;
    public Text _vehicleDecel;
    public Text _vehicleFlavorText;
    private void Awake()
    {
        nextTankButton.onClick.AddListener(() => REF.mMenu._mmTankPreview.NextTank());
        previousTankButton.onClick.AddListener(() => REF.mMenu._mmTankPreview.PreviousTank());
        launchGameButton.onClick.AddListener(() => REF.mMenu.StartNewRun());
        continueRunButton.onClick.AddListener(() => REF.mMenu.ContinueRun());
    }

    public void Show(bool active)
    {
        if(newRun)
        {
            _allObjects.SetActive(active);
        }
        else _allObjects.SetActive(false);
        continueRunButton.gameObject.SetActive(!newRun);
    }
    public void UpdateSelectedVehicleText(VehicleInfo vehicleInfo)
    {
        _vehicleName.text = vehicleInfo.TankName;
        _vehicleMaxHP.text = vehicleInfo.TankHealth.ToString() + "HP";
        _vehicleMaxSpeed.text = vehicleInfo.TankMaxSpeed.ToString() + " m/s";
        _vehicleRotationSpeed.text = vehicleInfo.RotationSpeed.ToString() + "°";
        _vehicleAccel.text = vehicleInfo.TankAccel.ToString() + " m/s";
        _vehicleDecel.text = vehicleInfo.TankDecel.ToString() + " m/s";
        _vehicleFlavorText.text = vehicleInfo.FlavorText;
    }
}
