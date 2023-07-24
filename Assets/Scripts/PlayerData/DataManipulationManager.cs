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
    public VerticalLayoutGroup _InventoryLayoutGroup;

    public Dictionary<InventoryItem, int> InventoryDictionary;
    public List<InventorySlot> _spawnedInventorySlots = new List<InventorySlot>();

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
        
        SavePlayerData.SavePlayer(_saveSlotDropDown.value, InventoryDictionary);
    }

    private void LoadData(int i)
    {
        Debug.Log("Loading save slot " + i);
        PlayerData data = SavePlayerData.LoadPlayer(i);

        //_scrapInputField.text = data.InventoryList[Resources.Load(GS.InventoryItems("Scrap"), typeof(InventoryItem)) as InventoryItem].ToString();

        if (data.InventoryToItemAmountDictionary != null)
        {
            foreach (InventoryItem item in data.InventoryToItemAmountDictionary.Keys)
            {
                GameObject invSlotObj = Instantiate(Resources.Load(GS.UIPrefabs("InventorySlot")) as GameObject, _InventoryLayoutGroup.transform, false);
                InventorySlot inventorySlot = invSlotObj.GetComponent<InventorySlot>();

                inventorySlot._inventorySlotName.text = item.Name;
                inventorySlot._inventoryItemImage.sprite = item.Image;
                inventorySlot._inventorySlotAmount.text = data.InventoryToItemAmountDictionary[item].ToString();
                _spawnedInventorySlots.Add(inventorySlot);

                InventoryDictionary.Add(item, data.InventoryToItemAmountDictionary[item]);
            }
        }
        else
        {
            Debug.Log("Save slot " + i + " has no Inventory. Initializing default Inventory!");
            UnityEngine.Object[] inventoryItemTypeList = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem));

            foreach (InventoryItem item in inventoryItemTypeList)
            {
                InventoryDictionary.Add(item, 0);
            }
            SavePlayerData.SavePlayer(i, InventoryDictionary);
        }
    }
}
