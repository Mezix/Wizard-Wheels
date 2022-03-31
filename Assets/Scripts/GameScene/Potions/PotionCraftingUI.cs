using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionCraftingUI : MonoBehaviour
{
    public Image _currentBottle;
    public Button _currentBottleButton;
    public bool _showingBottles;
    public HorizontalLayoutGroup _layout;

    public List<GameObject> _potionsToSpawn;
    public List<APotionType> _spawnedPotions;
    private void Awake()
    {
        _currentBottleButton.onClick.AddListener(() => ShowPotions(!_showingBottles));
    }
    private void Start()
    {
        InitPotionUI();
        ShowPotions(false);
    }
    private void Update()
    {
        
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
}
