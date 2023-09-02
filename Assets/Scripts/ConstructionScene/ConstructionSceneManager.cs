using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConstructionSceneManager : MonoBehaviour
{
    public static ConstructionSceneManager instance;

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
        //hideFlags = HideFlags.DontSaveInEditor;
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
                ConstructionSceneUI.instance._inputField.text = tankToEdit.name;
            }
            else
            {
                newTank = true;
                tankToEdit = new TankRoomConstellation();
                ConstructionSceneUI.instance._inputField.textComponent.text = "Untitled";
            }
        }
        LoadVehicle();
        ConstructionSceneUI.instance.LaunchInMode(launchMode);
    }

    public void SaveTank()
    {
        string newName = ConstructionSceneUI.instance._inputField.textComponent.text;
        ConstructionSceneUI.instance._inputField.text = newName;
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
            DataStorage.CopyVehicleDataFromTo(DataStorage.Singleton.playerData.vehicleData, ref _tmpVehicleData);
        }
        ConstructionSceneGeometry.instance.LoadVehicle();
        ConstructionSceneUI.instance._inputField.placeholder.GetComponent<Text>().text = tankToEdit.name;
    }
}
