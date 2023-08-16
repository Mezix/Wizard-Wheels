using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConstructionSceneManager : MonoBehaviour
{
    public static ConstructionSceneManager instance;
    public ConstructionSceneTools _tools;
    public ConstructionSceneGeometry _tGeo;
    public ConstructionSceneUI _tUI;
    public ConstructionSceneMouseScript mouse;
    private bool newTank;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        _tGeo._vehicleData = DataStorage.Singleton.playerData.vehicleData;
        LoadTank();
    }
    public void SaveTank()
    {
        string newName = _tUI._inputField.textComponent.text;
        SavePlayerData.SavePlayer(DataStorage.Singleton.saveSlot, DataStorage.Singleton.playerData);
    }
    public void LoadTank()
    {
        _tGeo.SpawnTankForCreator();
    }
}
