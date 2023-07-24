using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class DataManipulationInputSlot : MonoBehaviour
{
    public Image _inventoryItemImage;
    public Text _inventorySlotName;
    public InputField _inventorySlotAmount;
    public AudioSource _highlightedSound;
    public DataManipulationManager _manager;
    public int index;

    private void Start()
    {
        _inventorySlotAmount.onValueChanged.AddListener( delegate { ChangeAmountOfItem(); });
    }
    public void PlayHighlightedSound() //assigned in editor
    {
        _highlightedSound.Play();
    }
    public void ChangeAmountOfItem()
    {
        InventoryItemData newData = _manager.SceneInventoryList[index];
        newData.Name = _manager.SceneInventoryList[index].Name;
        newData.SpritePath = _manager.SceneInventoryList[index].SpritePath;
        newData.Amount = int.Parse(_inventorySlotAmount.text);

        _manager.SceneInventoryList[index] = newData;
    }
}
