using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManipulationManager : MonoBehaviour
{
    public Dropdown _saveSlotDropDown;
    public InputField _scrapInputField;
    public Button _saveButton;
    public Button _loadButton;

    private void Start()
    {
        //LoadData(0);

        _saveSlotDropDown.onValueChanged.AddListener(delegate { LoadData(_saveSlotDropDown.value);});
        _saveButton.onClick.AddListener(() => SaveData());
        _loadButton.onClick.AddListener(() => LoadData(_saveSlotDropDown.value));
    }

    private void SaveData()
    {
        Debug.Log("Saving in save slot " + _saveSlotDropDown.value);
        SavePlayerData.SavePlayer(_saveSlotDropDown.value, int.Parse(_scrapInputField.text));
    }

    private void LoadData(int i)
    {
        Debug.Log("Loading save slot " + i);
        PlayerData data = SavePlayerData.LoadPlayer(i);
        _scrapInputField.text = data.scrapAmount.ToString();
    }
}
