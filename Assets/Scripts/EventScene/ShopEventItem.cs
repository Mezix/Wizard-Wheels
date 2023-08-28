using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopEventItem : MonoBehaviour
{
    public Text _itemName;
    public Image _itemImage;
    public Text _priceText;
    public int _pricePerItem;
    public Text _amountText;
    public int _amountLeft;
    public int _amountSelected;

    public Button _addAmountButton;
    public Button _subtractAmountButton;
    public Button _buyItemButton;

    public int _invListIndex;

    public void Init(string name, Sprite img, int pricePerItem, int amountToSell, int index)
    {
        _itemName.text = name;
        _itemImage.sprite = img;
        _priceText.text = pricePerItem.ToString();
        _amountText.text = "0/" + _amountLeft;
        _invListIndex = index;

        _amountSelected = 0;
        _amountLeft = amountToSell;
        _pricePerItem = pricePerItem;
        UpdateAmount();

        _addAmountButton.onClick.AddListener(() => AddAmount());
        _subtractAmountButton.onClick.AddListener(() => SubtractAmount());
    }
    public void UpdateAmount()
    {
        _addAmountButton.interactable = false;
        _subtractAmountButton.interactable = false;
        _buyItemButton.interactable = false;

        _amountText.text = _amountSelected + "/" + _amountLeft;

        if (_amountSelected > 0 )
        {
            _subtractAmountButton.interactable = true;

            if(DataStorage.Singleton.playerData.GetScrap() >= _pricePerItem * _amountSelected) _buyItemButton.interactable = true;
        }
        if (_amountLeft - _amountSelected > 0 && DataStorage.Singleton.playerData.GetScrap() >= _pricePerItem * (_amountSelected+1)) _addAmountButton.interactable = true;
    }
    public void AddAmount()
    {
        _amountSelected++;
        UpdateAmount();
    }
    public void SubtractAmount()
    {
        _amountSelected--;
        UpdateAmount();
    }

    public void BuySelected()
    {
        _amountLeft -= _amountSelected;
        _amountSelected = 0;
        UpdateAmount();
    }
}
