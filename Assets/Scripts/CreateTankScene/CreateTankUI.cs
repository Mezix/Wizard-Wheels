﻿using System;
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
    public Text _tankWidth;
    public Text _tankHeight;

    public GameObject _layersParent;
    private List<UILayer> layers = new List<UILayer>();

    //  ALL TILES
    public int _partTypeIndex;

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

    //  Tires
    public List<GameObject> _systemGOList;
    private List<Dropdown.OptionData> systemsList = new List<Dropdown.OptionData>();
    public int systemsIndex;

    private void Start()
    {
        _partTypeIndex = 0;
        InitButtons();
        InitPartsDropdown();
        InitLayers();
        SelectPart(0);
        SelectList(_partTypeIndex);
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
        foreach (GameObject system in _systemGOList)
        {
            Dropdown.OptionData newOptData = new Dropdown.OptionData();
            newOptData.text = system.name;
            //newOptData.image = t.sprite;
            systemsList.Add(newOptData);
        }
        floorIndex = 0;
        roofIndex = 0;
        wallIndex = 0;
        tiresIndex = 0;
        systemsIndex = 0;
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
    public void NextItemInList()
    {
        if (_partTypeIndex == 0)
        {
            floorIndex++;
            if (floorIndex > floorTilesList.Count - 1) floorIndex = 0;
        }
        if (_partTypeIndex == 1)
        {
            roofIndex++;
            if (roofIndex > roofTilesList.Count - 1) roofIndex = 0;
        }
        if (_partTypeIndex == 2)
        {
            wallIndex++;
            if (wallIndex > wallTilesList.Count - 1) wallIndex = 0;
        }
        if (_partTypeIndex == 3)
        {
            tiresIndex++;
            if (tiresIndex > tiresList.Count - 1) tiresIndex = 0;
        }
        if (_partTypeIndex == 4)
        {
            systemsIndex++;
            if (systemsIndex > systemsList.Count - 1) systemsIndex = 0;
        }
        SelectList(_partTypeIndex);
    }
    public void PreviousItemInList()
    {
        if (_partTypeIndex == 0)
        {
            floorIndex--;
            if (floorIndex < 0) floorIndex = floorTilesList.Count-1;
        }
        if (_partTypeIndex == 1)
        {
            roofIndex--;
            if (roofIndex < 0) roofIndex = roofTilesList.Count - 1;
        }
        if (_partTypeIndex == 2)
        {
            wallIndex--;
            if (wallIndex < 0) wallIndex = wallTilesList.Count - 1;
        }
        if (_partTypeIndex == 3)
        {
            tiresIndex--;
            if (tiresIndex < 0) tiresIndex = tiresList.Count - 1;
        }
        if (_partTypeIndex == 4)
        {
            systemsIndex--;
            if (systemsIndex < 0) systemsIndex = systemsList.Count - 1;
        }
        SelectList(_partTypeIndex);
    }
    public void SelectList(int partType)
    {
        _partTypeIndex = partType;

        foreach (UILayer uil in layers) uil.BG.color = Color.gray;
        layers[_partTypeIndex].BG.color = Color.white;

        if (partType == 0)
        {
            _partsDropDown.options = floorTilesList;
            SelectPart(floorIndex);
        }
        if (partType == 1)
        {
            _partsDropDown.options = roofTilesList;
            SelectPart(roofIndex);
        }
        if (partType == 2)
        {
            _partsDropDown.options = wallTilesList;
            SelectPart(wallIndex);
        }
        if (partType == 3)
        {
            _partsDropDown.options = tiresList;
            SelectPart(tiresIndex);
        }
        if (partType == 4)
        {
            _partsDropDown.options = systemsList;
            SelectPart(systemsIndex);
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
        if(_partTypeIndex == 0)
        {
            floorIndex = partNr;
        }
        if (_partTypeIndex == 1)
        {
            roofIndex = partNr;
        }
        if (_partTypeIndex == 2)
        {
            wallIndex = partNr;
        }
        if (_partTypeIndex == 3)
        {
            tiresIndex = partNr;
        }
        if (_partTypeIndex == 4)
        {
            systemsIndex = partNr;
        }
        _partsDropDown.value = partNr;
    }
    public Tile GetTile()
    {
        if (_partTypeIndex == 0)
        {
            return _floorTiles[floorIndex];
        }
        else if (_partTypeIndex == 1)
        {
            return _roofTiles[roofIndex];
        }
        else return _floorTiles[floorIndex];
    }
    public GameObject GetTirePrefab()
    {
        return _tiresGOList[tiresIndex];
    }
    public GameObject GetSystemPrefab()
    {
        return _systemGOList[systemsIndex];
    }
    public void UpdateSize(int width, int height)
    {
        _tankWidth.text = width.ToString();
        _tankHeight.text = height.ToString();
    }
}