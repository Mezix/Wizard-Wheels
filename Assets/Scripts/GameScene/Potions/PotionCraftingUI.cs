using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PotionCraftingUI : MonoBehaviour
{
    public Image _currentBottle;
    public Button _currentBottleButton;
    public bool _showingBottles;
    public bool mouseHoveringOver;
    public VerticalLayoutGroup _layout;

    public List<GameObject> _potionsToSpawn;
    public List<APotionType> _spawnedPotions;
    private void Awake()
    {
        _currentBottleButton.onClick.AddListener(() => ShowPotions(!_showingBottles));
    }
    private void Start()
    {
        InitTriggers();
        InitPotionUI();
        ShowPotions(false);
    }
    private void Update()
    {
        CheckHover();
    }
    private void InitTriggers()
    {
        EventTrigger trigger = _currentBottleButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }
    public void InitPotionUI()
    {
        foreach(GameObject potion in _potionsToSpawn)
        {
            GameObject g = Instantiate(potion);
            APotionType p = g.GetComponent<APotionType>();

            g.name = p._potionName + "UI";
            g.transform.SetParent(_layout.transform, false);
            p._pUI = this;
            p.InitButtonsTriggers();
        }
    }
    private void CheckHover()
    {
        if (mouseHoveringOver) return;
        bool showBottles = false;
        foreach(APotionType pot in _spawnedPotions)
        {
            if (pot.mouseHoveringOver)
            {
                showBottles = true;
                break;
            }
        }
        ShowPotions(showBottles);
    }
    public void ToggleBottles()
    {
        _showingBottles = !_showingBottles;
        ShowPotions(_showingBottles);
    }
    public void ShowPotions(bool show)
    {
        _showingBottles = show;
        _layout.transform.gameObject.SetActive(show);
    }
    public void SelectPotion(APotionType potionType)
    {
        _currentBottle.sprite = potionType._potionImage.sprite;
        ShowPotions(false);
    }
    public void OnPointerEnterDelegate(PointerEventData data)
    {
        mouseHoveringOver = true;
    }
    public void OnPointerExitDelegate(PointerEventData data)
    {
        mouseHoveringOver = false;
    }
}
