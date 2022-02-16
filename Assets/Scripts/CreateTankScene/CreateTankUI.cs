using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class CreateTankUI : MonoBehaviour
{
    public Button _saveTankButton;
    public Button _loadTankButton;
    public InputField _inputField;
    public Dropdown _dropDown;
    public CreateTankMouseScript _mouse;

    public List<Tile> _useableTiles;
    public int currentTileIndex;

    private void Start()
    {
        InitButtons();
        InitPartsDropdown();
        SelectPart(0);
    }
    public void InitButtons()
    {
        _saveTankButton.onClick.AddListener(() => CreateTankSceneManager.instance.SaveTank());
        _loadTankButton.onClick.AddListener(() => CreateTankSceneManager.instance.LoadTank());
    }
    private void InitPartsDropdown()
    {
        _dropDown.options.Clear();
        foreach(Tile t in _useableTiles)
        {
            Dropdown.OptionData newTile = new Dropdown.OptionData();
            newTile.text = t.name;
            newTile.image = t.sprite;
            _dropDown.options.Add(newTile);
        }
    }
    public void SelectPart(int partNr)
    {
        currentTileIndex = partNr;
    }
}