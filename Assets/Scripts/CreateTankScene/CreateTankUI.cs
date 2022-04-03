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

    public GameObject _layersParent;
    private List<UILayer> layers = new List<UILayer>();

    //  ALL TILES
    public int _tileTypeIndex;

    //  Floor
    public List<Tile> _floorTiles;
    private List<Dropdown.OptionData> floorTilesList = new List<Dropdown.OptionData>();
    public int floorIndex;

    //  Roof
    public List<Tile> _roofTiles;
    private List<Dropdown.OptionData> roofTilesList = new List<Dropdown.OptionData>();
    public int roofIndex;

    //  Walls
    public List<GameObject> _wallsGOList;
    private List<Dropdown.OptionData> wallTilesList = new List<Dropdown.OptionData>();
    public int wallIndex;

    //  Tires
    public List<GameObject> _tiresGOList;
    private List<Dropdown.OptionData> tiresList = new List<Dropdown.OptionData>();
    public int tiresIndex;

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
        foreach (GameObject wall in _wallsGOList)
        {
            Dropdown.OptionData newOptData = new Dropdown.OptionData();
            newOptData.text = wall.name;
            //newOptData.image = t.sprite;
            wallTilesList.Add(newOptData);
        }
        foreach (GameObject tire in _tiresGOList)
        {
            Dropdown.OptionData newOptData = new Dropdown.OptionData();
            newOptData.text = tire.name;
            //newOptData.image = t.sprite;
            tiresList.Add(newOptData);
        }
        floorIndex = 0;
        roofIndex = 0;
        wallIndex = 0;
        tiresIndex = 0;
    }
    private void InitLayers()
    {
        int i = 0;
        foreach(Dropdown.OptionData data in _layersDropDown.options)
        {
            GameObject go = Instantiate((GameObject) Resources.Load("TankCreator/LayerPrefab"));
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
            SelectPart(floorIndex);
        }
        if (listNr == 1)
        {
            _partsDropDown.options = roofTilesList;
            SelectPart(roofIndex);
        }
        if (listNr == 2)
        {
            _partsDropDown.options = wallTilesList;
            SelectPart(wallIndex);
        }
        if (listNr == 3)
        {
            _partsDropDown.options = tiresList;
            SelectPart(tiresIndex);
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
        if (index == 2)
        {
            //  Hide Walls
        }
        if (index == 3)
        {
            // Hide Tires
        }
    }
    public void SelectPart(int partNr)
    {
        if(_tileTypeIndex == 0)
        {
            floorIndex = partNr;
        }
        if (_tileTypeIndex == 1)
        {
            roofIndex = partNr;
        }
        if (_tileTypeIndex == 2)
        {
            wallIndex = partNr;
        }
        if (_tileTypeIndex == 3)
        {
            tiresIndex = partNr;
        }
        _partsDropDown.value = partNr;
    }
    public Tile GetTile()
    {
        if (_tileTypeIndex == 0)
        {
            return _floorTiles[floorIndex];
        }
        else if (_tileTypeIndex == 1)
        {
            return _roofTiles[roofIndex];
        }
        else return _floorTiles[floorIndex];
    }
}