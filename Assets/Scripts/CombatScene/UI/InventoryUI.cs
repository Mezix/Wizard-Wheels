using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class InventoryUI : MonoBehaviour
{
    public GameObject _inventoryObjects;
    public Button _closeInventoryButton;
    public RectTransform _content;
    public VerticalLayoutGroup _verticalLayoutGroup;

    public List<InventorySlot> _spawnedInventorySlots = new List<InventorySlot>();
    //public List<InventoryItemData> SceneInventoryList = new List<InventoryItemData>();

    private void Awake()
    {
        REF.InvUI = this;
    }
    private void Start()
    {
        _closeInventoryButton.onClick.AddListener(() => Show(false));
        Show(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Show(!_inventoryObjects.activeSelf);
        }
    }

    public void Show(bool show)
    {
        _inventoryObjects.SetActive(show);
    }
    public void SpawnInventory(PlayerData data)
    {
        //PlayerData data = SavePlayerData.LoadPlayer(LevelManager.instance.saveSlot);
        DataStorage.Singleton.playerData.InventoryList = data.InventoryList;

        GameObject currentHorizontalLayoutGroup = (GameObject)Instantiate(Resources.Load(GS.UIPrefabs("InventoryHorizontalGroup")), _verticalLayoutGroup.transform, false);

        int rowIndex = 0;
        int columnIndex = 0;
        int index = -1;
        foreach (InventoryItemData item in DataStorage.Singleton.playerData.InventoryList)
        {
            index++;
            if (item.Amount == 0) continue;
            if(item.Name == "Scrap")
            {
                //dont add scrap to the inventory
                REF.UpgrScreen._remainingScrap = item.Amount;
                REF.UpgrScreen.UpdateMainScrapCounter();
                REF.UpgrScreen.UpdateUpgradeScrapCounter();
                REF.UpgrScreen.scrapInventoryItemSlotIndex = index;
                continue;
            }

            GameObject invSlotObj = Instantiate(Resources.Load(GS.UIPrefabs("InventorySlot")) as GameObject, currentHorizontalLayoutGroup.transform, false);
            InventorySlot inventorySlot = invSlotObj.GetComponent<InventorySlot>();

            inventorySlot._inventorySlotName.text = item.Name;
            inventorySlot._inventorySlotAmount.text = item.Amount.ToString();
            inventorySlot._inventoryItemImage.sprite = Resources.Load(item.SpritePath, typeof(Sprite)) as Sprite;

            _spawnedInventorySlots.Add(inventorySlot);

            columnIndex++;
            if (columnIndex >= 4) // spawn a row every 4 slots
            {
                currentHorizontalLayoutGroup = (GameObject)Instantiate(Resources.Load(GS.UIPrefabs("InventoryHorizontalGroup")), _verticalLayoutGroup.transform, false);
                columnIndex = 0;
                rowIndex++;
            }
        }

        float rowHeight = 32 * 5; //5 = scale of objects
        _content.sizeDelta = new Vector2(0, rowIndex * rowHeight + 5 * _verticalLayoutGroup.spacing * (rowIndex - 1));
    }

    public void AddAmount(int index, int amount)
    {
        InventoryItemData data = DataStorage.Singleton.playerData.InventoryList[index];
        data.Amount += amount;
        DataStorage.Singleton.playerData.InventoryList[index] = data;
    }
}
