using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DataManipulationRouteNode : MonoBehaviour
{
    public Dropdown _dropDown;
    public Toggle _visitedToggle;

    public PlayerData.NodeEventType _currentlySelectedEventType;
    public bool _visitedStatus;
    private void Start()
    {
        InitEventTypes();
        _dropDown.onValueChanged.AddListener( delegate { ChangeEventType() ; });
        _visitedToggle.onValueChanged.AddListener( delegate { ChangeVisitedStatus(_visitedToggle.isOn) ; });
    }

    private void InitEventTypes()
    {
        _dropDown.options.Clear();
        foreach(PlayerData.NodeEventType type in Enum.GetValues(typeof(PlayerData.NodeEventType)).Cast<PlayerData.NodeEventType>().ToList())
        {
            Dropdown.OptionData newOptData = new Dropdown.OptionData();
            newOptData.text = type.ToString();
            _dropDown.options.Add(newOptData);
        }
    }
    private void ChangeEventType()
    {
        _currentlySelectedEventType = Enum.GetValues(typeof(PlayerData.NodeEventType)).Cast<PlayerData.NodeEventType>().ToList()[_dropDown.value];
        DataManipulationManager.instance.UpdateTempEventList();
    }
    private void ChangeVisitedStatus(bool status)
    {
        _visitedStatus = status;
        DataManipulationManager.instance.UpdateTempEventList();
    }
}
