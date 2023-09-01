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
    public TankRoomConstellation tankToEdit;

    // PlayerMode
    public VehicleData _vehicleData;

    public enum CreatorMode
    {
        DevMode,
        PlayerMode
    }
    public CreatorMode _launchMode;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        LaunchInMode(_launchMode);
    }
    public void LaunchInMode(CreatorMode launchMode)
    {
        if (launchMode.Equals(CreatorMode.DevMode))
        {
            if (tankToEdit)
            {
                newTank = false;
                CreateTankUI.instance._inputField.text = tankToEdit.name;
                LoadTank();
            }
            else
            {
                newTank = true;
                tankToEdit = new TankRoomConstellation();
                CreateTankUI.instance._inputField.textComponent.text = "Untitled";
            }
        }
        else
        {
            _vehicleData = DataStorage.Singleton.playerData.vehicleData;
        }
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
        tankToEdit.SaveVehicle(newName);
    }
    public void LoadTank()
    {
        tankToEdit.InitTankForCreation();
        CreateTankGeometry.instance.SpawnTankForCreator();
        CreateTankUI.instance._inputField.placeholder.GetComponent<Text>().text = tankToEdit.name;
    }
}
