using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Button _smallMapButton;
    public Button _closeMapButton;

    [Space(30)]
    public GameObject _bigMap;
    public Image _bigMapImage;
    public RectTransform _bigMapRect;

    [Space(30)]
    public Image _fogOfWarImage;
    public RectTransform _fogOfWarRect;

    [HideInInspector]
    public bool _mapOpen;

    private void Awake()
    {
        _smallMapButton.onClick.AddListener(() => OpenMap());
        _closeMapButton.onClick.AddListener(() => CloseMap());
    }
    private void Start()
    {
        CloseMap();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        if (!_mapOpen) OpenMap();
        else CloseMap();
    }
    public void OpenMap()
    {
        _bigMap.SetActive(true);
        _mapOpen = true;
    }
    public void CloseMap()
    {
        _bigMap.SetActive(false);
        _mapOpen = false;
    }
}
