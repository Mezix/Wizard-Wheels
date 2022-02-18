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
    public Dropdown _partsDropDown;
    public Dropdown _layersDropDown;
    public CreateTankMouseScript _mouse;

    public GameObject _layersParent;
    private List<UILayer> layers = new List<UILayer>();

    //  ALL TILES
    public int _tileTypeIndex;

    //  Floor
    public List<Tile> _floorTiles;
    private List<Dropdown.OptionData> floorTilesList = new List<Dropdown.OptionData>();
    public int floorTileIndex;

    //  Roof
    public List<Tile> _roofTiles;
    private List<Dropdown.OptionData> roofTilesList = new List<Dropdown.OptionData>();
    public int roofTileIndex;

    //  Walls
    public List<Tile> _wallTiles;
    private List<Dropdown.OptionData> wallTilesList = new List<Dropdown.OptionData>();
    public int wallTileIndex;

    private void Start()
    {
        _tileTypeIndex = 0;
        InitButtons();
        InitPartsDropdown();
        InitLayers();
        SelectPart(0);
        SelectList(_tileTypeIndex);
    }
    public void InitButtons()
    {
        _saveTankButton.onClick.AddListener(() => CreateTankSceneManager.instance.SaveTank());
        _loadTankButton.onClick.AddListener(() => CreateTankSceneManager.instance.LoadTank());
    }
    private void InitPartsDropdown()
    {
        _partsDropDown.options.Clear();
        foreach (Tile t in _floorTiles)
        {
            Dropdown.OptionData newOptData = new Dropdown.OptionData();
            newOptData.text = t.name;
            newOptData.image = t.sprite;
            floorTilesList.Add(newOptData);
        }
        foreach (Tile t in _roofTiles)
        {
            Dropdown.OptionData newOptData = new Dropdown.OptionData();
            newOptData.text = t.name;
            newOptData.image = t.sprite;
            roofTilesList.Add(newOptData);
        }
    }
    private void InitLayers()
    {
        int i = 0;
        foreach(Dropdown.OptionData data in _layersDropDown.options)
        {
            GameObject go = Instantiate((GameObject) Resources.Load("TankCreator\\LayerPrefab"));
            go.GetComponentInChildren<Text>().text = data.text;
            go.transform.parent = _layersParent.transform;
            go.transform.position = Vector3.zero;
            UILayer uil = go.GetComponent<UILayer>();
            uil.index = i;
            uil._layerShown = true;
            layers.Add(uil);
            i++;
        }
    }
    public void SelectList(int listNr)
    {
        _tileTypeIndex = listNr;

        foreach (UILayer uil in layers) uil.BG.color = Color.gray;
        layers[_tileTypeIndex].BG.color = Color.white;

        if (listNr == 0)
        {
            _partsDropDown.options = floorTilesList;
        }
        else if (listNr == 1)
        {
            _partsDropDown.options = roofTilesList;
        }
    }

    public void ShowLayer(bool b, int index)
    {
        if (index == 0)
        {
            CreateTankSceneManager.instance._tGeo.FloorTilemap.gameObject.SetActive(b);
        }
        if (index == 1)
        {
            CreateTankSceneManager.instance._tGeo.RoofTilemap.gameObject.SetActive(b);
        }
    }
    public void SelectPart(int partNr)
    {
        if(_tileTypeIndex == 0)
        {
            floorTileIndex = partNr;
        }
        else if (_tileTypeIndex == 1)
        {
            roofTileIndex = partNr;
        }
    }
    public Tile GetTile()
    {
        if (_tileTypeIndex == 0)
        {
            return _floorTiles[floorTileIndex];
        }
        else if (_tileTypeIndex == 1)
        {
            return _roofTiles[roofTileIndex];
        }
        else return _floorTiles[floorTileIndex];
    }
}