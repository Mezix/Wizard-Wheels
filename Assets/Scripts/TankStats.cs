using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/TankStats")]
public class TankStats : ScriptableObject
{
    public string _tankName = "";
    public int _tankHealth;
    public float _tankMaxSpeed;
    public float _tankAccel;
    public float _tankDecel;
    public float _rotationSpeed;
}
