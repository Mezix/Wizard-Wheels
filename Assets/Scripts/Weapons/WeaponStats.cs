using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public float Damage;
    public float AttacksPerSecond;
}
