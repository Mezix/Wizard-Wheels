using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitStats")]
public class UnitStats : ScriptableObject
{
    public string _unitName = "";
    public string _unitClass = "";
    public float _unitHealth;
    public float _unitSpeed;
}
