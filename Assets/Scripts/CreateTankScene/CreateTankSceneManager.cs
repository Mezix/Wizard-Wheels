using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateTankSceneManager : MonoBehaviour
{
    public static CreateTankSceneManager instance;

    //  DevMode

    private bool newTank;
    [SerializeField]
    private TankRoomConstellation tankToEdit;

    // PlayerMode
    public VehicleData _tmpVehicleData;

    public enum CreatorMode
    {
        DevMode,
        PlayerMode
    }
    public CreatorMode _editorlaunchMode;
    public static bool LaunchInGameplayOverride = false;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if(LaunchInGameplayOverride) LaunchInMode(CreatorMode.PlayerMode);
        else LaunchInMode(_editorlaunchMode);
    }
    public void LaunchInMode(CreatorMode launchMode)
    {
        _editorlaunchMode = launchMode;
        if (launchMode.Equals(CreatorMode.DevMode))
        {
            if (tankToEdit)
            {
                newTank = false;
                CreateTankUI.instance._inputField.text = tankToEdit.name;
            }
            else
            {
                newTank = true;
                tankToEdit = new TankRoomConstellation();
                CreateTankUI.instance._inputField.textComponent.text = "Untitled";
            }
        }
        LoadVehicle();
        CreateTankUI.instance.LaunchInMode(launchMode);
    }

    public void SaveTank()
    {
        string newName = CreateTankUI.instance._inputField.textComponent.text;
        CreateTankUI.instance._inputField.text = newName;
        if (newTank)
        {
            if (newName == "") newName = "tmp";
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(tankToEdit, "Assets/Scripts/GameScene/SOs/TankConstellation/" + newName);
#endif
        }
        if (_editorlaunchMode.Equals(CreatorMode.DevMode)) tankToEdit.SaveVehicle(_tmpVehicleData);
        else DataStorage.Singleton.playerData.vehicleData = _tmpVehicleData;
    }
    public void LoadVehicle()
    {
        if (_editorlaunchMode.Equals(CreatorMode.DevMode))
        {
            _tmpVehicleData = DataStorage.Singleton.CopyTankRoomConstellationToVehicleData(tankToEdit);
        }
        else
        {
            DataStorage.CopyVehicleDataFromTo(ref DataStorage.Singleton.playerData.vehicleData.VehicleMatrix, ref _tmpVehicleData.VehicleMatrix);
            _tmpVehicleData._savedXSize = DataStorage.Singleton.playerData.vehicleData._savedXSize;
            _tmpVehicleData._savedYSize = DataStorage.Singleton.playerData.vehicleData._savedYSize;
        }

        CreateTankGeometry.instance.LoadVehicle();
        CreateTankUI.instance._inputField.placeholder.GetComponent<Text>().text = tankToEdit.name;
    }
}
