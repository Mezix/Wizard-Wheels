using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class DataManipulationManager : MonoBehaviour
{
    public Dropdown _saveSlotDropDown;
    public Button _saveButton;
    public Button _loadButton;
    public VerticalLayoutGroup _InventoryLayoutGroup;
    public RectTransform _content;

    public List<InventoryItemData> SceneInventoryList = new List<InventoryItemData>();
    public List<DataManipulationInputSlot> _spawnedInventorySlots;

    private void Start()
    {
        InitInventory();

        ResetSaveSlot(0);
        ResetSaveSlot(1);
        ResetSaveSlot(2);

        LoadData(0);

        _saveSlotDropDown.onValueChanged.AddListener(delegate { LoadData(_saveSlotDropDown.value);});
        _saveButton.onClick.AddListener(() => SaveData());
        _loadButton.onClick.AddListener(() => LoadData(_saveSlotDropDown.value));
    }

    private void InitInventory()
    {
        _spawnedInventorySlots = new List<DataManipulationInputSlot>();
        UnityEngine.Object[] inventoryItemTypeList = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem));

        _content.sizeDelta = new Vector2(_content.sizeDelta.x, (32 * 3) * inventoryItemTypeList.Length + (_InventoryLayoutGroup.spacing * 3) * inventoryItemTypeList.Length-1);
        foreach (InventoryItem item in inventoryItemTypeList)
        {
            GameObject invSlotObj = Instantiate(Resources.Load(GS.UIPrefabs("InventoryManipulationItemSlot")) as GameObject, _InventoryLayoutGroup.transform, false);
            DataManipulationInputSlot inventorySlot = invSlotObj.GetComponent<DataManipulationInputSlot>();

            inventorySlot._inventorySlotName.text = item.Name;
            inventorySlot._inventorySlotAmount.text = " Uninitialized";
            inventorySlot._manager = this;
            _spawnedInventorySlots.Add(inventorySlot);
        }
    }

    private void SaveData()
    {
        Debug.Log("Saving in save slot " + _saveSlotDropDown.value);
        SavePlayerData.SavePlayer(_saveSlotDropDown.value, SceneInventoryList);
    }

    private void LoadData(int saveSlot)
    {
        Debug.Log("Loading save slot " + saveSlot);
        PlayerData data = SavePlayerData.LoadPlayer(saveSlot);
        if (data == null) { Debug.Log("No data found! Returning!"); return; };

        SceneInventoryList = new List<InventoryItemData>();
        int counter = 0;
        foreach (InventoryItemData item in data.InventoryList)
        {
            _spawnedInventorySlots[counter]._inventorySlotName.text = item.Name;
            _spawnedInventorySlots[counter]._inventoryItemImage.sprite = Resources.Load(item.SpritePath, typeof(Sprite)) as Sprite;
            Debug.Log(item.SpritePath);
            _spawnedInventorySlots[counter]._inventorySlotAmount.SetTextWithoutNotify(item.Amount.ToString());
            _spawnedInventorySlots[counter].index = counter;

            SceneInventoryList.Add(item);
            counter++;
        }
    }


    private void ResetSaveSlot(int saveSlot)
    {
        Debug.Log("Resetting save slot " + _saveSlotDropDown.value);

        UnityEngine.Object[] inventoryItemTypeList = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem));

        int stringStartRemove = "Assets/Resources/".Length; // Removes "Assets/Resources/"
        int fileExtensionRemove = ".png".Length;            // Removes ".png"

        SceneInventoryList.Clear();
        foreach (InventoryItem item in inventoryItemTypeList)
        {
            InventoryItemData tmpItem = new InventoryItemData();
            tmpItem.Name = item.Name;
            tmpItem.SpritePath = GS.InventoryGraphics(item.Image.name);
            tmpItem.Amount = 0;
            SceneInventoryList.Add(tmpItem);
        }
        SavePlayerData.SavePlayer(saveSlot, SceneInventoryList);
        SavePlayerData.SavePlayer(_saveSlotDropDown.value, SceneInventoryList);
    }
}
