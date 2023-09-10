using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionSceneComponent : MonoBehaviour
{
    public Image _icon;
    public Text _amountText;
    public int _amount;
    public string _objectName;

    public bool ChangeAmountPossible(int amountToAdd)
    {
        if (_amount + amountToAdd >= 0) return true;
        else return false;
    }
    public void SetAmount(int amountToAdd)
    {
        _amount += amountToAdd;
        _amountText.text = _objectName + _amount.ToString();
    }
}
