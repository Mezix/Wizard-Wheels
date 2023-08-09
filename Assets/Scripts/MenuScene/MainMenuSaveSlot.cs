using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSaveSlot : MonoBehaviour
{
    public PlayerData _playerData;
    public int _slotIndex;
    public Button _selectButton;
    public Button _deleteButton;
    public Text _saveSlotNameText;
    public Text _timePlayedText;
    public void UpdateValues(int saveSlot, PlayerData data)
    {
        _slotIndex = saveSlot;
        if (data != null)
        {
            _saveSlotNameText.text = "Save File " + (saveSlot+1);
            _timePlayedText.gameObject.SetActive(true);
            _timePlayedText.text = HM.SecondsToTimeDisplay(data.TimeInSecondsPlayed);
            _deleteButton.gameObject.SetActive(true);
        }
        else
        {
            _saveSlotNameText.text = "EMPTY";
            _timePlayedText.gameObject.SetActive(false);
            _deleteButton.gameObject.SetActive(false);
        }
    }
}
