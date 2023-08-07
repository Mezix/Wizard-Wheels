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
    public static DataManipulationManager instance;
    private PlayerData playerData;

    //  SAVING
    public Dropdown _saveSlotDropDown;
    public Button _saveButton;
    public Button _loadButton;
    public Button _resetButton;

    //  INVENTORY
    public VerticalLayoutGroup _InventoryLayoutGroup;
    private GameObject _invSlotPrefab;
    public RectTransform _invContent;
    public List<InventoryItemData> SceneInventoryList = new List<InventoryItemData>();
    public List<DataManipulationInputSlot> _spawnedInventorySlots;

    // ROUTE CREATOR
    public List<EventNode> SceneEventNodes;
    public VerticalLayoutGroup _routeCreatorVLayoutGroup;
    public RectTransform _routeCreatorContent;
    private DataManipulationRouteNode _dataManipulationRouteNodePrefab;
    private List<DataManipulationRouteNode> _nodeList = new List<DataManipulationRouteNode>();
    public GameObject _addSubtractNodeParent;
    public Button _addNodeButton;
    public Button _subtractNodeButton;

    private void Awake()
    {
        instance = this;
        _dataManipulationRouteNodePrefab = Resources.Load(GS.DataScenePrefabs("DataManipulationRouteNode"), typeof (DataManipulationRouteNode)) as DataManipulationRouteNode;
        _invSlotPrefab = Resources.Load(GS.DataScenePrefabs("InventoryManipulationItemSlot")) as GameObject;
    }
    private void Start()
    {
        InitInventory();

        LoadData(0);

        _saveSlotDropDown.onValueChanged.AddListener(delegate { LoadData(_saveSlotDropDown.value);});
        _saveButton.onClick.AddListener(() => SaveData());
        _loadButton.onClick.AddListener(() => LoadData(_saveSlotDropDown.value));
        _resetButton.onClick.AddListener(() => ResetSaveSlot(_saveSlotDropDown.value));

        _addNodeButton.onClick.AddListener(() => AddNewRouteNode(NodeEventType.Combat, false));
        _subtractNodeButton.onClick.AddListener(() => SubtractLastRouteNode());
    }

    private void AddNewRouteNode(NodeEventType type, bool status)
    {
        DataManipulationRouteNode newNode = Instantiate(_dataManipulationRouteNodePrefab, _routeCreatorVLayoutGroup.transform, false);
        newNode._currentlySelectedEventType = type;
        newNode._visitedToggle.isOn = status;
        _nodeList.Add(newNode);
        _addSubtractNodeParent.transform.SetAsLastSibling();
        UpdateTempEventList();
    }
    private void SubtractLastRouteNode()
    {
        if(_nodeList.Count > 0)
        {
            DataManipulationRouteNode nodeToDestroy = _nodeList[_nodeList.Count - 1];
            _nodeList.Remove(nodeToDestroy);
            Destroy(nodeToDestroy.gameObject);
            UpdateTempEventList();
        }
    }
    public void UpdateTempEventList()
    {
        SceneEventNodes = new List<EventNode>();
        foreach (DataManipulationRouteNode node in _nodeList)
        {
            SceneEventNodes.Add(new EventNode(node._currentlySelectedEventType, node._visitedStatus));
        }
    }

    private void InitInventory()
    {
        _spawnedInventorySlots = new List<DataManipulationInputSlot>();
        UnityEngine.Object[] inventoryItemTypeList = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem));

        _invContent.sizeDelta = new Vector2(_invContent.sizeDelta.x, (32 * 3) * inventoryItemTypeList.Length + (_InventoryLayoutGroup.spacing * 3) * inventoryItemTypeList.Length-1);
        foreach (InventoryItem item in inventoryItemTypeList)
        {
            GameObject invSlotObj = Instantiate(_invSlotPrefab, _InventoryLayoutGroup.transform, false);
            DataManipulationInputSlot inventorySlot = invSlotObj.GetComponent<DataManipulationInputSlot>();

            inventorySlot._inventorySlotName.text = item.Name;
            inventorySlot._inventorySlotAmount.text = " Uninitialized";
            inventorySlot._manager = this;
            _spawnedInventorySlots.Add(inventorySlot);
        }
    }


    //  Saving Loading etc
    private void SaveData()
    {
        Debug.Log("Saving in save slot " + _saveSlotDropDown.value);
        SavePlayerData.SavePlayer(_saveSlotDropDown.value, SceneInventoryList, SceneEventNodes);
    }
    private void LoadData(int saveSlot)
    {
        Debug.Log("Loading save slot " + saveSlot);
        PlayerData data = SavePlayerData.LoadPlayer(saveSlot);
        if (data == null) { Debug.Log("No data found! Returning!"); return; };

        SceneInventoryList = new List<InventoryItemData>();
        if(data.InventoryList != null)
        {
            int counter = 0;
            foreach (InventoryItemData item in data.InventoryList)
            {
                _spawnedInventorySlots[counter]._inventorySlotName.text = item.Name;
                _spawnedInventorySlots[counter]._inventoryItemImage.sprite = Resources.Load(item.SpritePath, typeof(Sprite)) as Sprite;
                _spawnedInventorySlots[counter]._inventorySlotAmount.SetTextWithoutNotify(item.Amount.ToString());
                _spawnedInventorySlots[counter].index = counter;

                SceneInventoryList.Add(item);
                counter++;
            }
        }
        else Debug.LogWarning("No Inventory list found!");

        SceneEventNodes = new List<EventNode>();
        if (data.CurrentEventPath != null)
        {
            foreach (EventNode item in data.CurrentEventPath)
            {
                AddNewRouteNode(item._event, item._visited);
            }
        }
        else Debug.LogWarning("No Event Path found!");
    }
    private void ResetSaveSlot(int saveSlot)
    {
        Debug.Log("Resetting save slot " + _saveSlotDropDown.value);

        UnityEngine.Object[] inventoryItemTypeList = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem));

        SceneInventoryList.Clear();
        foreach (InventoryItem item in inventoryItemTypeList)
        {
            InventoryItemData tmpItem = new InventoryItemData();
            tmpItem.Name = item.Name;
            tmpItem.SpritePath = GS.InventoryGraphics(item.Image.name);
            tmpItem.Amount = 0;
            SceneInventoryList.Add(tmpItem);
        }
        SavePlayerData.SavePlayer(saveSlot, SceneInventoryList, SceneEventNodes);
    }

}
