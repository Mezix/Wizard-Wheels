using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;
using Modern2D;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConstructionSceneManager : MonoBehaviour
{
    public static ConstructionSceneManager instance;

    //  DevMode

    private bool newTank;
    [SerializeField]
    private VehicleConstellation tankToEdit;

    // PlayerMode
    public VehicleGeometry _tmpVehicleData;

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

        LightingSystem.system._shadowAlpha.value = 0;
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
                tankToEdit = new VehicleConstellation();
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
        else DataStorage.Singleton.playerData.Geometry = _tmpVehicleData;
    }
    public void LoadVehicle()
    {
        if (_editorlaunchMode.Equals(CreatorMode.DevMode))
        {
            _tmpVehicleData = ConvertVehicleConstellationToVehicleData(tankToEdit);
        }
        else
        {
            CopyVehicleDataFromTo(DataStorage.Singleton.playerData.Geometry, ref _tmpVehicleData);
        }
        ConstructionSceneGeometry.instance.LoadVehicle();
        ConstructionSceneUI.instance._inputField.placeholder.GetComponent<Text>().text = tankToEdit.name;
    }
}
