using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject _inventoryObjects;
    public Button _closeInventoryButton;
    public RectTransform _content;
    public VerticalLayoutGroup _verticalLayoutGroup;

    public List<InventorySlot> _spawnedInventorySlots = new List<InventorySlot>();
    //public Dictionary<InventoryItem, int> InventoryList;

    private void Awake()
    {
        REF.InventoryUI = this;
    }
    private void Start()
    {
        _closeInventoryButton.onClick.AddListener(() => Show(false));
        Show(false);
        SpawnInventory();
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
    private void SpawnInventory()
    {
        UnityEngine.Object[] inventoryItemTypeList = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem));

        int rowsToSpawn = Mathf.CeilToInt(inventoryItemTypeList.Length / 4f);
        float rowHeight = 32 * 5; //5 = scale of objects
        _content.sizeDelta = new Vector2(0, rowsToSpawn * rowHeight + 5 * _verticalLayoutGroup.spacing * (rowsToSpawn - 1));

        GameObject currentHorizontalLayoutGroup = (GameObject)Instantiate(Resources.Load(GS.UIPrefabs("InventoryHorizontalGroup")), _verticalLayoutGroup.transform, false);
        int slotCounter = 0;

        foreach (InventoryItem item in inventoryItemTypeList)
        {
            GameObject invSlotObj = Instantiate(Resources.Load(GS.UIPrefabs("InventorySlot")) as GameObject, currentHorizontalLayoutGroup.transform, false);
            InventorySlot inventorySlot = invSlotObj.GetComponent<InventorySlot>();

            inventorySlot._inventorySlotName.text = item.Name;
            inventorySlot._inventorySlotAmount.text = "2";
            inventorySlot._inventoryItemImage.sprite = item.Image;
            _spawnedInventorySlots.Add(inventorySlot);

            slotCounter++;
            if (slotCounter >= 4) // spawn a row every 4 slots
            {
                currentHorizontalLayoutGroup = (GameObject)Instantiate(Resources.Load(GS.UIPrefabs("InventoryHorizontalGroup")), _verticalLayoutGroup.transform, false);
                slotCounter = 0;
            }
        }
    }
}
