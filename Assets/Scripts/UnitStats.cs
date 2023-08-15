using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitStats")]
public class UnitStats : ScriptableObject
{
    public string _unitName = "";
    public WizardType _unitClass;
    public float _unitHealth;
    public float _unitSpeed;

}
