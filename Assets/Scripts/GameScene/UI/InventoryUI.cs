using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject _inventoryObjects;
    public Button _closeInventoryButton;
    public RectTransform _content;
    public VerticalLayoutGroup _verticalLayoutGroup;

    public List<InventorySlot> _spawnedInventorySlots = new List<InventorySlot>();
    private int _inventorySlotsToSpawn = 33;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Show(!_inventoryObjects.activeSelf);
        }
    }
    private void Start()
    {
        _closeInventoryButton.onClick.AddListener(() => Show(false));
        Show(false);
        SpawnInventory();
    }


    public void Show(bool show)
    {
        _inventoryObjects.SetActive(show);
    }
    private void SpawnInventory()
    {
        int rowsToSpawn = Mathf.CeilToInt(_inventorySlotsToSpawn / 4f);
        float rowHeight = 32 * 5; //5 = scale of objects
        _content.sizeDelta = new Vector2(0, rowsToSpawn * rowHeight + _verticalLayoutGroup.spacing * (rowsToSpawn - 1));

        GameObject currentHorizontalLayoutGroup = (GameObject) Instantiate(Resources.Load(GS.UIPrefabs("InventoryHorizontalGroup")), _verticalLayoutGroup.transform, false);
        int slotCounter = 0;
        for (int i = 0; i < _inventorySlotsToSpawn; i++)
        {
            GameObject invSlotObj = Instantiate(Resources.Load(GS.UIPrefabs("InventorySlot")) as GameObject, currentHorizontalLayoutGroup.transform, false);
            InventorySlot inventorySlot = invSlotObj.GetComponent<InventorySlot>();
            _spawnedInventorySlots.Add(inventorySlot);

            slotCounter++;
            if (slotCounter >= 4) // spawn a row every 4 slots
            {
                currentHorizontalLayoutGroup = (GameObject) Instantiate(Resources.Load(GS.UIPrefabs("InventoryHorizontalGroup")), _verticalLayoutGroup.transform, false);
                slotCounter = 0;
            }
        }
    }
}
