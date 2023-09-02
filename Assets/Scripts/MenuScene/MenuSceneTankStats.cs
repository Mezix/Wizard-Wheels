using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class MenuSceneTankStats : MonoBehaviour
{
    public GameObject _allObjects;
    [SerializeField]
    private Button launchGameButton;
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
        launchGameButton.onClick.AddListener(() => REF.mMenu.LaunchGame());
    }

    public void Show(bool active)
    {
       _allObjects.SetActive(active);
    }
    public void UpdateSelectedVehicleText(VehicleStats vehicleInfo)
    {
        _vehicleName.text = vehicleInfo._tankName;
        _vehicleMaxHP.text = vehicleInfo._tankHealth.ToString() + "HP";
        _vehicleMaxSpeed.text = vehicleInfo._tankMaxSpeed.ToString() + " m/s";
        _vehicleRotationSpeed.text = vehicleInfo._rotationSpeed.ToString() + "°";
        _vehicleAccel.text = vehicleInfo._tankAccel.ToString() + " m/s";
        _vehicleDecel.text = vehicleInfo._tankDecel.ToString() + " m/s";
        _vehicleFlavorText.text = vehicleInfo._flavorText;
    }
}
