using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image _inventoryItemImage;
    public Text _inventorySlotName;
    public Text _inventorySlotAmount;
    public AudioSource _highlightedSound;

    public void PlayHighlightedSound() //assigned in editor
    {
        _highlightedSound.Play();
    }
}
