using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string wpName = "";
    public Sprite weaponSprite;
    public float Damage;
    public float AttacksPerSecond;
}
