using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTankUI : MonoBehaviour
{
    public Button _saveTankButton;
    public Button _loadTankButton;
    public InputField _inputField;
    public Dropdown _dropDown;
    public CreateTankMouseScript _mouse;

    private void Start()
    {
        InitButtons();
    }
    public void InitButtons()
    {
        _saveTankButton.onClick.AddListener(() => CreateTankSceneManager.instance.SaveTank());
        _loadTankButton.onClick.AddListener(() => CreateTankSceneManager.instance.LoadTank());
    }
}
