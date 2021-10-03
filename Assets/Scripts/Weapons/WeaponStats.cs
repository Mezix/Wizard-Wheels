﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string _weaponName = "";
    public Sprite _weaponSprite;
    public float _damage;
    public float _attacksPerSecond;
    public float _rotationSpeed;
}