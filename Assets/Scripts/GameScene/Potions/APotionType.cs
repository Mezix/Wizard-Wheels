using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class APotionType : MonoBehaviour
{
    [HideInInspector]
    public PotionCraftingUI _pUI;

    public Image _potionImage;
    public Button _potionButton;
    public string _potionName;
    public Text _potionAmountText;
    public int _amountOfPotions;

    public bool mouseHoveringOver = false;
    private void Awake()
    {
        _amountOfPotions = 0;
        UpdatePotionAmount();
    }
    internal void InitButtonsTriggers()
    {
        EventTrigger trigger = _potionButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);

        _potionButton.onClick.AddListener(() => SelectPotion());
    }

    public void SelectPotion()
    {
        _pUI.SelectPotion(this);
    }
    public void OnPointerEnterDelegate(PointerEventData data)
    {
        mouseHoveringOver = true;
    }
    public void OnPointerExitDelegate(PointerEventData data)
    {
        mouseHoveringOver = false;
    }
    public void UpdatePotionAmount()
    {
        _potionAmountText.text = _amountOfPotions.ToString();
    }
}
