using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class FreeLootEventUI : MonoBehaviour
{
    public GameObject _allObjects;
    public Button _finishEventButton;
    public HorizontalLayoutGroup _lootLayoutGroup;
    private InventorySlot _inventorySlotPrefab;
    public List<InventoryItemData> _lootItems = new List<InventoryItemData>();
    private void Awake()
    {
        _inventorySlotPrefab = Resources.Load(GS.UIPrefabs("InventorySlot"), typeof (InventorySlot)) as InventorySlot;
    }
    public void Show(bool show)
    {
        _allObjects.SetActive(show);
    }
    public void Init()
    {
        _finishEventButton.onClick.AddListener(() => FinishEvent());
        CreateLootToSpawn();
    }
    public void CreateLootToSpawn()
    {
        int itemsToSpawn = UnityEngine.Random.Range(3, 6);
        InventoryItem[] allPossibleInventoryItems = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem)).Cast<InventoryItem>().ToArray();
        for (int i = 0; i < itemsToSpawn; i++)
        {
            int typeOfItemToSpawn = UnityEngine.Random.Range(0, allPossibleInventoryItems.Length);
            int amountToSpawn = UnityEngine.Random.Range(3, 100);
            InventorySlot tmpSlot = Instantiate(_inventorySlotPrefab, _lootLayoutGroup.transform, false);
            tmpSlot._inventoryItemImage.sprite = allPossibleInventoryItems[typeOfItemToSpawn].Image;
            tmpSlot._inventorySlotName.text = allPossibleInventoryItems[typeOfItemToSpawn].Name;
            tmpSlot._inventorySlotAmount.text = amountToSpawn.ToString();

            _lootItems.Add(new InventoryItemData
            {
                Name = allPossibleInventoryItems[typeOfItemToSpawn].Name,
                SpritePath = GS.InventoryGraphics(allPossibleInventoryItems[typeOfItemToSpawn].Image.name),
                Amount = amountToSpawn
            });
        }
    }

    public void FinishEvent()
    {
        DataStorage.Singleton.AddInventoryItemsToPlayerData(_lootItems);
        SavePlayerData.SavePlayer(DataStorage.Singleton.saveSlot, DataStorage.Singleton.playerData);
        DataStorage.Singleton.FinishEvent();
    }
}
