using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateTankSceneManager : MonoBehaviour
{
    public static CreateTankSceneManager instance;
    public CreateTankTools _tools;
    public CreateTankGeometry _tGeo;
    public CreateTankUI _tUI;
    public CreateTankMouseScript mouse;
    public TankRoomConstellation tankToEdit;
    private bool newTank;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (tankToEdit)
        {
            newTank = false;
            _tUI._inputField.text = tankToEdit.name;
        }
        else
        {
            newTank = true;
            tankToEdit = new TankRoomConstellation();
            _tUI._inputField.textComponent.text = "Untitled";
        }
        //_tGeo._vehicleData = DataStorage.Singleton.CopyVehicleDataFromTankRoomConstellationToVehicleData(tankToEdit);
        LoadTank();
    }
    public void SaveTank()
    {
        string newName = _tUI._inputField.textComponent.text;
        _tUI._inputField.text = newName;
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
        _tGeo.SpawnTankForCreator();
        _tUI._inputField.placeholder.GetComponent<Text>().text = tankToEdit.name;
    }
}
