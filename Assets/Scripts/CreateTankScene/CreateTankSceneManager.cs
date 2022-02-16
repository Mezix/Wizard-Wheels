using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CreateTankSceneManager : MonoBehaviour
{
    public static CreateTankSceneManager instance;
    public CreateTankGeometry _tGeo;
    public CreateTankUI _tUI;
    public TankRoomConstellation tankToEdit;
    private bool newTank;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (tankToEdit) newTank = false;
        else
        {
            newTank = true;
            tankToEdit = new TankRoomConstellation();
            _tUI._inputField.textComponent.text = "tmp";
        }

        _tGeo._tankRoomConstellation = tankToEdit;
        LoadTank();
    }
    public void SaveTank()
    {
        string newName = _tUI._inputField.textComponent.text;
        if (newTank)
        {
            if (newName == "") newName = "tmp";
            AssetDatabase.CreateAsset(tankToEdit, "Assets\\Scripts\\GameScene\\SOs\\TankConstellation\\" + newName);
        }
        tankToEdit.SaveTank(newName);
    }
    public void LoadTank()
    {
        tankToEdit.InitTankForCreation();
        _tGeo.SpawnTankForCreator();
        _tUI._inputField.placeholder.GetComponent<Text>().text = tankToEdit.name;
    }
}
