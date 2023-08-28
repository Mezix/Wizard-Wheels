using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopEventUI : MonoBehaviour
{
    public GameObject _allObjects;
    public Button _finishEventButton;

    public VerticalLayoutGroup shopItemVLayoutGroup;
    public RectTransform _content;
    private int _itemsToSpawn;

    //  Shop Items
    private ShopEventItem _shopItemPrefab;
    private float _shopItemHeight;
    public List<ShopEventItem> _shopItems = new List<ShopEventItem>();

    //  Cash
    public Text _currentScrap;
    private void Awake()
    {
        _shopItemPrefab = Resources.Load(GS.UIPrefabs("ShopItem"), typeof (ShopEventItem)) as ShopEventItem;
        _shopItemHeight = _shopItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }
    public void Show(bool show)
    {
        _allObjects.SetActive(show);
    }

    public void Init()
    {
        _finishEventButton.onClick.AddListener(() => DataStorage.Singleton.FinishEvent());
        _currentScrap.text = DataStorage.Singleton.playerData.GetScrap().ToString();
        _itemsToSpawn = UnityEngine.Random.Range(3, 8);

        List<InventoryItem> inventoryItemTypeList = Resources.LoadAll(GS.ScriptableObjects("InventoryItems"), typeof(InventoryItem)).Cast<InventoryItem>().ToList();
        
        for (int i = 0; i < _itemsToSpawn; i++)
        {
            ShopEventItem item = Instantiate(_shopItemPrefab, shopItemVLayoutGroup.transform, false);
            int itemToSpawnIndex = UnityEngine.Random.Range(0, inventoryItemTypeList.Count);
            InventoryItem invItem = inventoryItemTypeList[itemToSpawnIndex];
            int amountToBeAbleToSell = UnityEngine.Random.Range(1, 4);

            item.Init(invItem.Name, invItem.Image, invItem.Price, amountToBeAbleToSell, itemToSpawnIndex);
            item._buyItemButton.onClick.AddListener(() => BuyItem(item));
            _shopItems.Add(item);
        }
        _content.sizeDelta = new Vector2(_content.sizeDelta.x, _itemsToSpawn * _shopItemHeight + _itemsToSpawn * shopItemVLayoutGroup.spacing);
    }

    public void BuyItem(ShopEventItem item)
    {
        DataStorage.Singleton.playerData.InventoryList[item._invListIndex].Amount += item._amountSelected;
        DataStorage.Singleton.playerData.SetScrap(DataStorage.Singleton.playerData.GetScrap() - (item._amountSelected * item._pricePerItem));
        _currentScrap.text = DataStorage.Singleton.playerData.GetScrap().ToString();
        item.BuySelected();

        foreach(ShopEventItem shopItem in _shopItems)
        {
            item.UpdateAmount();
        }
    }
}
